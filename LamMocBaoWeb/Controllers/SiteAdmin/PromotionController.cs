using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/khuyen-mai")]
    public class PromotionController : AdminController
    {
        public IPromotionService _promotionService;
        public PromotionController(IPromotionService promotionService, IMapper mapper) : base(mapper)
        {
            _promotionService = promotionService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult PromotionAdmin(SearchQuery<Shared.Models.Promotion> searchQuery = null)
        {
            var searchResult = _promotionService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/PromotionAdmin", ResultListView<Shared.Models.Promotion>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult PromotionAdminCreate()
        {
            ViewBag.IsCreate = true;
            return View("Admin/PromotionAdminCreate", new CreatePromotionModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> ProductAdminCreate(CreatePromotionModel model)
        {
            if (!ModelState.IsValid || !IsValidMode(model))
            {
                return View("Admin/PromotionAdminCreate", model);
            }

            var promotion = _mapper.Map<Shared.Models.Promotion>(model);
            promotion.From = promotion.From.EnsureStartOfDate();
            promotion.To = promotion.To.EnsureStartOfDate();
            await _promotionService.AddAsync(promotion);
            return RedirectToAction("PromotionAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> PromotionAdminEdit(Guid id)
        {
            var product = await _promotionService.LoadAsync(id);
            return View("Admin/PromotionAdminEdit", _mapper.Map<EditPromotionViewModel>(product));
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> PromotionAdminUpdate(UpdatePromotionModel model)
        {
            if (!ModelState.IsValid || !IsValidMode(model))
            {
                var p1 = _mapper.Map<EditPromotionViewModel>(model);
                return View("Admin/PromotionAdminEdit", p1);
            }

            var product = _mapper.Map<Shared.Models.Promotion>(model);
            await _promotionService.UpdateAsync(product.Id, (d) =>
            {
                d.Code = model.Code;
                d.Content = model.Content;
                d.DiscountPercent = model.DiscountPercent;
                d.PromotionMode = _mapper.Map<Shared.Models.PromotionMode>(model.PromotionMode);
                d.From = model.From.EnsureStartOfDate();
                d.To = model.To.EnsureEndOfDate();
                d.IsActive = model.IsActive;
            });
            return RedirectToAction("PromotionAdmin");
        }

        private bool IsValidMode(PromotionModel model)
        {
            if (model.PromotionMode == PromotionMode.Period)
            {
                var isValid = model.From.HasValue || model.To.HasValue;
                if (!isValid)
                {
                    ModelState.AddModelError("PromotionMode", "Vui lòng cung cấp khoảng thời gian");
                }

                return isValid;
            }

            return true;
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> PromotionDelete(Guid id)
        {
            await _promotionService.DeleteAsync(id);
            return Success();
        }
    }
}
