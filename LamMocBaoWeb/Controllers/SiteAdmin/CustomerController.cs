using AutoMapper;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin")]
    public class CustomerController : AdminController
    {
        private readonly ICustomerDesiringService _customerDesiringService;
        public CustomerController(ICustomerDesiringService customerDesiringService, IMapper mapper) : base(mapper)
        {
            _customerDesiringService = customerDesiringService;
        }

        #region Customer Desiring
        [Route("muon-mua")]
        [HttpGet]
        public async Task<IActionResult> Index(SearchQuery<Shared.Models.CustomerDesiring> searchQuery = null, bool forceLoad = true)
        {
            var searchResult = _customerDesiringService.SearchInfo(searchQuery);
            var loadTasks = new List<Task>();
            var result = new SearchResult<Shared.Models.CustomerDesiring>();
            result.Total = searchResult.Total;
            foreach (var item in searchResult.Data)
            {
                result.Data.Add(await _customerDesiringService.LoadAsync(item.Id));
            }

            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/CustomerDesiring", ResultListView<Shared.Models.CustomerDesiring>.From(result, searchQuery.PageSize, _mapper));
        }
        
        [Route("khach-hang/dat-hang/{id}")]
        [HttpGet]
        public async Task<IActionResult> Order(Guid id)
        {
            var model = await _customerDesiringService.LoadAsync(id);
            var resultModel = _mapper.Map<CustomerDesiringViewModel>(model);
            LoadRelatedByRelatedInfomations(resultModel);
            return View("Admin/View", resultModel);
        }

        private void LoadRelatedByRelatedInfomations(CustomerDesiringViewModel result)
        {
            var customerPrefereds = _customerDesiringService.GetCustomerPrefered(result.CustomerEmail, result.CustomerPhoneNumber, result.CustomerName, result.Id);
            result.RelatedInfomations = _mapper.Map<ICollection<CustomerDesiringViewModel>>(customerPrefereds); ;
        }
        #endregion
    }
}
