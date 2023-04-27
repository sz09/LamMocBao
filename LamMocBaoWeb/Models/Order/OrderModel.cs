using System;

namespace LamMocBaoWeb.Models
{
    public class OrderModel
    {
        public AddressModel Address { get; set; }
        public CustomerModel Customer { get; set; }
        public DeliveryAddressModel DeliveryAddress { get; set; }
        public string Note { get; set; }
        public string PromotionCode { get; set; }
        public bool IsDeliveryToAnotherAddress { get; set; }
        public PaymentMethod PaymentType { get; set; }
    }

    public class CustomerModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string BirthDayDate { get; set; }
        public string BirthDayTime { get; set; }
        public BirthDayType BirthDayType { get; set; }
    }

    public class AddressModel
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string NumberAndStreet { get; set; }
    }

    public class DeliveryAddressModel
    {
        public Guid OrderId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Receiver { get; set; }
    }

    public enum PaymentMethod
    {
        COD = 1,
        BankTranfer = 2
    }

    public enum BirthDayType
    {
        SolarCalendar = 1,
        LunarCalendar = 2
    }
}
