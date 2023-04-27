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
    [Route("admin/chat-lieu")]
    public class MaterialController : AdminController
    {
        public IMaterialService _sizeService;
        public MaterialController(IMaterialService sizeService, IMapper mapper) : base(mapper)
        {
            _sizeService = sizeService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult MaterialAdmin(SearchQuery<Shared.Models.Material> searchQuery = null)
        {
            var searchResult = _sizeService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/MaterialAdmin", ResultListView<Shared.Models.Material>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult MaterialAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/MaterialAdminCreate", new CreateMaterialModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> ProductAdminCreate(CreateMaterialModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/MaterialAdminCreate", model);
            }

            var size = _mapper.Map<Shared.Models.Material>(model);

            await _sizeService.AddAsync(size);
            return RedirectToAction("MaterialAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> MaterialAdminEdit(Guid id)
        {
            var product = await _sizeService.LoadAsync(id);
            return View("Admin/MaterialAdminEdit", _mapper.Map<EditMaterialViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> MaterialAdminUpdate(UpdateMaterialModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditMaterialViewModel>(model);
                return View("Admin/MaterialAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.Material>(model);
            await _sizeService.UpdateAsync(product.Id, (d) =>
            {
                d.Name = model.Name;
                d.Description = model.Description;
            });

            return RedirectToAction("MaterialAdmin");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> MaterialDelete(Guid id)
        {
            await _sizeService.DeleteAsync(id);
            return Success();
        }
    }
}
