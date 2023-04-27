using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateProductTypeModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public int SequenceNumber { get; set; }
        public string DisplayImageUrl { get; set; }
        public string Description { get; set; }
        public string SubTypes { get; set; }
    }
}
