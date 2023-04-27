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
    [Route("admin/kich-co")]
    public class SizeController : AdminController
    {
        public ISizeService _sizeService;
        public SizeController(ISizeService sizeService, IMapper mapper) : base(mapper)
        {
            _sizeService = sizeService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult SizeAdmin(SearchQuery<Shared.Models.Size> searchQuery = null)
        {
            var searchResult = _sizeService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/SizeAdmin", ResultListView<Shared.Models.Size>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult SizeAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/SizeAdminCreate", new CreateSizeModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> ProductAdminCreate(CreateSizeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/SizeAdminCreate", model);
            }

            var size = _mapper.Map<Shared.Models.Size>(model);

            await _sizeService.AddAsync(size);
            return RedirectToAction("SizeAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> SizeAdminEdit(Guid id)
        {
            var product = await _sizeService.LoadAsync(id);
            return View("Admin/SizeAdminEdit", _mapper.Map<EditSizeViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> SizeAdminUpdate(UpdateSizeModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditSizeViewModel>(model);
                return View("Admin/SizeAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.Size>(model);
            await _sizeService.UpdateAsync(product.Id, (d) =>
            {
                d.Number = model.Number;
                d.Unit = model.Unit;
            });

            return RedirectToAction("SizeAdmin");
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> SizeDelete(Guid id)
        {
            await _sizeService.DeleteAsync(id);
            return Success();
        }
    }
}
