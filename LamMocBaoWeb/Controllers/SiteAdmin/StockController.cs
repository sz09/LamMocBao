using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/kho-hang")]
    public class StockController : AdminController
    {
        public IStockService _stockService;
        public StockController(IStockService sizeService, IMapper mapper) : base(mapper)
        {
            _stockService = sizeService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult StockAdmin(SearchQuery<Services.Services.ProductStock> searchQuery = null)
        {
            var searchResult = _stockService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            ViewBag.MaxPage = searchResult.Total;
            return View("Admin/StockAdmin", ResultListView<ViewModels.ProductStock>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [Route("nhap")]
        [HttpPost]
        public async Task<IActionResult> Import(Guid productId, Guid productSizeId, int quantity)
        {
            var newValue = await _stockService.ImportAsync(productId, productSizeId, quantity);
            return Json(new
            {
                NewValue = newValue
            });
        }

        [Route("xuat")]
        [HttpPost]
        public async Task<IActionResult> Export(Guid productId, Guid productSizeId, int quantity)
        {
            var newValue = await _stockService.ExportAsync(productId, productSizeId, quantity);
            return Json(new
            {
                NewValue = newValue
            });
        }
    }
}
