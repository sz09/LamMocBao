using System;
using System.Collections.Generic;

namespace LamMocBaoWeb.ViewModels
{
    public class ProductStock
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public List<ProductSizeStock> ProductSizeStocks { get; set; }
    }
    public class ProductSizeStock
    {
        public Guid ProductSizeId { get; set; }
        public string DisplayUnit { get; set; }
        public int CurrentQuantity { get; set; }
    }
}
