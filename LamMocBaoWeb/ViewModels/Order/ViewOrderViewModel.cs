using LamMocBaoWeb.Models;
using LamMocBaoWeb.Resources;
using LamMocBaoWeb.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LamMocBaoWeb.ViewModels
{
    public class ViewOrderViewModel
    {
        public Guid Id { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionMessage { get; set; }
        public PaymentMethod? PaymentType { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal CalculatedPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsDeliveryToAnotherAddress { get; set; }

        public CustomerViewModel Customer { get; set; }
        public DeliveryAddressViewModel DeliveryAddress { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusStr => OrderStatus.GetOrderStatusName();
    }

    public class OrderDetailModel
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public Guid SizeId { get; set; }
        public string SizeUnit { get; set; }
        public int SizeNumber { get; set; }

        public string SizeName => $"{SizeNumber} {SizeUnit}";
        public decimal TotalPrice => Price * Quantity;
    }

    public class DeliveryAddressViewModel
    {
        public Guid OrderId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Receiver { get; set; }
    }

    public class CustomerViewModel
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BirthdayStr => Birthday.HasValue ? Birthday.Value.ToString("MM/dd/yyyy HH:mm") : string.Empty;
        public DateTime? Birthday { get; set; }
        public BirthDayType BirthDayType { get; set; }

        public string BirthDayTypeStr => (BirthDayType) switch
        {
            BirthDayType.SolarCalendar => Resource.Customer_Birthday_SolarCalendar,
            BirthDayType.LunarCalendar => Resource.Customer_Birthday_LunarCalendar,
            _ => throw new NotImplementedException()
        };
    }

    public enum OrderStatus
    {
        Ordered = 0,
        Paid = 1,
        Prepared = 2,
        Delivering = 3,
        Delivered = 4,
        Cancelled = 5,
        ReturnedBack = 6
    }
}
