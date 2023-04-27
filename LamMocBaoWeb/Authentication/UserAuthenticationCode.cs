using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace LamMocBaoWeb.Authentication
{
    public class UserAuthenticationCode
    {
        public string Value { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// Temporary create this object in here to reuse the logic in this class. We should remove it when refactor again
        /// </summary>
        /// <returns></returns>
        public static UserAuthenticationCode GetFrom(HttpRequest request, ILogger logger)
        {
            UserAuthenticationCode authenticationCode = null;
            try
            {
                authenticationCode = request.GetClaimValue<UserAuthenticationCode>(LMBClaimType.UserToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            if (authenticationCode == null || authenticationCode.UserId == Guid.Empty)
            {
                var userGuid = request.GetClaimValue<Guid>(LMBClaimType.UserId);
                authenticationCode = new UserAuthenticationCode() { UserId = userGuid };
            }

            return authenticationCode;
        }
    }
}
