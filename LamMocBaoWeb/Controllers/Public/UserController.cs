using AutoMapper;
using LamMocBaoWeb.Authentication;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Identify;
using Services.Services.Interfaces;
using Shared;
using Shared.Models;
using Shared.Models.Identify;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers
{
    [Route("users")]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IProductImageService _productImageService;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IUserService _userService;
        private readonly IUserStore<User> _userStore;
        private readonly IRoleStore<Role> _roleStore;
        private readonly string RedirectUrl = "redirectUrl";
        public UserController(IProductService productService, IMapper mapper, IUserService userService,
            IUserStore<User> userStore, IRoleStore<Role> roleStore,
            IProductImageService productImageService,
            IProductTypeService productTypeService, ISystemSettingService systemSettingService, 
            IAuthenticationHelper authenticationHelper) : base(mapper)
        {
            _productService = productService;
            _mapper = mapper;
            _userService = userService;
            _userStore = userStore;
            _roleStore = roleStore;
            _productImageService = productImageService;
            _productTypeService = productTypeService;
            _systemSettingService = systemSettingService;
            _authenticationHelper = authenticationHelper;
        }

        [Route("dang-ky")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("../Cart/Public/ConsiderLogin");
            }

            if (model.Password != model.RetypePassword)
            {
                ModelState.TryAddModelError("Register_PasswordMismatch", Resources.Resource.Register_RetypePassword_Mismatch);
                return View("../Cart/Public/ConsiderLogin");
            }

            if (await _userStore.FindByNameAsync(model.Username, default) != null)
            {
                ModelState.TryAddModelError("Register_Exist_Username", Resources.Resource.Register_Username_Is_Existing);
                return View("../Cart/Public/ConsiderLogin");
            }

            var user = new User
            {
                Email = model.Email,
                Username = model.Username,
                PasswordHash = MD5Utilities.Hash(model.Password),
                Type = AccountType.Customer
            };

            await _userStore.CreateAsync(user, default);
            return await LogOn(new LoginModel { Username = model.Username, Password = model.Password });
        }

        [Route("dang-ky")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("Public/Register");
        }

        [Route("login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Public/Login");
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

            string previousRequestedUrl = HttpContext.Request.Cookies.ContainsKey(RedirectUrl) ? HttpContext.Request.Cookies[RedirectUrl].ToString() : string.Empty;

            return await PerformLogin(model, previousRequestedUrl);
        }

        private async Task<ActionResult> PerformLogin(LoginModel model, string previousRequestedUrl)
        {
            var isAuthorized = _userService.AuthorizeCustomer(model.Username, model.Password, out User user);
            if (isAuthorized)
            {
                var token = await _authenticationHelper.ProcessLoginAsync(user, HttpContext, model.RememberMe);
                HttpContext.Response.Cookies.Append(Constants.UserToken_Key, token);
                if (string.IsNullOrEmpty(previousRequestedUrl))
                {
                    return View("../Cart/Public/Index");
                }

                HttpContext.Response.Cookies.Delete(RedirectUrl);
                return Redirect(previousRequestedUrl);
            }

            ModelState.TryAddModelError("LogOn_Unauthorized", Resources.Resource.Login_Username_Password_Mismatch);
            return View(model);
        }
    }
}
