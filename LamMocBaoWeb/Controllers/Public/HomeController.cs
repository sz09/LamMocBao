using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers
{
    [Route("")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly INewsPaperPostService _newsPaperPostService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerDesiringService _customerDesiringService;
        private readonly ICustomerCommentService _customerCommentService;
        private readonly ICustomerPreferedInfosService _customerPreferedInfosService;
        private readonly IPublishedKnowledgeService _knowledgeService;
        private readonly int DisplayCategoryPerRows = 7;
        private readonly int DisplayCategoryPerRowsMobile = 3;
        private readonly int ItemsPerRows = 4;
        private readonly int ItemsPerRowsMobile = 2;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IMapper mapper, 
            ICustomerDesiringService customerDesiringService, 
            ICustomerPreferedInfosService customerPreferedInfosService, 
            IProductTypeService productTypeService, 
            ICategoryService categoryService, 
            INewsPaperPostService newsPaperPostService, 
            ICustomerCommentService customerCommentService,
            IPublishedKnowledgeService knowledgeService) : base(mapper)
        {
            _logger = logger;
            _productService = productService;
            _customerDesiringService = customerDesiringService;
            _customerPreferedInfosService = customerPreferedInfosService;
            _productTypeService = productTypeService;
            _categoryService = categoryService;
            _newsPaperPostService = newsPaperPostService;
            _customerCommentService = customerCommentService;
            _knowledgeService = knowledgeService;
        }

        [Route("")]
        public async Task<IActionResult> HomePage()
        {
            var screenWitdh = Convert.ToInt32(HttpContext.Request.Cookies.FirstOrDefault(D => D.Key == "userdevice").Value);
            ViewBag.ProductTypes = await _productTypeService.GetDisplayProductTypesAsync();
            var items = await _categoryService.GetHomePageDisplayCategoriesAsync();
            if(screenWitdh > 390)
            {
                ViewBag.CategoryRows = items.Batch(DisplayCategoryPerRows);
                ViewBag.NewsPaperPostRows = (await _newsPaperPostService.GetListAsync()).Batch(ItemsPerRows);
                ViewBag.CustomerCommentRows = (await _customerCommentService.GetListAsync()).Batch(ItemsPerRows);
                ViewBag.KnowdgeRows = (await _knowledgeService.GetHomepageRawItemsAsync()).Batch(ItemsPerRows);

            } else
            {
                ViewBag.CategoryRows = items.Batch(DisplayCategoryPerRowsMobile);
                ViewBag.NewsPaperPostRows = (await _newsPaperPostService.GetListAsync()).Batch(ItemsPerRowsMobile);
                ViewBag.CustomerCommentRows = (await _customerCommentService.GetListAsync()).Batch(ItemsPerRowsMobile);
                ViewBag.KnowdgeRows = (await _knowledgeService.GetHomepageRawItemsAsync()).Batch(ItemsPerRowsMobile);

            }
            
            return View();
        }

        [Route("gioi-thieu")]
        public IActionResult Introduction()
        {
            return View();
        }

        [Route("tu-van")]
        public IActionResult Advising()
        {
            return View();
        }

        [Route("tu-van")]
        [HttpPost]
        public async Task<IActionResult> Advising(AdvisingModel model)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModel();
            }

            var interestedInProductIds = model.IncludeOtherProductsInterestedIn ? model.InterestedInProductIds.ToListGuid() : new List<Guid>();
            return await Advise(model, interestedInProductIds);
        }

        [Route("tu-van/{linkName}")]
        [HttpGet]
        [ResponseCache(Duration = 360, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> AdviseByProduct(string linkName)
        {
            var product = await _productService.LoadByLinkName(linkName);
            if (product == null)
            {
                ViewBag.NotFoundProduct = true;
                return View();
            }

            ViewBag.CurrentProductId = product.Id;
            return View(_mapper.Map<Models.Product>(product));
        }

        [Route("tu-van/{linkName}")]
        [HttpPost]
        public async Task<IActionResult> AdviseByProduct(AdvisingModelByProduct model)
        {
            if (!ModelState.IsValid)
            {
                return InvalidModel();
            }

            var interestedInProductIds = model.IncludeOtherProductsInterestedIn ? model.InterestedInProductIds.ToListGuid() : new List<Guid>();
            interestedInProductIds.Add(model.InterestedInProductId);
            return await Advise(model, interestedInProductIds);
        }

        [Route("tu-van/thank-you")]
        public IActionResult ThankYou()
        {
            return View();
        }

        private async Task<IActionResult> Advise(AdvisingModel model, List<Guid> interestedInProductIds)
        {
            var customerDesiring = _mapper.Map<CustomerDesiring>(model);
            // Ignore Save old choosen
            var prefered = _customerDesiringService.GetCustomerPrefered(model.Email, model.PhoneNumber, model.CustomerName);
            var preferedProductIds = prefered.SelectMany(d => d.CustomerPrefereds != null ? d.CustomerPrefereds.Select(d => d.PreferedProductId): new List<Guid>());
            interestedInProductIds = interestedInProductIds.Where(d => !preferedProductIds.Contains(d)).ToList();

            // Avoid ForeignKey Contraint In SQL
            var productIds = _productService.LoadAvailableProductIds(interestedInProductIds);
            var id = await _customerDesiringService.AddAsync(customerDesiring);
            var tasks = productIds.Select(preferedProductId =>
                _customerPreferedInfosService.AddAsync(new CustomerPreferedInfos { 
                    CustomerDesiringId = id, 
                    PreferedProductId = preferedProductId })
            );

            await Task.WhenAll(tasks);
            return RedirectToAction("ThankYou", "Home", new { IsSubmitted = true });
        }
        
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //[Route("error")]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
