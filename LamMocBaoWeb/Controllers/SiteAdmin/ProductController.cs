using AutoMapper;
using LamMocBaoWeb.Models;
using LamMocBaoWeb.Models.Products;
using LamMocBaoWeb.Utilities;
using LamMocBaoWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LamMocBaoWeb.Controllers.Admin
{
    [Route("admin/san-pham")]
    public class ProductController : AdminController
    {
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly IProductImageService _productImageService;
        private readonly ITagService _tagService;
        private readonly ISizeService _sizeService;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;
        private readonly IUploadedImageService _uploadedImageService;
        private readonly IFileServices _fileServices;
        private readonly IHighlightService _highlightService;
        private readonly IProductSizeService _productSizeService;
        private readonly IProductTagService _productTagService;
        private readonly IProductMaterialService _productMaterialService;
        private readonly IProductCategoryService _productCategoryService;
        public ProductController(IProductService productService, IMapper mapper, IFileServices azureFileServices, IProductImageService productImageService,
            IUploadedImageService uploadedImageService, IProductTypeService productTypeService, ITagService tagService, ISizeService sizeService, ICategoryService categoryService,
            IMaterialService materialService, IProductMaterialService productMaterialService, IProductSizeService productSizeService, IProductTagService productTagService, IProductCategoryService productCategoryService, IHighlightService highlightService) : base(mapper)
        {
            _productService = productService;
            _fileServices = azureFileServices;
            _productImageService = productImageService;
            _uploadedImageService = uploadedImageService;
            _productTypeService = productTypeService;
            _tagService = tagService;
            _sizeService = sizeService;
            _categoryService = categoryService;
            _materialService = materialService;
            _productMaterialService = productMaterialService;
            _productSizeService = productSizeService;
            _productTagService = productTagService;
            _productCategoryService = productCategoryService;
            _highlightService = highlightService;
        }

        #region Product
        [Route("")]
        [HttpPost]
        [HttpGet]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> ProductAdmin(SearchQuery<Shared.Models.Product> searchQuery = null)
        {
            var searchResult = _productService.Search(searchQuery);
            ViewBag.HighlightItemIds = await _highlightService.HasCurrentItemHighlightAsync(searchResult.Data.Select(d => d.Id).ToList(), EntityType.Products);
            ViewBag.Page = searchQuery.Page;
            ViewBag.SearchText = searchQuery.Search;
            return View("Admin/ProductAdmin", ResultListView<Shared.Models.Product>.From(searchResult, searchQuery.PageSize, _mapper));
        }

        [HttpGet]
        [Route("create")]
        public async Task<ActionResult> ProductAdminCreate()
        {
            ViewBag.IsCreate = true;
            await SetUpData();
            return View("Admin/ProductAdminCreate", new CreateProductModel());
        }

        [Route("create")]
        [HttpPost]
        public async Task<ActionResult> ProductAdminCreate(CreateProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Admin/ProductAdminCreate", model);
            }

            var product = _mapper.Map<Shared.Models.Product>(model);
            product.ProductImages = new List<ProductImage>();
            product.ProductSizes = new List<ProductSize>();
            product.ProductTags = new List<ProductTag>();
            product.SellingPrice = model.SellingPriceNumber;
            product.PurchasingPrice = model.PurchasingPriceNumber;
            var productId = await _productService.AddAsync(product);
            await UpdateTag(productId, model.ProductTypeId, model.ProductTypeTagIds, model.TagIds);
            if (model.PriceBySizes != null)
            {
                foreach (var item in model.PriceBySizes)
                {
                    await _productSizeService.AddAsync(new ProductSize { ProductId = productId, SizeId = item.Key, SellingPrice = item.Value, Id = GuidGenerator.NewGuid() });
                }
            }

            if (model.MaterialIds != null)
            {
                foreach (var materialId in model.MaterialIds.Values)
                {
                    await _productMaterialService.AddAsync(new ProductMaterial { ProductId = productId, MaterialId = materialId, Id = GuidGenerator.NewGuid() });
                }
            }
            if (model.CategoryIds != null)
            {
                foreach (var categoryId in model.CategoryIds.Values)
                {
                    await _productCategoryService.AddAsync(new ProductCategory { ProductId = productId, CategoryId = categoryId, Id = GuidGenerator.NewGuid() });
                }
            }

            await GetUploadedImagesAsync(model.UploadedImageIds, productId);
            return RedirectToAction("ProductAdmin");
        }

        [Route("edit/{id}")]
        [HttpGet]
        public async Task<ActionResult> ProductAdminEdit(Guid id)
        {
            var product = await _productService.LoadAsync(id);
            await SetUpData();
            var model = _mapper.Map<EditProductViewModel>(product);
            return View("Admin/ProductAdminEdit", model);
        }

        [Route("update")]
        [HttpPost]
        public async Task<ActionResult> ProductAdminUpdate(UpdateProductModel model)
        {
            if (!ModelState.IsValid)
            {
                var p1 = _mapper.Map<EditProductViewModel>(model);
                p1.Images = _mapper.Map<List<ProductImageViewModel>>(await GetUploadedImagesAsync(model.UploadedImageIds, model.Id, false));
                return View("Admin/ProductAdminEdit", p1);
            }
            
            await RemoveOldImages(model);
            var product = _mapper.Map<Shared.Models.Product>(model);
            var oldProduct = await _productService.LoadAsync(model.Id);

            await GetUploadedImagesAsync(model.UploadedImageIds, model.Id);
            await UpdateSize(oldProduct, model.PriceBySizes);
            await UpdateTag(oldProduct.Id, model.ProductTypeId, model.ProductTypeTagIds, model.TagIds);
            await UpdateMaterial(oldProduct, model.MaterialIds);
            await UpdateCategory(oldProduct, model.CategoryIds);

            await _productService.UpdateAsync(product.Id, (d) =>
            {
                d.Name = model.Name;
                d.LinkName = model.LinkName;
                d.Description = model.Description;
                d.SellingPrice = model.SellingPriceNumber;
                d.PurchasingPrice = model.PurchasingPriceNumber;
                d.ProductTypeId = model.ProductTypeId;
                if (d.ProductImages == null)
                {
                    d.ProductImages = new List<ProductImage>();
                }

                d.FormattedInfomations = model.Infomations;
            });

            return RedirectToAction("ProductAdmin");
        }

        [Route("view/{id}")]
        [HttpGet]
        public async Task<ActionResult> ProductAdminView(Guid id)
        {
            var product = await _productService.LoadAsync(id);
            return PartialView("Admin/ProductAdminView", _mapper.Map<EditProductViewModel>(product));
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> ProductDelete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return Success();
        }

        private async Task<List<Guid>> RemoveOldImages(UpdateProductModel model)
        {
            if (string.IsNullOrEmpty(model.RemoveOldImages))
            {
                return new List<Guid>();
            }

            var ids = StringUtilities.Decombined(model.RemoveOldImages, (d) => Guid.Parse(d));
            await _productImageService.DelteAsync(ids);
            return ids;
        }

        private async Task UpdateSize(Shared.Models.Product product, Dictionary<Guid, decimal?> priceBySizes)
        {
            priceBySizes = priceBySizes ?? new Dictionary<Guid, decimal?>();
            product.ProductSizes = product.ProductSizes ?? new List<ProductSize>();
            var sizeToAdd = priceBySizes.Where(d => !product.ProductSizes.Select(d => d.SizeId).Contains(d.Key)).ToList();
            var sizeToRemove = product.ProductSizes.Where(d => !priceBySizes.Keys.Contains(d.SizeId)).ToList();
            var sizeToUpdate = priceBySizes.Where(d => product.ProductSizes.Select(d => d.SizeId).Contains(d.Key)).ToList();

            foreach (var item in sizeToAdd)
            {
                await _productSizeService.AddAsync(new ProductSize { ProductId = product.Id,  Id = GuidGenerator.NewGuid(), SizeId = item.Key, SellingPrice = item.Value });
            }

            foreach (var item in sizeToUpdate)
            {
                var productSize = product.ProductSizes.FirstOrDefault(d => d.SizeId == item.Key);
                if (productSize != null)
                {
                    productSize.SellingPrice = item.Value;
                }
            }
            if (sizeToUpdate.Any())
            {
               await _productSizeService.SaveChangesAsync(default);
            }
            foreach (var item in sizeToRemove)
            {
                await _productSizeService.DeleteAsync(item.Id, default);
            }
        }

        private async Task UpdateTag(Guid productId, Guid? productProductTypeId, Dictionary<int, Guid> productTypeTagIds, Dictionary<int, Guid> tagIds)
        {
            await _productTagService.CleanTagsAsync(productId);
            if (productTypeTagIds != null)
            {
                foreach (var item in productTypeTagIds)
                {
                    await _productTagService.AddAsync(new ProductTag
                    {
                        TagId = item.Value,
                        ProductId = productId,
                        ProductTypeId = productProductTypeId
                    });
                }
            }
            if(tagIds !=null)
            {
                foreach (var item in tagIds)
                {
                    await _productTagService.AddAsync(new ProductTag
                    {
                        TagId = item.Value,
                        ProductId = productId
                    });
                }
            }
        }

        private async Task UpdateMaterial(Shared.Models.Product product, Dictionary<int, Guid> materialIds)
        {
            materialIds = materialIds ?? new Dictionary<int, Guid>();
            product.ProductMaterials = product.ProductMaterials ?? new List<ProductMaterial>();
            var materialToAdd = materialIds.Where(d => !product.ProductMaterials.Select(d => d.MaterialId).Contains(d.Value)).ToList();
            var materialToRemove = product.ProductMaterials.Where(d => !materialIds.Values.Contains(d.MaterialId)).ToList();

            foreach (var item in materialToAdd)
            {
                await _productMaterialService.AddAsync(new ProductMaterial { ProductId = product.Id, Id = GuidGenerator.NewGuid(), MaterialId = item.Value });
            }

            foreach (var item in materialToRemove)
            {
               await _productMaterialService.DeleteAsync(item.Id, default);
            }
        }

        private async Task UpdateCategory(Shared.Models.Product product, Dictionary<int, Guid> categoryIds)
        {
            categoryIds = categoryIds ?? new Dictionary<int, Guid>();
            product.ProductCategories = product.ProductCategories ?? new List<ProductCategory>();
            var categoryToAdd = categoryIds.Where(d => !product.ProductCategories.Select(d => d.CategoryId).Contains(d.Value)).ToList();
            var categoryToRemove = product.ProductCategories.Where(d => !categoryIds.Values.Contains(d.CategoryId)).ToList();

            foreach (var item in categoryToAdd)
            {
                await _productCategoryService.AddAsync(new ProductCategory{ ProductId = product.Id, Id = GuidGenerator.NewGuid(), CategoryId = item.Value });
            }

            foreach (var item in categoryToRemove)
            {
               await _productCategoryService.DeleteAsync(item.Id, default);
            }
        }

        private async Task<IEnumerable<ProductImage>> GetUploadedImagesAsync(string uploadedImageIds, Guid productId, bool isSave = true)
        {
            var result = new List<ProductImage>();
            var guids = uploadedImageIds.Decombined(d => Guid.Parse(d));
            foreach (var guid in guids)
            {
                var uploadedImage = await _uploadedImageService.LoadAsync(guid);
                if(isSave)
                    await _productImageService.AddAsync(new ProductImage { ProductId = productId, Url = uploadedImage.Url, UrlPreview = uploadedImage.UrlPreview });
            }

            return result;
        }

        private async Task SetUpData()
        {
            ViewBag.ProductTypes = await _productTypeService.GetComboboxSelectListItems();
            ViewBag.Categories = (await _categoryService.GetAssignableToProductComboboxListAsync())
                                .GroupBy(d => d.Group)
                                .ToDictionary(d => d.Key, e => e.ToList());
            ViewBag.Tags = await _tagService.GetComboboxSelectListItems();
            ViewBag.ProductTypeTags = await _productTypeService.GetProductTypeTagModelsAsync();
            ViewBag.Sizes = await _sizeService.GetComboboxListAsync();
            ViewBag.Materials = await _materialService.GetComboboxSelectListItems();
        }
        #endregion
    }
}
