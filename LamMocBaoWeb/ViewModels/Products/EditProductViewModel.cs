using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LamMocBaoWeb.ViewModels
{
    public class EditProductViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public Guid? ProductTypeId { get; set; }
        public Guid? CategoryId { get; set; }
        public List<Guid> ProductTypeTagIds { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public List<string> CategoryStrs => CategoryIds != null ? CategoryIds.Select(d => d.ToString()).ToList(): new List<string>();
        public List<Guid> TagIds { get; set; }
        public List<string> TagIdStrs => TagIds != null ? TagIds.Select(d => d.ToString()).ToList(): new List<string>();
        public List<Guid> SizeIds { get; set; }
        public List<string> SizeIdStrs => SizeIds != null ? SizeIds.Select(d => d.ToString()).ToList() : new List<string>();
        public List<Guid> MaterialIds { get; set; }
        public List<string> MaterialIdStrs => MaterialIds != null ? MaterialIds.Select(d => d.ToString()).ToList() : new List<string>();
        public Dictionary<Guid, decimal?> PriceBySizes { get; set; }
        public string LinkName => Name.ToLinkName();

        public string Description { get; set; }
        public string Infomations { get; set; }
        public string SellingPrice { get; set; }
        public string PurchasingPrice { get; set; }
        public string ProductFrom { get; set; }

        public List<ProductImageViewModel> Images { get; set; }
    }

    public class ProductImageViewModel
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}
