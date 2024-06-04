using Core.Auth;
using Core.Interfaces;
using DataTransferObject.AuthDTO.Request;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IdentityProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        APIResult apiResult;
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
            apiResult = new APIResult();
        }

        [HttpGet("GetCookie")]
        public async Task<IActionResult> getCookie()
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.accessCookie());
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.Register(registerRequest));
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.Login(loginRequest));
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.GetRoles());
        }

        [HttpPost("AddNewRole")]
        public async Task<IActionResult> AddRole(RoleDTO roleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.AddRole(roleDTO));
        }

        [HttpPost("AssignRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(AddRoleToUserRequest addRoleToUserRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.AddRoleToUser(addRoleToUserRequest));
        }

        [HttpPost("AddClaimToUser")]
        public async Task<IActionResult> AddClaimToUser(AddClaimToUserRequest addClaimRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.AddClaimsToUser(addClaimRequest));
        }
        [HttpPost("AddClaimToRole")]
        public async Task<IActionResult> AddClaimToRole(AddClaimToRoleRequest addClaimToRoleRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.AddClaimsToRole(addClaimToRoleRequest));
        }

        [HttpPost("SeedClaimsForRole")]
        public async Task<IActionResult> SeedClaimsForRole(RoleDTO roleDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.SeedClaimsForRole(roleDTO));
        }


        [HttpGet("GetUsersWithAssignedRoles")]
        public async Task<IActionResult> GetUsersWithAssignedRoles()
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.GetUsersWithAssignedRoles());

        }

        [HttpPost("CheckInRole")]
        public async Task<IActionResult> CheckInRole(EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.CheckInRole(emailRequest));
        }

        [HttpPost("AddUserByAdmin")]
        public async Task<IActionResult> AddUserByAdmin(AddUserByAdminRequest addUserByAdminRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.AddUserByAdmin(addUserByAdminRequest));
        }


        [HttpPut("EditUserByAdmin")]
        public async Task<IActionResult> EditUserByAdmin(EditUserByAdminRequest editUserByAdminRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.EditUserByAdmin(editUserByAdminRequest));
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            return Ok(await _authService.DeleteUser(Id));

        }

        [HttpPost("GetNewAccessToken")]
        public async Task<IActionResult> GetNewAccessToken()
        {
            if (!ModelState.IsValid)
                return BadRequest(new APIResult { message = ModelState.ToString() });

            var refreshToken = Request.Cookies["RefreshTokenCookie"];
            if (refreshToken == null)
                return BadRequest(new APIResult { message = "RefreshToken Doesn't Exist in  Cookie"});

            //var dataResult = JsonConvert.DeserializeObject<RefreshTokenDevCreed>(refreshToken);

            return Ok(await _authService.GetNewAccessToken(refreshToken));
        }

        [Authorize(Policy = "AdminCantEditHimSelf")]
        [HttpGet("EditUser")]
        public IActionResult EditUser([FromQuery]Guid userId)
        {
             int x = 0;
            return Ok();

        }

     

    }
}
