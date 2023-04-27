using System;

namespace LamMocBaoWeb.ViewModels
{
    public class SystemSettingViewModel
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MomoPaymentQRImage { get; set; }
        public string MomoPaymentPhoneNumber { get; set; }
        public string BankAccount { get; set; }
        public string CardHolderName { get; set; }
        public string WorkingTime { get; set; }
        public string ContactPhoneNumbers { get; set; }
        public string ContactAddress { get; set; }
    }
}
