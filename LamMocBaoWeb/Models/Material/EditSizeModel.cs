using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditMaterialViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
