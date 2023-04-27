using AutoMapper;
using LamMocBaoWeb.Resources;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("khuyen-mai")]
    public class PromotionController : BaseController
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        public PromotionController(IMapper mapper, IPromotionService promotionService, IProductService productService) : base(mapper)
        {
            _promotionService = promotionService;
            _productService = productService;
        }

        [Route("kiem-tra")]
        [HttpGet]
        public async Task<IActionResult> Check(string code)
        {
            var cookieCarts = HttpContext.Request.Cookies.GetFullInfoProductCarts();
            var productIds = cookieCarts.Select(d => d.Key).ToList();
            var products = await _productService.GetProductsAsync(productIds);
            var cartViewModel = CartViewModel.From(cookieCarts, products);
            var promotion = await _promotionService.GetByCode(code);
            
            if (promotion != null)
            {
                if (promotion.IsExpired)
                {
                    return Failed(new { ErrorMessage = Resource.Product_Promotion_Expired });
                }

                var discountValue = (cartViewModel.SumPrice / 100) * (decimal)promotion.Promotion.DiscountPercent;
                var text = string.Format(Resource.Product_Promotion_Percent, promotion.Promotion.DiscountPercent, discountValue.ToCultureCurrency());
                return Success(new
                {
                    DiscountInfo = text,
                    CalculatedPrice = (cartViewModel.SumPrice - discountValue).ToCultureCurrency()
                });
            }

            return Failed(new { ErrorMessage = Resource.Product_Promotion_Invalid });
        }
    }
}
