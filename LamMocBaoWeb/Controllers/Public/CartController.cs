using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using LamMocBaoWeb.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("gio-hang")]
    public class CartController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductMaterialService _materialService;
        private readonly IProductSizeService _sizeService;
        public CartController(IMapper mapper, IProductService productService, IProductMaterialService materialService, IProductSizeService sizeService) : base(mapper)
        {
            _productService = productService;
            _materialService = materialService;
            _sizeService = sizeService;
        }

        [Route("")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Index()
        {
            // Get from cookies
            var cookieCarts = HttpContext.Request.Cookies.GetFullInfoProductCarts();
            var productIds = cookieCarts.Select(d => d.Key).ToList();
            var products = await _productService.GetProductsAsync(productIds);
            var sizeIds = cookieCarts.SelectMany(d => d.Value.Select(s => s.S)).ToList();
            var sizes = await _sizeService.GetSizesAsync(sizeIds);
            var viewModel = CartViewModel.From(cookieCarts, products, sizes);
            var suggestionProduct = await _productService.FindAsync(d => true);
            if (suggestionProduct != null)
            {
                viewModel.SuggestionProduct = _mapper.Map<ProductViewModel>(suggestionProduct);
                viewModel.SuggestionProduct.OnlyPreview = true;
            }
            return View("Public/Index", viewModel);
        }

        [Route("cap-nhat-so-luong")]
        [HttpPost]
        public IActionResult UpdateCartQuantity([FromForm] List<ProductCart> models)
        {
            var productGroupIds = models.GroupBy(d => d.Id);
            foreach (var group in productGroupIds)
            {
                var key = $"{Constant.Cookie_CartKey}_{group.Key}";
                var value = HttpContext.Request.Cookies[key];
                if (!string.IsNullOrEmpty(value))
                {
                    var productCartCounts = JsonSerialization.Deserialize<List<ProductCartTempCount>>(value);
                    foreach (var model in group)
                    {
                        try
                        {
                            var productCartCount = productCartCounts.FirstOrDefault(d => d.S == model.SizeId);
                            if (productCartCount != null)
                            {
                                productCartCount.Q = model.Quantity;
                            }
                            HttpContext.Response.Cookies.Append(key, JsonSerialization.Serialize(productCartCounts));
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    HttpContext.Response.Cookies.Delete(key);
                }
            }

            return RedirectToAction("Index", "Cart");
        }

        [HttpPut]
        [Route("add-to-cart/{productId}")]
        public IActionResult AddToCart(Guid productId, int quantity, Guid sizeId)
        {
            var key = $"{Constant.Cookie_CartKey}_{productId}";
            if (HttpContext.Request.Cookies.Any(d => d.Key == key))
            {
                var value = HttpContext.Request.Cookies[key];
                if (!string.IsNullOrEmpty(value))
                {
                    try
                    {
                        var productCartCounts = JsonSerialization.Deserialize<List<ProductCartTempCount>>(value);
                        var productCartCount = productCartCounts.FirstOrDefault(d => d.S == sizeId);
                        if (productCartCount != null)
                        {
                            productCartCount.Q += quantity;
                        }
                        else
                        {
                            productCartCounts.Add(new ProductCartTempCount { Q = quantity, S = sizeId });
                        }

                        HttpContext.Response.Cookies.Append(key, JsonSerialization.Serialize(productCartCounts));
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                var productCartCounts = new List<ProductCartTempCount> { new ProductCartTempCount { Q = quantity, S = sizeId } };
                HttpContext.Response.Cookies.Append(key, JsonSerialization.Serialize(productCartCounts));
            }

            return Success();
        }

        [Route("xoa")]
        [HttpDelete]
        public IActionResult RemoveItem(Guid id, Guid sizeId)
        {
            var key = new ProductCart { Id = id }.Key.Value;
            if (HttpContext.Request.Cookies.Any(d => d.Key == key))
            {
                var value = HttpContext.Request.Cookies[key];
                if (!string.IsNullOrEmpty(value))
                {
                    var productCartCounts = JsonSerialization.Deserialize<List<ProductCartTempCount>>(value);
                    var itemToRemove = productCartCounts.FirstOrDefault(d => d.S == sizeId);
                    if (itemToRemove != null)
                    {
                        productCartCounts.Remove(itemToRemove);
                        HttpContext.Response.Cookies.Append(key, JsonSerialization.Serialize(productCartCounts));
                    }
                }    
            }

            return RedirectToAction("Index", "Cart");
        }

        [Route("login")]
        public IActionResult ConsiderLogin()
        {
            return View("Public/ConsiderLogin");
        }
    }
}
