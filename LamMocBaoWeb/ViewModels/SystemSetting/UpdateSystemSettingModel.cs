using System;

namespace LamMocBaoWeb.Models
{
    public class UpdateSystemSettingModel
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string BankAccount { get; set; }
        public string CardHolderName { get; set; }
        public string BankName { get; set; }
        public string MomoPaymentPhoneNumber { get; set; }
        public string MomoPaymentCardHolder { get; set; }
        public string ContactPhoneNumbers { get; set; }
        public string WorkingTime { get; set; }
        public string ContactAddress { get; set; }
        public string GoogleMapFrameUrl { get; set; }
        public string Facebook { get; set; }
        public string Youtube { get; set; }
        public string Instagram { get; set; }

        public int HighlightItemsInDays { get; set; }
        public int NumberOfHighlightItems { get; set; }
    }
}
