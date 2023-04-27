using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class UpdateCategoryModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public int HomePageSequenceNumber { get; set; }
        public int FilterSequenceNumber { get; set; }
        public string DisplayImageUrl { get; set; }
        public string Description { get; set; }

        public bool ShowOnHomePage { get; set; }
        public bool ShowOnFilter { get; set; }
    }
}
