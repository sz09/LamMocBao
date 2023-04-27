using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/ngu-hanh")]
    public class ProductTypeController : AdminController
    {
        private readonly IProductTypeService _productTypeService;
        private readonly ITagService _tagService;
        private readonly IProductTypeTagService _productTypeTagService;
        public ProductTypeController(IProductTypeService productTypeService, IMapper mapper, IProductTypeTagService productTypeTagService, ITagService tagService) : base(mapper)
        {
            _productTypeService = productTypeService;
            _productTypeTagService = productTypeTagService;
            _tagService = tagService;
        }

        [Route("")]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult ProductTypeAdmin(SearchQuery<Shared.Models.ProductType> searchQuery = null)
        {
            var searchResult = _productTypeService.Search(searchQuery);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/ProductTypeAdmin", ResultListView<Shared.Models.ProductType>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        //[HttpGet]
        //[Route("create")]
        //public async Task<ActionResult> ProductTypeAdminCreateAsync()
        //{
        //    ViewBag.IsCreate = true;
        //    var model = new CreateProductTypeModel();
        //    model.SequenceNumber = (await _productTypeService.CountAsync(new SearchQuery<Shared.Models.ProductType>())) + 1;
        //    return View("Admin/ProductTypeAdminCreate", model);
        //}

        //[Route("create")]
        //[HttpPost]
        //public async Task<IActionResult> ProductAdminCreate(CreateProductTypeModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Admin/ProductTypeAdminCreate", model);
        //    }

        //    var productType = _mapper.Map<Shared.Models.ProductType>(model);

        //    await _productTypeService.AddAsync(productType);
        //    return ProductTypeAdmin(new SearchQuery<Shared.Models.ProductType>());
        //}

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> ProductTypeAdminEdit(Guid id)
        {
            var product = await _productTypeService.LoadAsync(id);
            ViewBag.MaxSequenceNumber = (await _productTypeService.CountAsync(new SearchQuery<ProductType>()));
            var tags = await _tagService.GetComboboxListAsync();
            var viewModel = _mapper.Map<EditProductTypeViewModel>(product);
            ViewBag.Tags = new SelectList(tags.OrderBy(viewModel.TagIds), "Id", "Label");
            return View("Admin/ProductTypeAdminEdit", viewModel);
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> ProductTypeAdminUpdate(UpdateProductTypeModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditProductTypeViewModel>(model);
                return View("Admin/ProductTypeAdminEdit", p1);
            }

            var productType = _mapper.Map<ProductType>(model);
            var oldProductType = await _productTypeService.LoadAsync(productType.Id);
            await _productTypeService.UpdateAsync(productType.Id, (d) =>
            {
                d.Name = model.Name;
                d.SequenceNumber = model.SequenceNumber;
                d.LinkName = model.LinkName;
            });
            await UpdateTag(oldProductType, model.TagIds);
            return RedirectToAction("ProductTypeAdmin");
        }

        private async Task UpdateTag(ProductType productType, Dictionary<int, Guid> tagIds)
        {
            tagIds = tagIds ?? new Dictionary<int, Guid>();
            productType.ProductTypeTags = productType.ProductTypeTags ?? new List<Shared.Models.ProductTypeTag>();
            var sourceTagIds = productType.ProductTypeTags.Select(d => d.TagId);
            var tagToAdd = tagIds.Where(d => !sourceTagIds.Contains(d.Value)).ToList();
            var tagToUpdate = tagIds.Where(d => sourceTagIds.Contains(d.Value)).ToList();
            var tagToRemove = productType.ProductTypeTags.Where(d => !tagIds.Values.Contains(d.TagId)).ToList();

            foreach (var item in tagToAdd)
            {
                await _productTypeTagService.AddAsync(new Shared.Models.ProductTypeTag
                {
                    SequenceNumber = tagIds.FirstOrDefault(d => d.Value == item.Value).Key,
                    ProductTypeId = productType.Id,
                    Id = GuidGenerator.NewGuid(),
                    TagId = item.Value
                });
            }

            foreach (var item in tagToUpdate)
            {
                var currentTag = productType.ProductTypeTags.FirstOrDefault(d => d.Tag.Id == item.Value);
                if (currentTag != null)
                {
                    await _productTypeTagService.UpdateAsync(currentTag.Id, d =>
                    {
                        d.SequenceNumber = item.Key;
                    });
                }
            }

            foreach (var item in tagToRemove)
            {
                await _productTypeTagService.DeleteAsync(item.Id, default);
            }
        }
        //[HttpDelete]
        //[Route("delete/{id}")]
        //public async Task<ActionResult> ProductTypeDelete(Guid id)
        //{
        //    await _productTypeService.DeleteAsync(id);
        //    return Success();
        //}
    }
}
