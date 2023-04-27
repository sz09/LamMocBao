using AutoMapper;
using Humanizer;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Resources;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Caching;
using Services.Services;
using Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("dat-hang")]
    public class OrderController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IPromotionService _promotionService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IDeliveryAddressService _deliveryAddressService;
        private readonly IWebHostEnvironment _environment;
        private readonly InMemoryCache _cache;
        private readonly IProductMaterialService _materialService;
        private readonly IProductSizeService _sizeService;

        public OrderController(IMapper mapper, IProductService productService, IPromotionService promotionService, InMemoryCache cache,
            IWebHostEnvironment environment, IOrderService orderService, IOrderDetailService orderDetailService, ICustomerService customerService,
            IDeliveryAddressService deliveryAddressService, IProductMaterialService materialService, IProductSizeService sizeService) : base(mapper)
        {
            _productService = productService;
            _promotionService = promotionService;
            _cache = cache;
            _environment = environment;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _customerService = customerService;
            _deliveryAddressService = deliveryAddressService;
            _materialService = materialService;
            _sizeService = sizeService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Index(string code)
        {
            var viewModel = await GetOrderAsync(code);
            return View("Public/Index", viewModel);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> DoOrder(OrderModel model)
        {
            var addressList = GetAddressList();

            var province = addressList.FirstOrDefault(d => d.Id == model.Address.Province);
            var district = province.Districts.FirstOrDefault(d => d.Id == model.Address.District);
            var ward = district.Wards.FirstOrDefault(d => d.Id == model.Address.Ward);
            DateTime? birthDay = null;
            if (!string.IsNullOrEmpty(model.Customer.BirthDayDate))
            {
                var combinedDateTime = $"{model.Customer.BirthDayDate} {model.Customer.BirthDayTime ?? "00:00"}";
                birthDay = DateTime.ParseExact(combinedDateTime, "dd/MM/yyyy HH:mm", null);
                birthDay = DateTime.SpecifyKind(birthDay.Value, DateTimeKind.Unspecified);
            }
            var viewModel = await GetOrderAsync(model.PromotionCode);
            var customerId =  await _customerService.AddAsync(new Shared.Models.Customer
            {
                Province = province.Name,
                District = district.Name,
                Ward = ward.Name,
                Address = model.Address.NumberAndStreet,
                BirthDayType = _mapper.Map<Shared.Models.BirthDayType>(model.Customer.BirthDayType),
                FullName = model.Customer.FullName,
                Email = model.Customer.Email,
                PhoneNumber = model.Customer.PhoneNumber,
                Birthday = birthDay
            });
            var orderId = await _orderService.AddAsync(new Shared.Models.Order
            {
                Province = province.Name,
                District = district.Name,
                Ward = ward.Name,
                Address = model.Address.NumberAndStreet,
                Note = model.Note,
                PaymentType = _mapper.Map<Shared.Models.PaymentMethod>(model.PaymentType),
                PromotionCode = model.PromotionCode,
                CalculatedPrice = viewModel.CalculatePrice,
                TotalPrice = viewModel.SumPrice,
                Email = model.Customer.Email,
                CustomerId = customerId,
                Status = Shared.Models.OrderStatus.Ordered,
                PromotionMessage = viewModel.PromotionInfo?.Message,
                IsDeliveryToAnotherAddress = model.IsDeliveryToAnotherAddress
            });

            if (model.IsDeliveryToAnotherAddress)
            {
                var province1 = addressList.FirstOrDefault(d => d.Id == model.DeliveryAddress.Province);
                var district1 = province1.Districts.FirstOrDefault(d => d.Id == model.DeliveryAddress.District);
                var ward1 = district1.Wards.FirstOrDefault(d => d.Id == model.DeliveryAddress.Ward);
                var deliveryAddress = new Shared.Models.DeliveryAddress
                {
                    OrderId = orderId,
                    Address = model.DeliveryAddress.Address ?? string.Empty,
                    Province = province1.Name,
                    District = district1.Name,
                    Ward = ward1.Name,
                    PhoneNumber = model.DeliveryAddress.PhoneNumber,
                    Receiver = model.DeliveryAddress.Receiver
                };
                await _deliveryAddressService.AddAsync(deliveryAddress);
            }
            foreach (var productCart in viewModel.ProductCarts)
            {
                await _orderDetailService.AddAsync(new Shared.Models.OrderDetail
                {
                    OrderId = orderId,
                    ProductId = productCart.Id,
                    Quantity = productCart.Quantity,
                    Price = productCart.Price,
                    SizeId = productCart.SizeId,
                });
            }

            foreach (var cookieKey in HttpContext.Request.Cookies.GetCartCookies())
            {
                HttpContext.Response.Cookies.Delete(cookieKey);
            }

            return RedirectToAction(nameof(Success), "Order");
        }

        [Route("thanh-cong")]
        [HttpGet]
        public async Task<IActionResult> Success()
        {
            return View("Public/Success");
        }

        private async Task<OrderViewModel> GetOrderAsync(string code)
        {
            var cookieCarts = HttpContext.Request.Cookies.GetFullInfoProductCarts();
            var productIds = cookieCarts.Select(d => d.Key).ToList();
            var products = await _productService.GetProductsAsync(productIds);
            var sizeIds = cookieCarts.SelectMany(d => d.Value.Select(s => s.S)).ToList();
            var sizes = await _sizeService.GetSizesAsync(sizeIds);
            var viewModel = OrderViewModel.From(cookieCarts, products, sizes);

            if (!string.IsNullOrEmpty(code))
            {
                var promotion = await _promotionService.GetByCode(code);
                if (promotion != null && !promotion.IsExpired)
                {
                    var discountValue = (viewModel.SumPrice / 100) * (decimal)promotion.Promotion.DiscountPercent;
                    viewModel.PromotionInfo = new PromotionInfo
                    {
                        DiscountValue = discountValue,
                        Code = code,
                        Message = string.Format(Resource.Product_Promotion_Percent, promotion.Promotion.DiscountPercent, discountValue.ToCultureCurrency())
                    };
                }
            }
            return viewModel;
        }

        private List<Province> GetAddressList()
        {
            return _cache.TryGet1Async(nameof(Province).Pluralize(), () =>
            {
                var path = Path.Combine(_environment.WebRootPath, "lib", "vn-address.json");
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Province>>(json);
                }
            });
        }
    }
}
