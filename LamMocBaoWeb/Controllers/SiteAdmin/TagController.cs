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
    [Route("admin/tag")]
    public class TagController : AdminController
    {
        public ITagService _tagService;
        public TagController(ITagService tagService, IMapper mapper) : base(mapper)
        {
            _tagService = tagService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult TagAdmin(SearchQuery<Shared.Models.Tag> searchQuery = null)
        {
            var searchResult = _tagService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/TagAdmin", ResultListView<Shared.Models.Tag>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult TagAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/TagAdminCreate", new CreateTagModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> ProductAdminCreate(CreateTagModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/TagAdminCreate", model);
            }

            var tag = _mapper.Map<Shared.Models.Tag>(model);

            await _tagService.AddAsync(tag);
            return RedirectToAction("TagAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> TagAdminEdit(Guid id)
        {
            var product = await _tagService.LoadAsync(id);
            return View("Admin/TagAdminEdit", _mapper.Map<EditTagViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> TagAdminUpdate(UpdateTagModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditTagViewModel>(model);
                return View("Admin/TagAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.Tag>(model);
            await _tagService.UpdateAsync(product.Id, (d) =>
            {
                d.Name = model.Name;
            });
            return RedirectToAction("TagAdmin");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> TagDelete(Guid id)
        {
            await _tagService.DeleteAsync(id);
            return Success();
        }
    }
}
