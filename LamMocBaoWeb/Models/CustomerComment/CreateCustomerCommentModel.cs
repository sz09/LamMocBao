using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateCustomerCommentModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Required(ErrorMessage = "Vui lòng nhập vào gợi ý !")]

        public string Hint { get; set; }
        [Required(ErrorMessage = "Vui lòng upload hình ảnh !")]
        public Guid? UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }
    }
}
