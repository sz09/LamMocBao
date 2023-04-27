using LamMocBaoWeb.Models;
using Microsoft.AspNetCore.Http;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LamMocBaoWeb.ViewModels.Cart
{
    public enum CartStatus {
        OnCart,
        OnPayment,
        Complete
    }

    public class CartViewModel
    {
        //public static CartViewModel From(List<Shared.Models.Product> products, IEnumerable<KeyValuePair<Guid, int>> cookieCarts)
        //{
        //    return new CartViewModel
        //    {
        //        ProductCarts = products.Select(product => new ProductCart
        //        {
        //            Id = product.Id,
        //            PreviewImage = product.ProductImages.First().UrlPreview,
        //            Name = product.Name,
        //            Price = product.SellingPrice ?? 0,
        //            Quantity = cookieCarts.FirstOrDefault(d => d.Key == product.Id).Value
        //        }).ToList()
        //    };
        //}

        public static CartViewModel From(Dictionary<Guid, List<ProductCartTempCount>> savedItems, 
                                         List<Shared.Models.Product> products,
                                         List<Shared.Models.ProductSize> sizes = null)
        {
            var productCarts = new List<ProductCart>();
            foreach (var product in products)
            {
                var carts = savedItems[product.Id];
                productCarts.AddRange(carts.Select(d => {
                    var size = sizes?.FirstOrDefault(s => s.Id == d.S);
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
            return new CartViewModel
            {
                ProductCarts = productCarts
            };
        }

        public List<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();
        public decimal SumPrice => ProductCarts.Sum(d => d.TotalPrice);
        public ProductViewModel SuggestionProduct { get; set; }
    }

    public class ProductCart
    {
        public Guid Id { get; set; }
        public string PreviewImage { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid SizeId { get; set; }
        public string SizeName { get; set; }
        public decimal TotalPrice => Quantity * Price;

        public Lazy<string> Key => new Lazy<string>(() => $"{Constant.Cookie_CartKey}_{Id}");
    }
}
