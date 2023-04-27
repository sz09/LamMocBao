using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers
{
    [Route("vat-pham")]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly ICategoryService _categoryService;
        private readonly IKnowledgeService _knowledgeService;
        private readonly ITagService _tagService;
        private readonly ICartService _cartService;
        private readonly IAnalysisService _analysisService;
        private readonly IStockService _stockService;
        private readonly IServiceConfig _serviceConfig;
        private readonly int DisplayProductsPerRows = 4;
        private readonly int NumerSuggestProductInPageByLinkName;
        public ProductController(IMapper mapper,
            IProductService productService,
            IKnowledgeService knowledgeService,
            IProductTypeService productTypeService,
            ICategoryService categoryService,
            ITagService tagService,
            ICartService cartService,
            IServiceConfig serviceConfig,
            IAnalysisService analysisService,
            IStockService stockService) : base(mapper)
        {
            _productService = productService;
            _knowledgeService = knowledgeService;
            _productTypeService = productTypeService;
            _categoryService = categoryService;
            _tagService = tagService;
            _cartService = cartService;
            _serviceConfig = serviceConfig;
            NumerSuggestProductInPageByLinkName = serviceConfig.NumerSuggestProductInPageByLinkName;
            _analysisService = analysisService;
            _stockService = stockService;
        }

        [HttpPost]
        [HttpGet]
        [Route("")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index(SearchQuery<Shared.Models.Product> searchQuery = null, ProductFilter filter = null)
        {
            if (searchQuery == null || searchQuery.PageSize == 0 || searchQuery.PageSize == 10)
            {
                searchQuery.PageSize = _serviceConfig.NumerDisplayProducts; // Fix by mobile
            }

            switch (filter.Order)
            {
                case ProductSort.MoiNhat:
                    searchQuery.OrderBy = "CreatedAt desc";
                    break;
                case ProductSort.BanChay:
                    searchQuery.OrderBy = "SoldNumberCount desc";
                    break;
                case ProductSort.GiaThapToiCao:
                    searchQuery.OrderBy = "SellingPrice asc";
                    break;
                case ProductSort.GiaCaoToiThap:
                    searchQuery.OrderBy = "SellingPrice desc";
                    break;
                default:
                    break;
            }

            var searchResult = _productService.Search(searchQuery, filter);
            var resultListView = ResultListView<ProductViewModel>.From(searchResult, searchQuery.PageSize, _mapper);
            var batches = resultListView.Data.Batch(DisplayProductsPerRows);
            await PrepareMenuAsync();

            #region Set to client filtered
            ViewBag.SearchText = searchQuery.Search;
            ViewBag.Page = searchQuery.Page;
            ViewBag.ProductRows = batches;
            ViewBag.Group = filter.Group;
            ViewBag.CategoryRows = await _categoryService.GetFilterCategoriesAsync();
            ViewBag.HasProduct = batches.Any();
            ViewBag.Search = searchQuery?.Search;
            ViewBag.ProductTypeId = filter?.ProductTypeId;
            ViewBag.ProductTypeTagId = filter?.ProductTypeTagId;
            ViewBag.CategoryId = filter?.CategoryId;
            ViewBag.CurrentSort = filter?.Order.ToString().ToLower();
            ViewBag.PriceFrom = filter?.PriceFrom ?? 0;
            ViewBag.PriceTo = filter?.PriceTo ?? _serviceConfig.FilterProductPriceUpTo;
            #endregion
            return View("Public/Index", resultListView);
        }

        [HttpGet]
        [Route("goi-y")]
        public async Task<IActionResult> SuggestionProducts([FromBody] SuggestProduct model)
        {
            return Json(await _productService.SuggestionProductsAsync(model.Ids, model.Quantity));
        }


        [HttpGet]
        [Route("{name}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> ByLinkName(string name)
        {
            var product = await _productService.LoadByLinkName(name);
            if (product == null)
            {
                return LMBNotFound();
            }

            await _analysisService.IncreaseVisitAsync(product.Id, Shared.Models.EntityType.Products);
            var quantityBySizes = _mapper.Map<List<ViewModels.ProductSizeStock>>(await _stockService.GetQuantityAsync(product.Id));
            var viewModel = _mapper.Map<ProductViewModel>(product);
            var productSizeIds = quantityBySizes.Select(d => d.ProductSizeId).ToList();
            foreach (var item in viewModel.SupportedSizes.Where(d => !productSizeIds.Contains(d.Id)).ToList())   
            {
                quantityBySizes.Add(new ViewModels.ProductSizeStock { ProductSizeId = item.Id, CurrentQuantity = 0 });
            }
            ViewBag.QuantityBySizes = quantityBySizes;
            ViewBag.SuggestionProducts = await _productService.SuggestionProductsAsync(new List<Guid> {  product.Id }, NumerSuggestProductInPageByLinkName);
            return View("Public/ByLinkName", viewModel);
        }

        private async Task<MenuHierarchy> PrepareMenuAsync()
        {
            var menuHierarchy = new MenuHierarchy();
            var loadMenuTasks = new List<Task>();
            loadMenuTasks.Add(Task.Run(async () =>
            {
                var productTypes = await _productTypeService.GetFullProductTypesAsync();
                menuHierarchy.MenuByProductTypes = productTypes.Select(d => new MenuByProductType
                {
                    Id = d.Id,
                    Name = d.Name,
                    TypeTags = d.ProductTypeTags.OrderBy(d => d.SequenceNumber)
                                                .Select(d => new ProductTypeTag { 
                                                    Id = d.TagId, 
                                                    Name = d.Tag.Name
                                                })
                                                .ToList() ?? new List<ProductTypeTag>()
                }).ToList();
            }));

            loadMenuTasks.Add(Task.Run(async () =>
            {
                var categories = await _categoryService.GetFilterCategoriesAsync();
                menuHierarchy.MenuByCategories = categories.GroupBy(d => d.Group)
                                                           .ToDictionary(d => d.Key, e => e.Select(d => new MenuByCategory
                                                           {
                                                               Id = d.Id,
                                                               Name = d.Name,
                                                               IsShowOnFilter = d.ShowOnFilter
                                                           }).ToList());
            }));

            loadMenuTasks.Add(Task.Run(async () =>
            {
                var tags = await _tagService.GetFullTagsAsync();
                menuHierarchy.Tags = tags.Select(d => new TagViewModel
                {
                    Id = d.Id,
                    Name = d.Label,
                    Label = d.Name
                }).ToList();
            }));

            await Task.WhenAll(loadMenuTasks);
            ViewBag.MenuHierarchy = menuHierarchy;
            return menuHierarchy;
        }
    }
}
