using AutoMapper;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using Shared.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Public
{
    [Route("kien-thuc")]
    public class KnowledgeController : BaseController
    {
        private readonly IPublishedKnowledgeService _publishedKnowledgeService;
        private readonly IKnowledgeService _knowledgeService;
        private readonly IProductService _productService;
        private readonly IAnalysisService _analysisService;
        private readonly IServiceConfig _serviceConfig;
        public KnowledgeController(IMapper mapper, IPublishedKnowledgeService publishedKnowledgeService, IKnowledgeService knowledgeService, IServiceConfig serviceConfig, IAnalysisService analysisService, IProductService productService) : base(mapper)
        {
            _publishedKnowledgeService = publishedKnowledgeService;
            _knowledgeService = knowledgeService;
            _serviceConfig = serviceConfig;
            _analysisService = analysisService;
            _productService = productService;
        }

        [Route("")]
        public async Task<IActionResult> Index(SearchQuery<Shared.Models.PublishedKnowledge> searchQuery = null)
        {
            if (searchQuery == null || searchQuery.PageSize == 0 || searchQuery.PageSize > 5)
            {
                searchQuery.PageSize = 5;
            }
            var searchResult = _publishedKnowledgeService.Search(searchQuery);
            await PrepareDataForKnowledge();
            var resultListView = ResultListView<Shared.Models.PublishedKnowledge>.From(searchResult, searchQuery.PageSize, _mapper);
            ViewBag.Page = searchQuery.Page;
            return View("Public/Index", resultListView);
        }

        [Route("{linkName}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> PageForKnowledge(string linkName)
        {
            var knowledge = await _publishedKnowledgeService.LoadByLinkName(linkName);
            if (knowledge == null)
            {
                return LMBNotFound();
            }

            await PrepareDataForKnowledge();
            await _analysisService.IncreaseVisitAsync(knowledge.Id, Shared.Models.EntityType.Knowledges);
            var model = _mapper.Map<KnowledgeViewModel>(knowledge);
            return View("Public/PageForKnowledge", model);
        }

        [Route("preview")]
        public async Task<IActionResult> PreviewKnowledge(Guid id)
        {
            var knowledge = await _knowledgeService.LoadAsync(id);
            if (knowledge == null)
            {
                return LMBNotFound();
            }
            await PrepareDataForKnowledge();
            var model = _mapper.Map<KnowledgeViewModel>(knowledge);
            return View("Public/PageForKnowledge", model);
        }

        private async Task PrepareDataForKnowledge()
        {
            var productsOnTrend = await _analysisService.GetResourceOnTrendsAsync(Shared.Models.EntityType.Products);
            var knowledgesOnTrend = await _analysisService.GetResourceOnTrendsAsync(Shared.Models.EntityType.Knowledges);
            ViewBag.ProductsOnTrend = (await _productService.GetProductsAsync(productsOnTrend))
                                                           .Select(d => new ProductLiteViewModel {
                                                               Name = d.Name,
                                                               LinkName = d.LinkName,
                                                               SellingPrice = d.SellingPrice ?? 0,
                                                               ShortDescription = d.Description,
                                                               ImagePreview = d.ProductImages?.FirstOrDefault().UrlPreview
                                                           }).ToList();
            ViewBag.KnowledgesOnTrend = (await _publishedKnowledgeService.GetKnowledgesAsync(knowledgesOnTrend))
                                                                        .Select(d => new KnowledgeLiteModel
                                                                        {
                                                                            Name = d.Name,
                                                                            LinkName = d.LinkName
                                                                        }).ToList();
            ViewBag.ListKnowledges = _publishedKnowledgeService.Search(new SearchQuery<Shared.Models.PublishedKnowledge>
            {
                Order = NameCollector<Shared.Models.PublishedKnowledge>.Get(d => d.CreatedAt),
                OrderDirection = OrderDirection.Descending,
                PageSize = 5
            }).Data.Select(d => new KnowledgeLiteModel
            {
                Name = d.Name,
                LinkName = d.LinkName
            }).ToList();
        }
    }
}
