using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditProductSubTypeViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
    }
}
