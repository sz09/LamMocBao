using LamMocBaoWeb.Utilities;
using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.ViewModels
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string LinkName { get; set; }

        public string Description { get; set; }
        public decimal SellingPrice { get; set; }
        public string SellingPriceStr => SellingPrice.ToCultureCurrency();

        public IList<string> ImageUrls { get; set; } = new List<string>();
        public List<ProductImageViewModel> Images { get; set; }
        public List<TagViewModel> Tags { get; set; }
        public List<MaterialViewModel> Materials { get; set; }
        public List<StockViewModel> SupportedSizes { get; set; }
        public List<SizePriceModel> PriceBySizes { get; set; }
        public string ShortInfomations { get; set; }
        public string ProductFrom { get; set; } // Xuất xứ
        public bool OnlyPreview { get; set; }
    }

    public class ProductLiteViewModel
    {
        public string Name { get; set; }
        public string LinkName { get; set; }
        public string ImagePreview { get; set; }
        public string ShortDescription { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
