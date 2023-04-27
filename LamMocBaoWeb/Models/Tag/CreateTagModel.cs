using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateTagModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào hiển thị cho tag !")]
        public string Label { get; set; }
    }
}
