using BusinessLogic.Services.Auth;
using Core.Auth;
using Core.Interfaces;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CustomAuthorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class AuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private  TokenValidationParameters _tokenValidationParams;
        public AuthorizationAttribute(string roles)
        {

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _tokenValidationParams = context.HttpContext.RequestServices.GetService<TokenValidationParameters>();

            //string result = context.Request.Headers["Authorization"];
            string result = context.HttpContext.Request.Headers["Authorization"];
            if (result != null)
            {

                var type = result.Split(" ")[0];
                if (type != "Bearer")
                {
                    context.HttpContext.Items["AuthorizationState"] = "Unauthorized";
                    var responseObj = new APIResult();
                    responseObj.message = "Unauthorized";
                    var response = new ObjectResult(responseObj)
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
                    context.Result = response;
                }
                else
                {
                    //Check Time
                    #region CheckTime
                    var token = result.Split(" ")[1];


                    var handler = new JwtSecurityTokenHandler();
                    var decodedValue = handler.ReadJwtToken(token);
                    var expiresValue = decodedValue.Payload.GetValueOrDefault("exp")!;
                    DateTime? expiresOn = expiresValue != null
                                        ? DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expiresValue)).LocalDateTime : DateTime.MinValue;
                    DateTime? expiresOn2 = DateTime.Now;
                    if (_tokenValidationParams?.ClockSkew != null)
                    {
                        double totalSeconds = _tokenValidationParams.ClockSkew.TotalSeconds;
                        expiresOn = expiresOn.Value.AddSeconds(totalSeconds);
                        expiresOn2 = expiresOn2.Value.AddSeconds(totalSeconds);
                    }

                    if (expiresOn < DateTime.Now)
                    {
                        context.HttpContext.Items["AuthorizationState"] = "Unauthorized";
                        var responseObj = new APIResult();
                        responseObj.message = "Unauthorized";
                        var response = new ObjectResult(responseObj)
                        {
                            StatusCode = (int) HttpStatusCode.Unauthorized
                        };
                        context.Result = response;
                        return;
                    }
                    #endregion

                    try
                    {
                        var jwtTokenHandler = new JwtSecurityTokenHandler();
                        var tokenInVerification = jwtTokenHandler.ValidateToken(result.Split(" ")[1], _tokenValidationParams, out var validatedToken);
                        context.HttpContext.User = tokenInVerification;
                        var k = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                        if (validatedToken is JwtSecurityToken jwtSecurityToken)
                        {
                            var results = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                            if (!results)
                            {
                                context.HttpContext.Items["AuthorizationState"] = "Unauthorized";
                                var response = new ObjectResult(new APIResult { message = "Token is Created by Another Algorithm" }) { StatusCode = (int) HttpStatusCode.Unauthorized};
                                context.Result = response; 
                            }
                        }


                    }
                    catch (Exception ex)
                    {

                        context.HttpContext.Items["AuthorizationState"] = "Unauthorized";
                        var response = new ObjectResult(new APIResult { message =
                            "Token is not correct or is created by another secretkey or another algorithm"
                        }) { StatusCode = (int)HttpStatusCode.Unauthorized };
                        context.Result = response;
                    }
                    return;
                }
            }

        }
    }
}
