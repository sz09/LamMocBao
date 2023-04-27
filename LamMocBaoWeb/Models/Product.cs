using LamMocBaoWeb.ViewModels;
using Shared.Utilities;
using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();

        public string Description { get; set; }

        public IList<string> ImageUrls { get; set; } = new List<string>();
        public List<ProductImageViewModel> Images { get; set; }
        public IList<string> ShortInfomations { get; set; } = new List<string>();
    }
}
