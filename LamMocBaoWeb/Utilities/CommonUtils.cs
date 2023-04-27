using Microsoft.AspNetCore.Http;
using System;

namespace LamMocBaoWeb.Utilities
{
    public static class CommonUtils
    {

        public static decimal? ToDecimal(this string src)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return null;
            }
            foreach (var item in new string[] { ".", "," })
            {

                src = src.Replace(item, "");
            }

            return Convert.ToDecimal(src);
        }

        public static string GetUserIp(this HttpRequest request)
        {
            var ip = request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); ;
            if (ip.Contains(':'))
            {
                var index = ip.IndexOf(':');
                ip = ip.Substring(0, index);
            }
            return ip;
        }
    }
}
