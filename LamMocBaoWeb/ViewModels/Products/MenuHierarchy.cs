using Shared.Models;
using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.ViewModels
{
    public class MenuHierarchy
    {
        public List<MenuByProductType> MenuByProductTypes { get; set; }
        public Dictionary<CategoryGroup, List<MenuByCategory>> MenuByCategories { get; set; }
        public List<TagViewModel> Tags { get; set; }
    }

    // Ngũ hành
    public class MenuByProductType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProductTypeTag> TypeTags { get; set; }
    }


    public class ProductTypeTag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    // Chất liệu
    public class MenuByCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsShowOnFilter { get; set; }
    }
}
