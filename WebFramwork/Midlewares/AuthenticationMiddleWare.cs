using AutoMapper;
using Common.Api;
using Common.Exceptions;
using Data.Context;
using Data.Contract;
using Data.DTOs.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebFramework.Midlewares
{
    public class AuthenticationMiddleWare
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var db = context.RequestServices.GetRequiredService<AppDbContext>();
            var projectRepository = context.RequestServices.GetRequiredService<IPermissionRepository>();
            var claimTypesRolesRepository = context.RequestServices.GetRequiredService<IPermissionRoleRepository>();
            var userRepository = context.RequestServices.GetRequiredService<IUserRepository>();
            var roleRepository = context.RequestServices.GetRequiredService<IRoleRepository>();
            var userRoleRepository = context.RequestServices.GetRequiredService<IUserRoleRepository>();
            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleIdClaim = context.User.FindFirstValue(ClaimTypes.Role);
            string UserSecurityStamp = context.User.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
            string RoleSecurityStamp = context.User.FindFirstValue("RoleSecurityStamp");
            

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            string token = null;
            long userId = !string.IsNullOrEmpty(userIdClaim) ? long.Parse(userIdClaim) : 0;
            long roleId = !string.IsNullOrEmpty(roleIdClaim) ? long.Parse(roleIdClaim) : 0;

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (!string.IsNullOrEmpty(token))
            {
                context.Items["Token"] = token;

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                //var validate = handler.ValidateToken(token,);

                userId = userId == null || userId == 0 ?
                    long.Parse(jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value.ToString()) : userId;
                roleId = roleId == null || roleId == 0 ?
                    long.Parse(jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToString()) : roleId;
                UserSecurityStamp = string.IsNullOrEmpty(UserSecurityStamp) ?
                    jwt.Claims.FirstOrDefault(c => c.Type == new ClaimsIdentityOptions().SecurityStampClaimType)?.Value.ToString() : UserSecurityStamp;
                RoleSecurityStamp = string.IsNullOrEmpty(RoleSecurityStamp) ?
                    jwt.Claims.FirstOrDefault(c => c.Type == "RoleSecurityStamp")?.Value.ToString() : RoleSecurityStamp;



                context.Items[ClaimTypes.NameIdentifier] = userId;
                context.Items[ClaimTypes.Role] = roleId;
            }



            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleIdClaim))
            {
                //await _next(context);
                //return;
            }
            else
            {
                userId = long.Parse(userIdClaim);
                roleId = long.Parse(roleIdClaim);
            }





            bool hasAccess = await ConfirmUriByClaimTypeRole(context, userId, roleId,
                userRoleRepository, claimTypesRolesRepository, projectRepository,
                userRepository, roleRepository, UserSecurityStamp, RoleSecurityStamp);
            if (!hasAccess)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                //await context.Response.WriteAsync("Access Denied");
                throw new AppException(ApiResultStatusCode.UnAuthentication,
                    "شما به این بخش دسترسی ندارید.");
            }

            await _next(context);
        }

        private async Task<bool> ConfirmUriByClaimTypeRole(HttpContext hContext,
            long userId, long roleId, IUserRoleRepository userRoleRepository,
            IPermissionRoleRepository permissionRoleRepository,
            IPermissionRepository projectRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            string UserSecurityStamp, string RoleSecurityStamp)
        {
            //var userRoles = await userRoleRepository.GetByUserId(userId);
            var claimTypes = new List<PermissionDto>();
            var allClaimTypes = await projectRepository.TableNoTracking.ToListAsync();


            //foreach (var role in userRoles.Select(c => c.Role))
            //{
            var claims = await permissionRoleRepository.GetUriByRoleId(roleId);
            if (claims.Any())
            {
                claimTypes.AddRange(claims.Select(c => new PermissionDto
                {
                    Id = c.Id,
                    AreaName = c.AreaName,
                    ControllerName = c.ControllerName,
                    ActionName = c.ActionName,
                    NeedAccess = c.NeedAccess
                }));
            }
            //}

            //if (!claimTypes.Any())
            //    return false;

            var area = hContext.Request.RouteValues["area"]?.ToString() ?? "NoArea";
            var controller = hContext.Request.RouteValues["controller"]?.ToString() ?? "";
            var action = hContext.Request.RouteValues["action"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(controller))
            {
                return true;
            }

            var Claim = claimTypes.FirstOrDefault(c =>
                c.AreaName.Equals(area, StringComparison.OrdinalIgnoreCase) &&
                c.ControllerName.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
                c.ActionName.Equals(action, StringComparison.OrdinalIgnoreCase));
            if ((Claim != null && Claim.NeedAccess.Value &&
                (await userRepository.CheckSecurityStamp(userId, UserSecurityStamp)) &&
                (await roleRepository.CheckSecurityStamp(roleId, RoleSecurityStamp))) ||
                allClaimTypes.Any(c => c.AreaName.Equals(area, StringComparison.OrdinalIgnoreCase) &&
                c.ControllerName.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
                c.ActionName.Equals(action, StringComparison.OrdinalIgnoreCase) &&
                !c.NeedAccess.Value))
                return true;
            else
                return false;

        }
    }
}
