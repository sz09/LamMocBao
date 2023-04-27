using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/kien-thuc")]
    public class KnowledgeController : AdminController
    {
        public IKnowledgeService _knowledgeService;
        public IPublishedKnowledgeService _publishedKnowledgeService;
        public IHighlightService _highlightService;
        public KnowledgeController(IKnowledgeService knowledgeService, IMapper mapper, IPublishedKnowledgeService publishedKnowledgeService, IHighlightService highlightService) : base(mapper)
        {
            _knowledgeService = knowledgeService;
            _publishedKnowledgeService = publishedKnowledgeService;
            _highlightService = highlightService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> KnowledgeAdminAsync(SearchQuery<Shared.Models.Knowledge> searchQuery = null)
        {
            var searchResult = _knowledgeService.Search(searchQuery);
            var source = searchResult.Data.Select(d => d.Id).ToList();
            var publishedKnowledgeIds = await _publishedKnowledgeService.GetKnowledgesBySourceAsync(source);
            ViewBag.PublishedKnowledgeIds = publishedKnowledgeIds;
            ViewBag.HighlightItemIds = await _highlightService.HasCurrentItemHighlightAsync(publishedKnowledgeIds.Values.Where(d => d.HasValue).Select(d => d.Value).ToList(), EntityType.Knowledges);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/KnowledgeAdmin", ResultListView<Shared.Models.Knowledge>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult KnowledgeAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/KnowledgeAdminCreate", new CreateKnowledgeModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> KnowledgeAdminCreate(CreateKnowledgeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/KnowledgeAdminCreate", model);
            }

            var knowledge = _mapper.Map<Shared.Models.Knowledge>(model);

            await _knowledgeService.AddAsync(knowledge);
            return RedirectToAction("KnowledgeAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> KnowledgeAdminEdit(Guid id)
        {
            var product = await _knowledgeService.LoadAsync(id);
            return View("Admin/KnowledgeAdminEdit", _mapper.Map<EditKnowledgeViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> KnowledgeAdminUpdate(UpdateKnowledgeModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditKnowledgeViewModel>(model);
                return View("Admin/KnowledgeAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.Knowledge>(model);
            await _knowledgeService.UpdateAsync(product.Id, (d) =>
            {
                d.Name = model.Name;
                d.LinkName = model.LinkName;
                d.Content = model.Content;
                d.SequenceNumber = model.SequenceNumber;
                d.Summary = model.Summary;
                d.UploadedImageId = model.UploadedImageId.Value;
            });

            return RedirectToAction("KnowledgeAdmin");
        }

        [Route("publish")]
        [HttpPost]
        public async Task<JsonResult> Publish(Guid id)
        {
            var originKnowledge = await _knowledgeService.LoadAsync(id);
            if (originKnowledge != null)
            {
                var publishedKnowledge = await _publishedKnowledgeService.FindAsync(d => d.OriginKnowledgeId == id);
                if (publishedKnowledge != null)
                {
                    publishedKnowledge.SequenceNumber = originKnowledge.SequenceNumber;
                    publishedKnowledge.LinkName = originKnowledge.LinkName;
                    publishedKnowledge.UploadedImageId = originKnowledge.UploadedImageId;
                    publishedKnowledge.Name = originKnowledge.Name;
                    publishedKnowledge.Content = originKnowledge.Content;
                    publishedKnowledge.Summary = originKnowledge.Summary;

                    await _publishedKnowledgeService.SaveChangesAsync(default);
                }
                else
                {
                    await _publishedKnowledgeService.AddAsync(new Shared.Models.PublishedKnowledge
                    {
                        Content = originKnowledge.Content,
                        LinkName = originKnowledge.LinkName,
                        Name = originKnowledge.Name,
                        SequenceNumber = originKnowledge.SequenceNumber,
                        OriginKnowledgeId = originKnowledge.Id,
                        Summary = originKnowledge.Summary,
                        UploadedImageId = originKnowledge.UploadedImageId
                    });
                }
            }

            return Success();
        }

        
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> KnowledgeDelete(Guid id)
        {
            await _knowledgeService.DeleteAsync(id);
            await _publishedKnowledgeService.DeleteAsync(d => d.OriginKnowledgeId == id);
            return Success();
        }
    }
}
