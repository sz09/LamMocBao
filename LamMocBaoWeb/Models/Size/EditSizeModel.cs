using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditSizeViewModel
    {
        public Guid Id { get; set; }

        [Range(0, double.MaxValue)]
        public double Number { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào đơn vị !")]
        public string Unit { get; set; }
    }
}
