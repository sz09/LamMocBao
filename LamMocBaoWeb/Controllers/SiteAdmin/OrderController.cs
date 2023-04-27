using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/don-hang")]
    public class OrderController : AdminController
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IMaterialService _materialService;
        private readonly IProductSizeService _productSizeService;
        private readonly IDeliveryAddressService _deliveryAddressService;
        public OrderController(IOrderService sizeService, IMapper mapper, IDeliveryAddressService deliveryAddressService, IMaterialService materialService, IProductSizeService productSizeService, IOrderDetailService orderDetailService) : base(mapper)
        {
            _orderService = sizeService;
            _deliveryAddressService = deliveryAddressService;
            _materialService = materialService;
            _productSizeService = productSizeService;
            _orderDetailService = orderDetailService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult OrderAdmin(SearchQuery<Shared.Models.Order> searchQuery = null)
        {
            var searchResult = _orderService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/OrderAdmin", ResultListView<Shared.Models.Order>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> OrderAdminView(Guid id)
        {
            var order = await _orderService.LoadAsync(id);
            if (order != null)
            {
                order.OrderDetails = await _orderDetailService.GetByOrderIdAsync(order.Id);
            }
            var vm = _mapper.Map<ViewOrderViewModel>(order);
            
            if (order.IsDeliveryToAnotherAddress)
            {
                var deliveryAddress = await _deliveryAddressService.FindAsync(d => d.OrderId == order.Id);
                vm.DeliveryAddress = _mapper.Map<DeliveryAddressViewModel>(deliveryAddress);
            }
            return View("Admin/OrderAdminView", vm);
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> OrderDelete(Guid id)
        {
            await _orderService.DeleteAsync(id);
            return Success();
        }

        [Route("doi-trang-thai")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(Guid orderId, OrderStatus orderStatus)
        {
            await _orderService.UpdateAsync(orderId, d =>
            {
                d.Status = _mapper.Map<Shared.Models.OrderStatus>(orderStatus);
            });
            await _orderService.MarkOrderSuccessAsync(orderId);
            if (orderStatus == OrderStatus.Delivered)
            {
                await _orderService.ExportStocksAsync(orderId);
            }
            if (orderStatus == OrderStatus.ReturnedBack)
            {
                await _orderService.ImportStocksAsync(orderId);
            }
            return Success();
        }
    }
}
