using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Services.Identify;
using Shared.Models.Identify;
using System;
using System.Linq;
using System.Security.Claims;

namespace LamMocBaoWeb.Utilities
{
    public static class LMBClaimType
    {
        public const string UserToken = "UserToken";
        public const string UserId = "UserId";
        public const string UserEmail = "UserEmail";
    }

    public static class ClaimsIdentityExtensions
    {
        public static Guid GetUserToken(this HttpContext httpContext)
        {
            return httpContext.GetClaimValue<Guid>(LMBClaimType.UserToken);
        }

        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetClaimValue<Guid>(LMBClaimType.UserId);
        }

        public static string GetUserEmail(this ClaimsPrincipal claimsPrincipal)
        {
            //Do not use: GetClaimValue method for string value
            return claimsPrincipal.GetClaimValueAsString(LMBClaimType.UserEmail);
        }

        public static T GetClaimValue<T>(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var userIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            return GetClaimValue<T>(userIdentity, claimType);
        }

        public static T GetClaimValue<T>(this HttpContext httpContext, string claimType)
        {
            var userIdentity = httpContext.GetUserIdentity();
            return GetClaimValue<T>(userIdentity, claimType);
        }

        public static T GetClaimValue<T>(this HttpRequest httpRequest, string claimType)
        {
            return GetClaimValue<T>(httpRequest.HttpContext, claimType);
        }

        private static T GetClaimValue<T>(ClaimsIdentity userIdentity, string claimType)
        {
            var value = userIdentity.FindFirstOrEmpty(claimType);

            if (typeof(T).IsValueType)
            {
                return DataConverter.Instance.To<T>(value);
            }
            else
            {
                return JsonSerialization.Deserialize<T>(value);
            }
        }

        private static string GetClaimValueAsString(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var userIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            var value = userIdentity.FindFirstOrEmpty(claimType);
            return value;
        }

        public static ClaimsIdentity GetUserIdentity(this HttpContext httpContext)
        {
            return (ClaimsIdentity)httpContext.User.Identity;
        }

        public static string FindFirstOrEmpty(this ClaimsIdentity identity, string claimType)
        {
            var value = identity.Claims.Where(e => e.Type == claimType).Select(e => e.Value).FirstOrDefault();
            return string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        public static IdentityBuilder RegisterDBStores(this IdentityBuilder builder)
        {
            builder.Services.AddScoped<IUserStore<User>, UserStore<User>>();
            builder.Services.AddScoped<IRoleStore<Role>, RoleStore<Role>>();
            return builder;
        }
    }
}
