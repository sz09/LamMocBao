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
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/loai-san-pham")]
    public class CategoryController : AdminController
    {
        public readonly ICategoryService _categoryService;
        public readonly IServiceConfig _serviceConfig;
        public CategoryController(ICategoryService categoryService, IMapper mapper, IServiceConfig serviceConfig) : base(mapper)
        {
            _categoryService = categoryService;
            _serviceConfig = serviceConfig;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult CategoryAdmin(SearchQuery<Shared.Models.Category> searchQuery = null)
        {
            var searchResult = _categoryService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/CategoryAdmin", ResultListView<Shared.Models.Category>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> CategoryAdminEdit(Guid id)
        {
            var product = await _categoryService.LoadAsync(id);
            ViewBag.MaxSequenceNumber = (await _categoryService.CountAsync(new SearchQuery<Shared.Models.Category>()));
            return View("Admin/CategoryAdminEdit", _mapper.Map<EditCategoryViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> CategoryAdminUpdate(UpdateCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditCategoryViewModel>(model);
                return View("Admin/CategoryAdminEdit", p1);
            }

            var category = _mapper.Map<Shared.Models.Category>(model);
            await _categoryService.UpdateAsync(category.Id, (d) =>
            {
                d.Name = model.Name;
                d.HomePageSequenceNumber = model.HomePageSequenceNumber;
                d.FilterSequenceNumber = model.FilterSequenceNumber;
                d.LinkName = model.LinkName;
                d.Description = model.Description;
            });

            _serviceConfig.ShowOnHomepageFengshuis = new List<ProductFilterByFengShui>();
            return RedirectToAction("CategoryAdmin");
        }
    }
}
