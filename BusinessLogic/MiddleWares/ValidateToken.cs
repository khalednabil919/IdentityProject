using BusinessLogic.Services.Auth;
using Core.Auth;
using Core.Interfaces;
using DataTransferObject.AuthDTO.Request;
using DataTransferObject.AuthDTO.Response;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.MiddleWares
{
    public class ValidateToken : IMiddleware
    {
        private readonly TokenValidationParameters _tokenValidationParams;
        public ValidateToken(TokenValidationParameters tokenValidationParameters)
        {
            _tokenValidationParams = tokenValidationParameters;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            //string result = context.Request.Headers["Authorization"];
            string result = context.Request.Headers["Authorization"];
            if (result != null)
            {
                 
                var type = result.Split(" ")[0] ;
                if (type != "Bearer")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    string jsonString = JsonConvert.SerializeObject(new APIResult { message = "Schema Should Be Bearer Only" });
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                    return;
                }
                else
                {
                    //Check Time
                    #region CheckTime
                    var token = result.Split(" ")[1];

                    //var jwtTokenHandler = new JwtSecurityTokenHandler();
                    //var tokenInVerification = jwtTokenHandler.ValidateToken(token, _tokenValidationParams, out var validatedToken);
                    //var anotherWayToAccessClaims = tokenInVerification.FindFirst(x=>x.Type == JwtRegisteredClaimNames.Email)

                    var handler = new JwtSecurityTokenHandler();
                    var decodedValue = handler.ReadJwtToken(token);
                    var expiresValue = decodedValue.Payload.GetValueOrDefault("exp")!;
                    DateTime? expiresOn = expiresValue != null
                                        ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expiresValue)).LocalDateTime : DateTime.MinValue;
                    DateTime? expiresOn2 = DateTime.Now;
                    //var validTo = decodedValue.ValidTo;
                    if (_tokenValidationParams?.ClockSkew != null)
                    {
                        double totalSeconds = _tokenValidationParams.ClockSkew.TotalSeconds;
                        expiresOn = expiresOn.Value.AddSeconds(totalSeconds);
                        expiresOn2 = expiresOn2.Value.AddSeconds(totalSeconds);
                    }

                    if (expiresOn < DateTime.Now)
                    {
                        //var cookie = context.Request.Cookies.FirstOrDefault(x => x.Key == "RefreshTokenCookie").Value;
                        //var refreshToken = context.Request.Cookies["RefreshTokenCookie"];
                        //var dataResult = JsonConvert.DeserializeObject<RefreshTokenDevCreed>(refreshToken);
                        //if (!dataResult.IsActive)
                        //{
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            string jsonString = JsonConvert.SerializeObject(new APIResult { message = "Time of AccessToken has Expired" });
                            await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                            return;
                        //}
                        //var tokenResult = await _authService.GetNewAccessToken(dataResult.Token);
                        //var accessToken = ((AccessAndRefreshToken)tokenResult.entity!).AccessToken;
                        //context.Request.Headers["Authorization"] = $"Bearer {accessToken}";
                        //result = $"Bearer {accessToken}";
                    }
                    #endregion

                    try
                    {
                        var jwtTokenHandler = new JwtSecurityTokenHandler();
                        var tokenInVerification = jwtTokenHandler.ValidateToken(result.Split(" ")[1], _tokenValidationParams, out var validatedToken);
                        context.User = tokenInVerification;
                        var k = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                        if (validatedToken is JwtSecurityToken jwtSecurityToken)
                        {

                            var results = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                            if (!results)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                string jsonString = JsonConvert.SerializeObject(new APIResult { message = "Token is Created by Another Algorithm" });
                                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        string jsonString = JsonConvert.SerializeObject(new APIResult { message = "Token is not correct or is created by another secretkey or another algorithm" });
                        await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                        return;
                    }



                }
            }

            await next(context);
        }

    }
}
