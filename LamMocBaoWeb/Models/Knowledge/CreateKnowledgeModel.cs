using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateKnowledgeModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public string Content { get; set; }
        public string Summary { get; set; }
        [Required(ErrorMessage = "Vui lòng upload hình ảnh !")]
        public Guid? UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }
    }
}
