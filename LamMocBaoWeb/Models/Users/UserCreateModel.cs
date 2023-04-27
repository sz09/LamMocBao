using Shared.Models.Identify;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models.Users
{
	public class UserCreateModel
	{
		public UserCreateModel()
		{
		}

        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản đăng nhập !")]		
		public string Username { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu !")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu !")]
		public string RetypePassword { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập địa chỉ email !")]
		public string Email { get; set; }
		public string PhoneNumber { get; set; }

	}
}

