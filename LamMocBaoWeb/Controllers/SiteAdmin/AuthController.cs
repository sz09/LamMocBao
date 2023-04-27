using AutoMapper;
using LamMocBaoWeb.Authentication;
using LamMocBaoWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared;
using Shared.Models.Identify;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationHelper _authenticationHelper;
        public AuthController(IUserService userService, IAuthenticationHelper authenticationHelper, IMapper mapper): base(mapper)
        {
            _userService = userService;
            _authenticationHelper = authenticationHelper;
        }

        #region Login
        public IActionResult LogOn()
        {
            return View(new LoginModel {  });
        }

        public async Task<IActionResult> LogOff()
        {
            await _authenticationHelper.ProcessLogoffAsync(HttpContext);
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult> LogOn(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("LogOn", new LoginModel { Username = model.Username });
            }

            string previousRequestedUrl = HttpContext.Request.Query.ContainsKey("redirectUrl") ? HttpContext.Request.Query["redirectUrl"].ToString() : string.Empty;

            return await PerformLogin(model, previousRequestedUrl);
        }

        private async Task<ActionResult> PerformLogin(LoginModel model, string previousRequestedUrl)
        {
            ClearAuthenticationCookie();

            var isAuthorized = _userService.AuthorizeAdminUser(model.Username, model.Password, out User user);
            if (isAuthorized)
            {
                var token = await _authenticationHelper.ProcessLoginAsync(user, HttpContext, true /*model.RememberMe*/);
                HttpContext.Response.Cookies.Append(Constants.UserToken_Key, token);
                var redirectUrl = GetAppropriateRedirectUrlForUser(previousRequestedUrl, model.HashValue);
                return redirectUrl;
            }

            ModelState.TryAddModelError("Unauthorized", Resources.Resource.Login_Username_Password_Mismatch);
            return View(model);
        }

        private void ClearAuthenticationCookie()
        {
            //// clear all authetication cookie
            //foreach (var region in _regionOptions.JobLogicRegions)
            //{
            //    Response.Cookies.Delete($"{MultiRegionConstants.AuthenticationCookieName}.{region.ToLower()}");
            //}
        }

        private ActionResult GetAppropriateRedirectUrlForUser(string previousRequestedUrl = null, string hashValue = null)
        {
            if (!string.IsNullOrEmpty(previousRequestedUrl))
            {
                return Redirect(GenerateUrl(previousRequestedUrl, hashValue));
            }

            return Redirect("/admin/san-pham");
        }

        private string GenerateUrl(string previousRequestedUrl, string hashValue)
        {
            if (previousRequestedUrl.ToLower().StartsWith("/us/") || previousRequestedUrl.ToLower().StartsWith("/uk/"))
                previousRequestedUrl = previousRequestedUrl.Remove(0, 3);
            return $"{previousRequestedUrl}{(string.IsNullOrWhiteSpace(hashValue) ? string.Empty : $"#{hashValue}")}";
        }
        #endregion
    }
}
