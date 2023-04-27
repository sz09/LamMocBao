using LamMocBaoWeb.Controllers.Public;
using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Models.Identify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace LamMocBaoWeb.Attributes
{
    public class LMBForwarderAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly List<string> AuthorizedControllers = new List<string>
        {
            "Cart"
        };
        private readonly List<string> AuthorizingEndpoints = new List<string>
        {
            "Cart/ConsiderLogin"
        };

        private readonly Dictionary<string, string> Forwarders = new Dictionary<string, string>
        {
            {  "Cart/ConsiderLogin", "/gio-hang"}
        };

        private readonly KeyValuePair<string, string> LoginEndpoint = new KeyValuePair<string, string>("Cart/ConsiderLogin",  "/gio-hang/login");

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var controller = context.ActionDescriptor.RouteValues["controller"];
            var route = string.Join("/", context.ActionDescriptor.RouteValues.OrderByDescending(d => d.Key).Select(d => d.Value));
            if (!await IsAuthorized(context))
            {
                context.Result = new RedirectResult(LoginEndpoint.Value);
                context.HttpContext.Response.Cookies.Append("redirectUrl", context.HttpContext.Request.Path);
            }
        }

        private async Task<bool> IsAuthorized(AuthorizationFilterContext context)
        {
            var cookies = context.HttpContext.Request.Cookies;
            if (!cookies.ContainsKey(Constants.UserToken_Key))
            {
                return false;
            }
            var userToken = cookies[Constants.UserToken_Key];
            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new UnauthorizedResult();
                return false;
            }
            var jwtSecurityToken = JwtUtilities.ReadJwtSecurityToken(userToken);
            if (jwtSecurityToken == null)
            {
                context.Result = new UnauthorizedResult();
                return false;
            }
            var userClaim = jwtSecurityToken.Claims.First(d => d.Type == Constants.UserId_Key);
            var userId = userClaim.Value;
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var user = await userManager.FindByIdAsync(userId); 
            if (user == null || user.IsDeleted)
            {
                context.Result = new UnauthorizedResult();
                return false;
            }

            var expireTime = jwtSecurityToken.Claims.First(d => d.Type == "exp");
            if (DateTime.UtcNow > jwtSecurityToken.ValidTo)
            {
                context.Result = new UnauthorizedResult();
                return false;
            }

            return true;
        }
    }

}
