using AutoMapper;
using BusinessLogic.Extension;
using Core.Auth;
using Core.Interfaces;
using DataTransferObject.AuthDTO.Request;
using DataTransferObject.AuthDTO.Response;
using DataTransferObject.Entity;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RoboGas.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace BusinessLogic.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IHttpContextAccessor _context;
        public AuthService(IMapper mapper, SignInManager<User> signInManager, UserManager<User> userManager, IUnitOfWork unitOfWork
            , RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, TokenValidationParameters tokenValidationParameters, IHttpContextAccessor context)
        {
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
        }
        public async Task<APIResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var userByEmail = await _userManager.FindByEmailAsync(loginRequest.LoginType);
                var userByUserName = await _userManager.FindByNameAsync(loginRequest.LoginType);
                User user;

                if (userByEmail != null)    
                    user = userByEmail;
                else if (userByUserName != null)
                    user = userByUserName;
                else
                    return new APIResult { message = "InValid UserName Or Email" };

                if (!(await _userManager.CheckPasswordAsync(user, loginRequest.Password)))
                    return new APIResult { message = "InValid Password" };

                var token = await CreateJWTToken(user);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);


                var refreshTokenResult = await AddRefreshToken(user.Email);
                if (!refreshTokenResult.state)
                    return refreshTokenResult;

                var refreshToken = (RefreshTokenDevCreed)refreshTokenResult.entity!;

                return new APIResult
                {
                    state = true,
                    entity = new AccessAndRefreshToken
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken.Token
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResult { message = ex.InnerException.Message };
            }
        }
        public async Task<APIResult> Register(RegisterRequest registerRequest)
        {
            try
            {
                var user = _mapper.Map<User>(registerRequest);

                if (Validations.EmailOrPhone(registerRequest.Email) != Res.Email)
                    return new APIResult { message = "This is not Email Form" };

                if (await _userManager.FindByEmailAsync(registerRequest.Email) is not null)
                    return new APIResult { message = "This Email Exists Before" };

                if (await _userManager.FindByNameAsync(registerRequest.UserName) is not null)
                    return new APIResult { message = "This Name Exists Before" };

                var s = await _userManager.GetPhoneNumberAsync(user);

                if (await _userManager.GetPhoneNumberAsync(user) is null)
                    return new APIResult { message = "Please set PhoneNumber" };

                if (Validations.EmailOrPhone(registerRequest.PhoneNumber) != Res.PhoneNumber)
                    return new APIResult { message = "This is not PhoneNumber Form" };

                var phoneReturned = _unitOfWork.user.Query().Where(x => x.PhoneNumber == registerRequest.PhoneNumber).SingleOrDefault();

                if (phoneReturned is not null)
                    return new APIResult { message = "This PhoneNumber Exists before" };



                var returnedUser = await _userManager.CreateAsync(user, registerRequest.Password);
                string Errors = string.Empty;
                if (!returnedUser.Succeeded)
                {
                    foreach (var i in returnedUser.Errors)
                        Errors = Errors + i.Description + '\n';

                    return new APIResult { message = Errors };
                }

                await _userManager.AddToRoleAsync(user, Res.Visitor);

                var jwtSecurityToken = await CreateJWTToken(user);

                var refreshTokenResult = await AddRefreshToken(registerRequest.Email);
                if (!refreshTokenResult.state)
                    return refreshTokenResult;

                string refreshToken = ((RefreshTokenDevCreed)refreshTokenResult.entity!).Token;
                return new APIResult
                {
                    state = true,
                    entity = new AccessAndRefreshToken
                    {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                        RefreshToken = refreshToken
                    }
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public void GenerateCookie(RefreshTokenDevCreed refreshTokenDevCreed)
        {
            if (_context.HttpContext!.Request.Cookies["RefreshTokenCookie"] != null)
                _context.HttpContext.Response.Cookies.Delete("RefreshTokenCookie");

            //string refreshTokenObject = JsonSerializer.Serialize(refreshTokenDevCreed);
            //string o = refreshTokenObject;
            // Create a new cookie
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                //HttpOnly = true, // Set to true if the cookie should be accessible only through HTTP requests
                //Secure = true // Set to true if the cookie should only be sent over HTTPS
            };
            // Set the cookie in the response
            _context.HttpContext!.Response.Cookies.Append("RefreshTokenCookie", refreshTokenDevCreed.Token, options);

        }
        public async Task<APIResult> GenerateAccessTokenDueToRefreshToken(string RefreshToken)
        {
            try
            {
                var refreshToken = _unitOfWork.refreshToken.Query().Where(x => x.Token == RefreshToken!).SingleOrDefault();
                if (refreshToken == null)
                    return new APIResult { message = "RefreshToken Doesn't Exist" };
                if (!refreshToken!.IsActive)
                    return new APIResult { message = "RefreshToken Is UnValid" };
                var user = _userManager.FindByIdAsync(refreshToken.UserId).Result;
                var token = await CreateJWTToken(user);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                return new APIResult() { entity = accessToken, state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };

            }
        }
        public async Task<APIResult> GetRoles()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return new APIResult { entity = roles, state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> AddRole(RoleDTO roleDTO)
        {
            try
            {
                var isExist = await _roleManager.RoleExistsAsync(roleDTO.Name);
                if (isExist)
                    return new APIResult { message = Res.RoleExistsBefore };

                await _roleManager.CreateAsync(new IdentityRole { Name = roleDTO.Name.Trim() });
                return new APIResult { state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> AddRoleToUser(AddRoleToUserRequest addRoleToUserRequest)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(addRoleToUserRequest.UserId);
                if (user is null)
                    return new APIResult { message = "User Doesn't Exist" };

                var role = await _roleManager.RoleExistsAsync(addRoleToUserRequest.RoleName);
                if (!role)
                    return new APIResult { message = "Role Doesn't Exist" };

                if (await _userManager.IsInRoleAsync(user, addRoleToUserRequest.RoleName))
                    return new APIResult { message = "This Role Assigned To This User Before" };

                var result = await _userManager.AddToRoleAsync(user, addRoleToUserRequest.RoleName);
                if (!result.Succeeded)
                    return new APIResult { message = "SomeThing Went Wrong With adding Role" };

                return new APIResult { state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> SeedClaimsForRole(RoleDTO roleDTO)
        {
            try
            {
                var allPermissions = _roleManager.GenerateClaimsForRole(Res.Region);
                var allClaims = await _roleManager.GetClaimsAsync(new IdentityRole() { Name = roleDTO.Name });
                var role = await _roleManager.FindByNameAsync(roleDTO.Name);
                foreach (var per in allPermissions)
                {
                    if (!(allClaims.Any(x => x.Type == Res.Permission && x.Value == per)))
                        await _roleManager.AddClaimAsync(role, new Claim(Res.Permission, per));
                }
                return new APIResult { state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> GetUsersWithAssignedRoles()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                List<UserResponse> userResponses = new List<UserResponse>();
                foreach (var i in users)
                {
                    var roles = await _userManager.GetRolesAsync(i);
                    var userResponse = new UserResponse
                    {
                        FirstName = i.FirstName,
                        LastName = i.LastName,
                        Email = i.Email,
                        Id = i.Id,
                        UserName = i.UserName,
                        Roles = roles
                    };
                    userResponses.Add(userResponse);
                }
                return new APIResult { entity = userResponses, state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> CheckInRole(EmailRequest emailRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailRequest.Email);

                if (await _userManager.IsInRoleAsync(user, emailRequest.RoleName))
                    return new APIResult { state = true };

                return new APIResult { message = Res.ThisUserDoesntAssignedToThisRole };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> AddUserByAdmin(AddUserByAdminRequest addUserByAdminRequest)
        {
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    var email = await _userManager.FindByEmailAsync(addUserByAdminRequest.Email);
                    if (email != null)
                    {
                        transaction.Rollback();
                        return new APIResult { message = "Email Exists before" };
                    }

                    var userName = await _userManager.FindByNameAsync(addUserByAdminRequest.UserName);
                    if (userName != null)
                    {
                        transaction.Rollback();
                        return new APIResult { message = "UserName Exists before" };
                    }

                    var phoneNumber = await _userManager.Users.Where(x => x.PhoneNumber == addUserByAdminRequest.PhoneNumber).SingleOrDefaultAsync();
                    if (phoneNumber != null)
                    {
                        transaction.Rollback();
                        return new APIResult { message = "PhoneNumber Exists before" };
                    }

                    var user = _mapper.Map<User>(addUserByAdminRequest);
                    var Result = await _userManager.CreateAsync(user, addUserByAdminRequest.Password);
                    string Errors = string.Empty;
                    if (!Result.Succeeded)
                    {
                        foreach (var i in Result.Errors)
                            Errors = Errors + i.Description + '\n';

                        transaction.Rollback();
                        return new APIResult { message = Errors };
                    }

                    bool isExist = true;
                    foreach (var i in addUserByAdminRequest.Roles)
                        isExist = isExist && await _roleManager.RoleExistsAsync(i);

                    if (!isExist)
                    {
                        transaction.Rollback();
                        return new APIResult { message = Res.SomeRoleOfThoseRolesDoesntExist };
                    }

                    Result = await _userManager.AddToRolesAsync(user, addUserByAdminRequest.Roles.Select(x => x));
                    Errors = string.Empty;
                    if (!Result.Succeeded)
                    {
                        foreach (var i in Result.Errors)
                            Errors = Errors + i.Description + '\n';

                        transaction.Rollback();
                        return new APIResult { message = Errors };
                    }

                    transaction.Commit();
                    return new APIResult { state = true };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.InnerException != null)
                        return new APIResult { message = ex.InnerException.Message };
                    return new APIResult { message = ex.Message };
                }
            }
        }
        public async Task<APIResult> EditUserByAdmin(EditUserByAdminRequest editUserByAdminRequest)
        {
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(editUserByAdminRequest.Id);
                    var userEmail = await _userManager.FindByEmailAsync(editUserByAdminRequest.Email);
                    if (userEmail != null && user.Id != userEmail.Id)
                    {
                        transaction.Rollback();
                        return new APIResult { message = "Email Exists before" };
                    }

                    var userUserName = await _userManager.FindByNameAsync(editUserByAdminRequest.UserName);
                    if (userUserName != null && user.Id != userUserName.Id)
                    {
                        transaction.Rollback();
                        return new APIResult { message = "UserName Exists before" };
                    }

                    var phoneNumberUser = await _userManager.Users.Where(x => x.PhoneNumber == editUserByAdminRequest.PhoneNumber).AsNoTracking().SingleOrDefaultAsync();
                    if (phoneNumberUser != null && user.Id != phoneNumberUser.Id)
                    {
                        transaction.Rollback();
                        return new APIResult { message = "PhoneNumber Exists before" };
                    }

                    var userConverted = _mapper.Map<EditUserByAdminRequest, User>(editUserByAdminRequest, user);
                    var result = await _userManager.UpdateAsync(userConverted);
                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        result = await _userManager.RemoveFromRolesAsync(user, roles);
                        if (result.Succeeded)
                        {
                            bool isExist = true;
                            foreach (var i in editUserByAdminRequest.Roles)
                                isExist = isExist && await _roleManager.RoleExistsAsync(i);

                            if (!isExist)
                            {
                                transaction.Rollback();
                                return new APIResult { message = Res.SomeRoleOfThoseRolesDoesntExist };
                            }

                            result = await _userManager.AddToRolesAsync(user, editUserByAdminRequest.Roles.Select(r => r));
                            if (result.Succeeded)
                            {
                                transaction.Commit();
                                return new APIResult { state = true };
                            }
                        }
                    }
                    transaction.Rollback();
                    return new APIResult { message = Res.SomeWrongIssuesWithIdentity };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex.InnerException != null)
                        return new APIResult { message = ex.InnerException.Message };
                    return new APIResult { message = ex.Message };
                }
            }
        }
        public async Task<APIResult> DeleteUser(string Id)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                    return new APIResult { message = Res.UserWithThisIdDoesntExist };
                var result = await _userManager.DeleteAsync(user);
                string Errors = string.Empty;
                if (!result.Succeeded)
                {
                    foreach (var i in result.Errors)
                        Errors = Errors + i.Description + '\n';

                    return new APIResult { message = Errors };
                }

                return new APIResult { state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> AddClaimsToUser(AddClaimToUserRequest addClaimRequest)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(addClaimRequest.UserId);
                if (user == null)
                    return new APIResult { message = "User Doesn't Exist" };

                if ((await _userManager.GetClaimsAsync(user)).ToList().Any(x => x.Value == addClaimRequest.ClaimValue))
                    return new APIResult { message = "This Claims Exists For This User Before" };

                var result = await _userManager.AddClaimAsync(user, new Claim("roles", addClaimRequest.ClaimValue));
                if (!result.Succeeded)
                    return new APIResult { message = "Failed To Add Claims To User" };

                return new APIResult { state = true };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        public async Task<APIResult> AddClaimsToRole(AddClaimToRoleRequest addClaimToRoleRequest)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(addClaimToRoleRequest.RoleId);
                if (role == null)
                    return new APIResult { message = "Role Doesn't Exist" };

                var addedClaim = await _roleManager.AddClaimAsync(role, new Claim("roles", addClaimToRoleRequest.ClaimValue));
                if (!addedClaim.Succeeded)
                    return new APIResult { message = "Failed To Add Claims To Role" };

                return new APIResult { state = true };
                ;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };
            }
        }
        private async Task<JwtSecurityToken> CreateJWTToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roleClaims = new List<Claim>();

            // Get User Roles and add its to User Claims
            var userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            userClaims.Add(new Claim("roles", "Anonamouse"));
            userClaims.Add(new Claim("roles", "Anonamouse"));
            if (userRoles is not null && userRoles.Count > 0)
            {
                foreach (var userRole in userRoles)
                {
                    var role = await _roleManager.FindByNameAsync(userRole);
                    var claimsRelatedToRole = await _roleManager.GetClaimsAsync(role);
                    foreach (var claim in claimsRelatedToRole.ToList())
                    {
                        if (userClaims.Any(x => x != claim))
                            roleClaims.Add(claim);
                    }
                    roleClaims.Add(new Claim("roles", userRole));
                }
            }

            var claims = new[]
           {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            }
           .Union(userClaims)
           .Union(roleClaims);

            var jwtSecurityToken = new JwtSecurityToken(
               issuer: _jwt.Issuer,
               audience: _jwt.Audience,
               claims: claims,
               expires: DateTime.Now.AddDays(70),
               signingCredentials: signingCredentials
               );

            var x = DateTime.Now;

            return jwtSecurityToken;
            //var accessToken = jwtTokenHandler.WriteToken(jwtSecurityToken);
            //var refreshToken = new RefreshToken
            //{
            //    JwtId = jwtSecurityToken.Id,
            //    IsUsed = false,
            //    IsRevoked = false,
            //    UserId = user.Id,
            //    AddedDate = DateTime.Now,
            //    ExpiryDate = DateTime.UtcNow.AddMonths(6),
            //    Token = RandomString(32) + Guid.NewGuid()
            //};
            //_unitOfWork.refreshToken.Add(refreshToken);
            //_unitOfWork.Complete();
            //return new TokenRequest
            //{
            //    Token = accessToken,
            //    RefreshToken = refreshToken.Token
            //};
        }
        private async Task<APIResult> AddRefreshToken(string loginType)
        {
            var refreshToken = GenerateRefreshToken();

            var type = Validations.EmailOrPhone(loginType);
            User user = null;
            if (type == Res.Email)
                user = await _userManager.FindByEmailAsync(loginType);
            else if (type == Res.phoneType)
                user = _userManager.Users.Where(x => x.PhoneNumber == loginType).FirstOrDefault();
            else
                return new APIResult { message = "Invalid PhoneNumber Or Email!" };


            var revokeRefreshToken = _unitOfWork.refreshToken.Query().OrderByDescending(x => x.CreatedOn).FirstOrDefault();

            if (revokeRefreshToken != null)
            {
                revokeRefreshToken.IsRevoked = DateTime.UtcNow;
                _unitOfWork.refreshToken.Update(revokeRefreshToken);
                if (_unitOfWork.Complete() <= 0)
                    return new APIResult { message = "Failed To Revoke Token" };
            }
            refreshToken.UserId = user.Id;
            _unitOfWork.refreshToken.Add(refreshToken);
            _unitOfWork.Complete();
            GenerateCookie(refreshToken);

            return new APIResult { state = true, entity = refreshToken };
        }
        private RefreshTokenDevCreed GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshTokenDevCreed
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                CreatedOn = DateTime.UtcNow
            };
        }
        private string RandomString(int n)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+123456789";
            return new string(Enumerable.Repeat(chars, n).
                Select(x => x[random.Next(x.Length)]).ToArray());
        }
        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(utcExpiryDate).ToLocalTime();
            return dateTimeVal;
        }

        //private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        //{
        //    var cookieOptions = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Expires = expires
        //    };
        //}
        public async Task<APIResult> GetNewAccessToken(string RefreshToken)
        {
            try
            {
                var token = _unitOfWork.refreshToken.Query().Where(x => x.Token == RefreshToken).SingleOrDefault();
                if (token == null)
                    return new APIResult { message = "RefreshToken Is NotExist" };

                if (!token.IsActive)
                    return new APIResult { message = "RefreshToken Is NotActive" };

                User user = _userManager.Users.Where(x => x.RefreshTokens.Any(x => x.Token == RefreshToken)).SingleOrDefault();

                var accessToken = (await CreateJWTToken(user));
                var WrittenToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                token.IsRevoked = DateTime.UtcNow;

                var refreshTokenResult = await AddRefreshToken(user.Email);
                var refreshToken = (RefreshTokenDevCreed)refreshTokenResult.entity!;

                if (!refreshTokenResult.state)
                    return refreshTokenResult;

                //user.RefreshTokens!.Add(refreshToken);

                //await _userManager.UpdateAsync(user);

                //_unitOfWork.refreshToken.Update(token);

                //if (_unitOfWork.Complete() <= 0)
                //    return new APIResult { message = "Failed To Revoke Token" };

                return new APIResult { state = true, entity = new AccessAndRefreshToken { AccessToken = WrittenToken, RefreshToken = refreshToken.Token } };

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return new APIResult { message = ex.InnerException.Message };
                return new APIResult { message = ex.Message };

            }
        }
        public async Task<APIResult> accessCookie()
        {
            var cookie = _context.HttpContext!.Request.Cookies["RefreshTokenCookie"];
            return new APIResult { state = true, entity = cookie };
        }
    }
}
