using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models.Products
{
    public class UpdateProductModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public Guid? ProductTypeId { get; set; }
        public Guid? CategoryId { get; set; }
        public string LinkName => Name.ToLinkName();

        public string Description { get; set; }
        public string Infomations { get; set; }

        [BindProperty]
        public List<IFormFile> Files { get; set; }

        public string RemoveOldImages { get; set; }
        public string UploadedImageIds { get; set; }
        public string SellingPrice { get; set; }
        public decimal? SellingPriceNumber => SellingPrice.ToDecimal();
        public string PurchasingPrice { get; set; }
        public decimal? PurchasingPriceNumber => PurchasingPrice.ToDecimal();
        public string ProductFrom { get; set; }
        public Dictionary<int, Guid> ProductTypeTagIds { get; set; }
        public Dictionary<int, Guid> TagIds { get; set; }
        public Dictionary<int, Guid> SizeIds { get; set; }
        public Dictionary<int, Guid> MaterialIds { get; set; }
        public Dictionary<int, Guid> CategoryIds { get; set; }
        public Dictionary<Guid, decimal?> PriceBySizes { get; set; }
    }
}
