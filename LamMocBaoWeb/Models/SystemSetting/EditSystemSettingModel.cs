using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditSystemSettingViewModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào số điện thoại liên hệ!")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào email!")]
        [EmailAddress]
        public string Email { get; set; }
        public string MomoPaymentQRImage { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào số điện thoại dùng thanh toán momo!")]
        public string MomoPaymentPhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào tên chủ tài khoản momo!")]
        public string MomoPaymentCardHolder { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào số tài khoản!")]
        public string BankAccount { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào tên chủ tài khoản!")]
        public string CardHolderName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào tên ngân hàng!")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào thời gian làm việc!")]
        public string WorkingTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào số điện thoại liên lạc!")]
        public string ContactPhoneNumbers { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào số địa chỉ liên lạc!")]
        public string ContactAddress { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào google map frame!")]
        public string GoogleMapFrameUrl { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào Facebook!")]
        public string Facebook { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào Youtube!")]
        public string Youtube { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào Instagram!")]
        public string Instagram { get; set; }

        public int HighlightItemsInDays { get; set; }
        public int NumberOfHighlightItems { get; set; }
    }
}
