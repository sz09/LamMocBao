using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateSizeModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Range(0, double.MaxValue)]
        public double Number { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào đơn vị !")]
        public string Unit { get; set; }
    }
}
