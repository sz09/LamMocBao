using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Services.Services;
using Services.Services.Interfaces;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Middleware
{
    public class GrantSystemSettingMiddleware
    {
        private readonly RequestDelegate _next; 
        private readonly IServiceConfig _serviceConfig;
        public GrantSystemSettingMiddleware(RequestDelegate next, IServiceConfig serviceConfig)
        {
            _next = next;
            _serviceConfig = serviceConfig;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var _systemSettingService = context.RequestServices.GetService(typeof(ISystemSettingService)) as ISystemSettingService;
            var systemSetting = await _systemSettingService.LoadAsync();
            if (systemSetting != null)
            {
                if (_serviceConfig.ContactInfos == null)
                {
                    _serviceConfig.ContactInfos = new ContactInfos();
                }

                _serviceConfig.ContactInfos.ContactAddress = systemSetting.ContactAddress;
                _serviceConfig.ContactInfos.ContactPhoneNumbers = systemSetting.ContactPhoneNumbers;
                _serviceConfig.ContactInfos.Email = systemSetting.Email;
                _serviceConfig.ContactInfos.WorkingTime = systemSetting.WorkingTime;
                _serviceConfig.ContactInfos.GoogleMapFrameUrl = systemSetting.GoogleMapFrameUrl;
                _serviceConfig.ContactInfos.Facebook = systemSetting.Facebook;
                _serviceConfig.ContactInfos.Instagram = systemSetting.Instagram;
                _serviceConfig.ContactInfos.Youtube = systemSetting.Youtube;
                _serviceConfig.NumberOfItemsOnTrend = systemSetting.NumberOfHighlightItems;
                _serviceConfig.AutoHighlightItemsInDays = systemSetting.HighlightItemsInDays;

                _serviceConfig.PaymentInfos = new PaymentInfos
                {
                    BankAccount = systemSetting.BankAccount,
                    BankName = systemSetting.BankName,
                    CardHolderName = systemSetting.CardHolderName
                };
            }

            if (_serviceConfig.ShowOnHomepageFengshuis == null || !_serviceConfig.ShowOnHomepageFengshuis.Any())
            {
                var categoryService = context.RequestServices.GetService(typeof(ICategoryService)) as ICategoryService;
                _serviceConfig.ShowOnHomepageFengshuis = await categoryService.GetShowOnHomepageFengshuisAsync();
            }
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }

    public class SetCommonSettingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceConfig _serviceConfig;
        public SetCommonSettingMiddleware(RequestDelegate next, IServiceConfig serviceConfig)
        {
            _next = next;
            _serviceConfig = serviceConfig;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrEmpty(_serviceConfig.PingEndpoint))
            {
                _serviceConfig.PingEndpoint = $"{context.Request.Scheme}://{context.Request.Host.Value}/ping";
            }

            if (string.IsNullOrEmpty(_serviceConfig.HostWebRootPath))
            {
                var webHostEnvironment = context.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

                _serviceConfig.HostWebRootPath = $"{webHostEnvironment.WebRootPath}";
            }

            await _next(context);
        }
    }
}
