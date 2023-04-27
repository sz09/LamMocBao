using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditCustomerCommentViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào gợi ý !")]

        public string Hint { get; set; }
        [Required(ErrorMessage = "Vui lòng upload hình ảnh!")]
        public string ImagePreview { get; set; }
        public Guid UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }
    }
}
