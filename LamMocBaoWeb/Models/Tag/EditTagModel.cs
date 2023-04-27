using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditTagViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào hiển thị cho tag !")]
        public string Label { get; set; }
    }
}
