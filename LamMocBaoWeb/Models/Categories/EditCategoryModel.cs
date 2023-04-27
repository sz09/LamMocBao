using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.ViewModels
{
    public class EditCategoryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public int HomePageSequenceNumber { get; set; }
        public int FilterSequenceNumber { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool ShowOnFilter { get; set; }
    }
}
