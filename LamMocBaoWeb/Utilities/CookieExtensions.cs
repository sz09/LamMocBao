using LamMocBaoWeb.Models;
using Microsoft.AspNetCore.Http;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LamMocBaoWeb.Utilities
{
    public static class CookieExtensions
    {
        //public static IEnumerable<KeyValuePair<Guid, int>> GetFullInfoProductCarts(this IRequestCookieCollection requestCookie)
        //{
        //    return requestCookie.Where(d => d.Key.StartsWith(Constant.Cookie_CartKey))
        //                        .GroupBy(d => d.Key)
        //                        .Select(d => {
        //                            var productId = d.Key.Replace($"{Constant.Cookie_CartKey}_", string.Empty);
        //                            Guid.TryParse(productId, out Guid guid);
        //                            return KeyValuePair.Create(guid, d.Sum(e => int.Parse(e.Value)));
        //                        });
        //}
        
        public static Dictionary<Guid, List<ProductCartTempCount>> GetFullInfoProductCarts(this IRequestCookieCollection requestCookie)
        {
            return requestCookie.Where(d => d.Key.StartsWith(Constant.Cookie_CartKey))
                                .GroupBy(d => d.Key, d => d.Value)
                                .Select(d => {
                                    var productId = d.Key.Replace($"{Constant.Cookie_CartKey}_", string.Empty);
                                    Guid.TryParse(productId, out Guid guid);

                                    var items = d.SelectMany(s => JsonSerialization.Deserialize<List<ProductCartTempCount>>(s)).ToList();

                                    return KeyValuePair.Create(guid, items);
                                })
                                .ToDictionary(d => d.Key, e => e.Value);
        }

        public static List<string> GetCartCookies(this IRequestCookieCollection requestCookie)
        {
            return requestCookie.Where(d => d.Key.StartsWith(Constant.Cookie_CartKey)).Select(d => d.Key).ToList();
        }
    }
}
