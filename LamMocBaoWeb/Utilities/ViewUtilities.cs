using LamMocBaoWeb.Resources;
using LamMocBaoWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace LamMocBaoWeb.Utilities
{
    public static class ViewUtilities
    {
        public static string Display<T, TResult>(this IEnumerable<T> src, Func<T, TResult> selector, string separator = " | ", int take = 3)
        {
            if (src == null) return string.Empty;
            var first = string.Join(separator, src.Take(take).Select(selector));
            var last = src.Count() > take ? "...": string.Empty;
            return first + last;
        }

        public static string DisplayConcurency(this decimal src, string symbol)
        {
            return $"{src} {symbol}";
        }

        public static string ToTel(this string phoneNumbers, string splitter = "-")
        {
            if (string.IsNullOrEmpty(phoneNumbers))
            {
                return string.Empty;
            }
            var array = phoneNumbers.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(splitter, array.Select(d => $"<a href='tel: {d}'>{d}</a>"));
        }

        public static string ToCultureCurrency(this decimal amount)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var numberFormat = cultureInfo.NumberFormat;
            string pattern = null;
            switch (numberFormat.CurrencyPositivePattern)
            {
                case 0:
                    pattern = "{0}{1:N" + numberFormat.CurrencyDecimalDigits + "}";
                    break;
                case 1:
                    pattern = "{1:N" + numberFormat.CurrencyDecimalDigits + "}{0}";
                    break;
                case 2:
                    pattern = "{0} {1:N" + numberFormat.CurrencyDecimalDigits + "}";
                    break;
                case 3:
                    pattern = "{1:N" + numberFormat.CurrencyDecimalDigits + "} {0}";
                    break;
            }

            return string.Format(cultureInfo, pattern, numberFormat.CurrencySymbol, amount);
        }

        public static string GetCustomerContactInfos(this Shared.Models.Customer customer, bool isAppenGoogleMap = false)
        {
            if (customer == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='customer-grid-contact'>");
            var address = $"{customer.Address}, {customer.Ward}, {customer.District}, {customer.Province}";
            stringBuilder.AppendLine($"<p>{Resource.Customer_Address}: {address} </p>");
            stringBuilder.AppendLine($"<p>{Resource.Customer_Phone}: {customer.PhoneNumber}</p>");
            stringBuilder.AppendLine($"<p>{Resource.Customer_Email}: {customer.Email}</p>");
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }

        public static string GetCustomerContactInfos(this Shared.Models.Appointment customer, bool isAppenGoogleMap = false)
        {
            if (customer == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='customer-grid-contact'>");
            var address = $"{customer.Address}, {customer.Ward}, {customer.District}, {customer.Province}";
            stringBuilder.AppendLine($"<p>{Resource.Customer_Address}: {address} </p>");
            stringBuilder.AppendLine($"<p>{Resource.Customer_Phone}: {customer.PhoneNumber}</p>");
            stringBuilder.AppendLine($"<p>{Resource.Customer_Email}: {customer.Email}</p>");
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }

        public static string GetCustomerContactInfos(this ViewOrderViewModel order, bool isAppenGoogleMap = false)
        {
            if (order == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='customer-grid-contact'>");
            var address = $"{order.Address}, {order.Ward}, {order.District}, {order.Province}";
            var ggaddress = "";
            if (isAppenGoogleMap)
            {
                var @string = $"{order.Address} {order.Ward} {order.District} {order.Province}";
                ggaddress = $"href='https://www.google.com/maps/place/{RemoveSpecialText(@string, "+")}' target='_blank'";
            }
            stringBuilder.AppendLine($"<label class='form-control f-content'>{Resource.Customer_Address}: <a {ggaddress}> {address} </a> </label>");
            stringBuilder.AppendLine($"<label class='form-control f-content'>{Resource.Customer_Phone}: {order.Customer.PhoneNumber.ToTel()}</label>");
            if (!string.IsNullOrEmpty(order.Note))
            {
                stringBuilder.AppendLine($"<label class='form-control f-content'>{Resource.Order_Note}: {order.Note}</label>");
            }
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }

        public static string GetAddresssInfos(this ViewAppointmentViewModel model, bool isAppenGoogleMap = false)
        {
            if (model == null)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='customer-grid-contact'>");
            var address = $"{model.Address}, {model.Ward}, {model.District}, {model.Province}";
            var ggaddress = "";
            if (isAppenGoogleMap)
            {
                var @string = $"{model.Address} {model.Ward} {model.District} {model.Province}";
                ggaddress = $"href='https://www.google.com/maps/place/{RemoveSpecialText(@string, "+")}' target='_blank'";
            }
            stringBuilder.AppendLine($"<label class='form-control f-content' id='address'>{Resource.Customer_Address}: <a {ggaddress}> {address} </a> </label>");
            stringBuilder.AppendLine("</div>");
            return stringBuilder.ToString();
        }

        private static string RemoveSpecialText(string input, string replace)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            char[] specs = new char[] { ',', ' ' };
            var segments = input.Split(specs, StringSplitOptions.RemoveEmptyEntries);
            return string.Join('+', segments);
        }

        public static string GetOrderStatusName(this OrderStatus orderStatus) => (orderStatus) switch
        {
            OrderStatus.Ordered => Resource.Customer_OrderStatus_Ordered,
            OrderStatus.Paid => Resource.Customer_OrderStatus_Paid,
            OrderStatus.Prepared => Resource.Customer_OrderStatus_Prepared,
            OrderStatus.Delivering => Resource.Customer_OrderStatus_Delivering,
            OrderStatus.Delivered => Resource.Customer_OrderStatus_Delivered,
            OrderStatus.Cancelled => Resource.Customer_OrderStatus_Cancelled,
            OrderStatus.ReturnedBack => Resource.Customer_OrderStatus_ReturnedBack,
            _ => throw new NotImplementedException()
        };

        public static string GetOrderStatusName(this Shared.Models.OrderStatus orderStatus) => (orderStatus) switch
        {
            Shared.Models.OrderStatus.Ordered => Resource.Customer_OrderStatus_Ordered,
            Shared.Models.OrderStatus.Paid => Resource.Customer_OrderStatus_Paid,
            Shared.Models.OrderStatus.Prepared => Resource.Customer_OrderStatus_Prepared,
            Shared.Models.OrderStatus.Delivering => Resource.Customer_OrderStatus_Delivering,
            Shared.Models.OrderStatus.Delivered => Resource.Customer_OrderStatus_Delivered,
            Shared.Models.OrderStatus.Cancelled => Resource.Customer_OrderStatus_Cancelled,
            Shared.Models.OrderStatus.ReturnedBack => Resource.Customer_OrderStatus_ReturnedBack,
            _ => throw new NotImplementedException()
        };

        public static string GetOrderStatusCss(this Shared.Models.OrderStatus orderStatus) => (orderStatus) switch
        {
            Shared.Models.OrderStatus.Ordered => "btn btn-info",
            Shared.Models.OrderStatus.Paid => "btn btn-primary",
            Shared.Models.OrderStatus.Prepared => "btn btn-warning",
            Shared.Models.OrderStatus.Delivering => "btn btn-secondary",
            Shared.Models.OrderStatus.Delivered => "btn btn-success",
            Shared.Models.OrderStatus.Cancelled => "btn btn-danger",
            Shared.Models.OrderStatus.ReturnedBack => "btn btn-dark",
            _ => throw new NotImplementedException()
        };

        public static string GetOrderStatusCss(this OrderStatus orderStatus) => (orderStatus) switch
        {
            OrderStatus.Ordered => "btn btn-info",
            OrderStatus.Paid => "btn btn-primary",
            OrderStatus.Prepared => "btn btn-warning",
            OrderStatus.Delivering => "btn btn-secondary",
            OrderStatus.Delivered => "btn btn-success",
            OrderStatus.Cancelled => "btn btn-danger",
            OrderStatus.ReturnedBack => "btn btn-dark",
            _ => throw new NotImplementedException()
        };

        public static List<OrderStatus> CantChangeTo(this OrderStatus orderStatus)
        {
            switch(orderStatus)
            {
                case OrderStatus.Paid:
                    return new List<OrderStatus>
                    {
                        OrderStatus.Ordered
                    };
                case OrderStatus.Delivering:
                    return new List<OrderStatus>
                    {
                        OrderStatus.Ordered,
                        OrderStatus.Paid,
                        OrderStatus.Prepared
                    };
                case OrderStatus.Delivered:
                    return new List<OrderStatus>
                    {
                        OrderStatus.Ordered,
                        OrderStatus.Paid,
                        OrderStatus.Prepared,
                        OrderStatus.Delivering,
                        OrderStatus.Cancelled
                    };
                case OrderStatus.Cancelled:
                    return new List<OrderStatus>
                    {
                        OrderStatus.Ordered,
                        OrderStatus.Paid,
                        OrderStatus.Prepared,
                        OrderStatus.Delivering,
                        OrderStatus.Delivered,
                        OrderStatus.ReturnedBack
                    };
                case OrderStatus.ReturnedBack:
                    return new List<OrderStatus>
                    {
                        OrderStatus.Ordered,
                        OrderStatus.Paid
                    };
                default:
                    return new List<OrderStatus>();
            }
        }
    }
}
