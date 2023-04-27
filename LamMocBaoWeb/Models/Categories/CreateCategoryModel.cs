using Shared.Models;
using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class CreateCategoryModel
    {
        public Guid Id { get; set; } = GuidGenerator.NewGuid();

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public int HomePageSequenceNumber { get; set; }
        public int FilterSequenceNumber { get; set; }
        public string DisplayImageUrl { get; set; }
        public string Description { get; set; }
        public bool ShowOnHomePage { get; set; }
        public bool ShowOnFilter { get; set; }
        public bool AssignableToProduct { get; set; }
        public CategoryGroup Group { get; set; }
        public OriginalType? OriginalType { get; set; }
    }
}
