using DataTransferObject.AuthDTO.Request;
using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        public Task<APIResult> accessCookie();
        public Task<APIResult> Register(RegisterRequest registerRequest);
        public Task<APIResult> Login(LoginRequest loginRequest);
        public Task<APIResult> GetRoles();
        public Task<APIResult> GetNewAccessToken(string RefreshToken);

        public Task<APIResult> AddClaimsToUser(AddClaimToUserRequest addClaimRequest);
        public Task<APIResult> AddRole(RoleDTO roleDTO);
        public Task<APIResult> AddRoleToUser(AddRoleToUserRequest addRoleToUserRequest);
        public Task<APIResult> AddClaimsToRole(AddClaimToRoleRequest addClaimToRoleRequest);
        public Task<APIResult> GenerateAccessTokenDueToRefreshToken(string RefreshToken);
        public Task<APIResult> SeedClaimsForRole(RoleDTO roleDTO);
        public Task<APIResult> GetUsersWithAssignedRoles();
        public Task<APIResult> CheckInRole(EmailRequest email);
        public Task<APIResult> AddUserByAdmin(AddUserByAdminRequest addUserByAdminRequest);
        public Task<APIResult> EditUserByAdmin(EditUserByAdminRequest editUserByAdminRequest);
        public Task<APIResult> DeleteUser(string Id);
        //public Task<APIResult> VerifyandValidateToken(TokenRequest tokenRequest);
    }
}
