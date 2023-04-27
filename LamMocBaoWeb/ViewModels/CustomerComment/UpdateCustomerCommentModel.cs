using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class UpdateCustomerCommentModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào gợi ý !")]

        public string Hint { get; set; }

        [Required(ErrorMessage = "Vui lòng upload hình ảnh !")]
        public Guid? UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }
    }
}
