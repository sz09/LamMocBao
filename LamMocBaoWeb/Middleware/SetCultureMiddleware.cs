using Microsoft.AspNetCore.Http;
using Services.Services;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Middleware
{
    public class SetCultureMiddleware
    {
        private readonly RequestDelegate _next; 
        private readonly IServiceConfig _serviceConfig;
        public SetCultureMiddleware(RequestDelegate next, IServiceConfig serviceConfig)
        {
            _next = next;
            _serviceConfig = serviceConfig;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            CultureInfo ci = new CultureInfo("vi-VN", true);
            ci.NumberFormat.CurrencyDecimalDigits = 0;
            if (!string.IsNullOrEmpty(_serviceConfig.Currency_Symbol))
            {
                ci.NumberFormat.CurrencySymbol = _serviceConfig.Currency_Symbol;
            }
            else
            {
                _serviceConfig.Currency_Symbol = ci.NumberFormat.CurrencySymbol;
            }

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
