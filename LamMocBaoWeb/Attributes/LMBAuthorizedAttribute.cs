using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Models.Identify;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace LamMocBaoWeb.Attributes
{
    public class LMBAuthorizedAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public LMBAuthorizedAttribute() { }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var cookies = context.HttpContext.Request.Cookies;
            if(!cookies.ContainsKey(Constants.UserToken_Key))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userToken = cookies[Constants.UserToken_Key];
            if(string.IsNullOrEmpty(userToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var jwtSecurityToken = JwtUtilities.ReadJwtSecurityToken(userToken);
            if (jwtSecurityToken == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var userClaim = jwtSecurityToken.Claims.First(d => d.Type == Constants.UserId_Key);
            var userId = userClaim.Value; 
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var expireTime = jwtSecurityToken.Claims.First(d => d.Type == "exp");
            if (DateTime.UtcNow > jwtSecurityToken.ValidTo)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }

}
