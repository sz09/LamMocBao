using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Services.Services;
using Shared;
using Shared.Models.Identify;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Authentication
{
    public interface IAuthenticationHelper
    {
        Task<string> ProcessLoginAsync(User model, HttpContext httpContext, bool isPersistent);
        Task ProcessLogoffAsync(HttpContext httpContext);
    }
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IServiceConfig _serviceConfig;
        public AuthenticationHelper(SignInManager<User> signInManager, IServiceConfig serviceConfig)
        {
            _signInManager = signInManager;
            _serviceConfig = serviceConfig;
        }

        public async Task<string> ProcessLoginAsync(User model, HttpContext httpContext, bool isPersistent)
        {
            await _signInManager.SignInAsync(model, new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTime.UtcNow.AddDays(_serviceConfig.LMB.ExpireAfterDays) : DateTime.UtcNow.AddHours(_serviceConfig.LMB.ExpireAfterHours)
            });

            return GenerateJwtToken(model);
        }

        public async Task ProcessLogoffAsync(HttpContext httpContext)
        {
            await _signInManager.SignOutAsync();
            httpContext.Response.Cookies.Delete(Constants.UserToken_Key);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(Constants.UserId_Key, user.Id.ToString()),
                new Claim(Constants.Username_Key, user.Username),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_serviceConfig.LMB.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(_serviceConfig.LMB.ExpireAfterDays);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
