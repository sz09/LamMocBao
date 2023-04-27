using LamMocBaoWeb.Models;
using LamMocBaoWeb.ViewModels.Cart;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LamMocBaoWeb.ViewModels
{
    public class OrderViewModel
    {
        public static OrderViewModel From(Dictionary<Guid, List<ProductCartTempCount>> savedItems, List<Shared.Models.Product> products, List<ProductSize> sizes)
        {
            var productCarts = new List<ProductCart>();
            foreach (var product in products)
            {
                var carts = savedItems[product.Id];
                productCarts.AddRange(carts.Select(d => {
                    var size = sizes.FirstOrDefault(e => d.S == e.Id);
                    return new ProductCart
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Quantity = d.Q,
                        SizeId = d.S,
                        PreviewImage = product.ProductImages.First().UrlPreview,
                        Price = size.SellingPrice ?? product.SellingPrice ?? 0,
                        SizeName = size != null ? $"{size.Size.Number} {size.Size.Unit}": string.Empty
                    };
                }));
            }
            return new OrderViewModel
            {
                ProductCarts = productCarts
            };
        }
        public List<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();
        public PromotionInfo PromotionInfo { get; set; }
        public decimal SumPrice => ProductCarts.Sum(d => d.TotalPrice);
        public decimal CalculatePrice => PromotionInfo != null ? SumPrice - PromotionInfo.DiscountValue : SumPrice;
    }

    public class PromotionInfo
    {
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public string Message { get; set; }
    }
}
