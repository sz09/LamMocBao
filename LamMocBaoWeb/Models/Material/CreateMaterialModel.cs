using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateMaterialModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
