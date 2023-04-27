using AutoMapper;
using LamMocBaoWeb.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Identify;
using Services.Services;
using Services.Services.Interfaces;
using Shared;
using Shared.Models;
using Shared.Models.Identify;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace LamMocBaoWeb.Controllers
{
    [Route("setup")]
    public class SetupController : Controller
    {
        private readonly IServiceConfig _serviceConfig;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly IProductTypeService _productTypeService;
        private readonly ICategoryService _categoryService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IProductImageService _productImageService;
        private readonly IProductSizeService _productSizeService;
        private readonly ISizeService _sizeService;
        private readonly ITagService _tagService;
        private readonly IMaterialService _materialService;
        private readonly IProductTagService _productTagService;
        private readonly IUserService _userService;
        private readonly IFileServices _azureFileServices;
        private readonly IUploadedImageService _uploadedImageService;
        private readonly IProductMaterialService _productMaterialService;
        private readonly IUserStore<User> _userStore;
        private readonly IRoleStore<Role> _roleStore;
        public SetupController(IProductService productService,
            IMapper mapper,
            IUserService userService, IUserStore<User> userStore,
            IRoleStore<Role> roleStore,
            IProductImageService productImageService,
            IProductTypeService productTypeService,
            ISystemSettingService systemSettingService,
            IWebHostEnvironment _environment, ICategoryService categoryService, IProductSizeService productSizeService, ISizeService sizeService, ITagService tagService, IMaterialService materialService, IServiceConfig serviceConfig, IFileServices azureFileServices, IUploadedImageService uploadedImageService, IProductMaterialService productMaterialService, IProductTagService productTagService)
        {
            _productService = productService;
            _mapper = mapper;
            _userService = userService;
            _userStore = userStore;
            _roleStore = roleStore;
            _productImageService = productImageService;
            _productTypeService = productTypeService;
            _systemSettingService = systemSettingService;

            InitialFilesPath = Path.Combine(_environment.ContentRootPath, "Files");
            _categoryService = categoryService;
            _productSizeService = productSizeService;
            _sizeService = sizeService;
            _tagService = tagService;
            _materialService = materialService;
            _serviceConfig = serviceConfig;
            _azureFileServices = azureFileServices;
            _uploadedImageService = uploadedImageService;
            _productMaterialService = productMaterialService;
            _productTagService = productTagService;
        }

        [Route("view")]
        [HttpGet]
        public IActionResult Index(bool phpTest)
        {
            if (phpTest)
            {
                return Json(new { Con = Constant.SqliteConnectionStr });
            }

            return Json(new { });
        }

        [Route("migrate")]
        public async Task<JsonResult> MigrateProducts()
        {
            var searchResultProducts = _productService.Search(new SearchQuery<Product> { PageSize = int.MaxValue });
            foreach (var product in searchResultProducts.Data)
            {
               await  _productService.UpdateAsync(product.Id, d => { });
            }
            return Json(new { Success = true });
        }

        [Route("dummy/{length:int}")]
        public async Task<JsonResult> DummyProduct(int length = 1000, bool isContinue = false)
        {
            if(!isContinue && await _productService.CountAsync(new SearchQuery<Product>()) > 0)
            {
                return Json(new {  M = "Already dump"});
            }

            var from = 0;
            var to = length;
            if (isContinue)
            {
                var x = await _productService.CountAsync(new SearchQuery<Product>());
                from += x;
                to += x;
            }

            await _sizeService.AddAsync(new Size { Number = 6, Unit = "mm" });
            await _sizeService.AddAsync(new Size { Number = 8, Unit = "mm" });
            await _sizeService.AddAsync(new Size { Number = 10, Unit = "mm" });
            await _sizeService.AddAsync(new Size { Number = 12, Unit = "mm" });
            await _sizeService.AddAsync(new Size { Number = 15, Unit = "mm" });


            await _tagService.AddAsync(new Tag { Name = "Trầm tốc", Label = "Trầm tốc" });
            await _tagService.AddAsync(new Tag { Name = "Aquamarine", Label = "Aquamarine" });
            await _tagService.AddAsync(new Tag { Name = "Charm bạc thái 925", Label = "Charm bạc thái 925" });

            await _materialService.AddAsync(new Material { Name = "Trầm tốc", Description = "Trầm  tốc" });
            await _materialService.AddAsync(new Material { Name = "Aquamarine", Description = "Aquamarine" });
            await _materialService.AddAsync(new Material { Name = "Charm bạc thái 925", Description = "Charm bạc thái 925" });
            var productTypes = _productTypeService.GetAll().ToList();
            var categories = _categoryService.GetAll().ToList();
            var listFrom = new List<string>
            {
                "Indonesia",
                "China",
                "Thailand",
                "Laos",
                "UK",
                "US"
            };
            var rdm = new Random();
            for (int i = from; i < to; i++)
            {
                var sellingPrice = rdm.Next(10000000);
                var purchasingPrice = sellingPrice -  rdm.Next(1500000);

                var product = new Product
                {
                    Name = "Sản phẩm " + i,
                    Description = "Chi tiết " + i,
                    FormattedInfomations = string.Join("\r\n", new List<string>()
                    {
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                        $"Thông tin vui vẻ {i}",
                    }),
                    ProductTypeId = productTypes[rdm.Next(productTypes.Count - 1)].Id,
                    //CategoryId = categories[rdm.Next(categories.Count - 1)].Id,
                    SellingPrice = sellingPrice,
                    PurchasingPrice = purchasingPrice,
                    ProductFrom = listFrom[rdm.Next(listFrom.Count - 1)]
                };
                product.LinkName = product.Name.ToLinkName();

                await _productService.AddAsync(_mapper.Map<Product>(product));
            }
            await UpdateRelatedData();
            return Json(new { length });
        }

        [Route("dummy/update-related-data")]
        public async Task UpdateRelatedData()
        {
            var rdm = new Random();

            var materials = _materialService.GetAll().ToList();
            var sizes = _sizeService.GetAll().ToList();
            var tags = _tagService.GetAll().ToList();
            var urlStrs = new List<string>
            {
                "https://bacminhcanh.com/wp-content/uploads/2019/03/lac-tay-co-3-l-may-man-269.jpg",
                "https://kaigold.vn/wp-content/uploads/2021/12/vong-tay-tam-ho-vang-24k.jpg",
                "https://tramhuongducnam.com/wp-content/uploads/2023/02/top-5-mau-vong-tay-dep-cho-nu.jpg",
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR0_vM8urWpEhGZD1ev8XFBYQeAu05xG4Y2PA&usqp=CAU",
                "https://kimtin2.com/wp-content/uploads/2021/09/Vong-tay-vang-24k-1-chi-50-chiec-NI-50.jpg",
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSahsLed1W6TrFZ3UoRCrcXFg4GCnDJ-qXuYQ&usqp=CAU",
                "https://longbeachpearl.com/upload/product/Nh%E1%BA%ABn_55.LBRG36A3600012-16038973530.jpg",
                "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQjs02vFh1iYYxMUS_lU3q8cy8EZG91lzxHvw&usqp=CAU",
                "https://kaigold.vn/wp-content/uploads/2021/12/Vong-Ty-huu-chi-do.jpg",
                "https://baotinmanhhai.vn/upload/product/thumb_800x0/52-1662973630.jpg",
                "https://phiten.vn/storage/media/KAn2V8KocnVh95T5w2W07t1yPbftzpfr9vL0h0AB.jpeg",
                "https://bacminhcanh.com/wp-content/uploads/2017/11/vong-tay-bac-nu-dang-kieng-tron-tron.jpg",
                "https://www.tierra.vn/files/vta8109_1-I96CimsMNa.jpg",
            };
            var products = _productService.GetAll().ToList();
            
            foreach (var batch in products.Batch(100))
            {
                List<ProductImage> productImages = new List<ProductImage>();
                List<ProductMaterial> productMaterials = new List<ProductMaterial>();
                List<ProductSize> productSizes = new List<ProductSize>();
                List<ProductTag> productTags = new List<ProductTag>();
                foreach (var item in batch)
                {
                    var size = rdm.Next(3, 10);
                    for (int j = 0; j < size; j++)
                    {
                        var url = urlStrs[rdm.Next(urlStrs.Count - 1)];
                        productImages.Add(new ProductImage
                        {
                            ProductId = item.Id,
                            Url = url,
                            UrlPreview = url
                        });
                    }

                    var x = rdm.Next(materials.Count - 1);
                    for (int j = 0; j < x; j++)
                    {
                        productMaterials.Add(new ProductMaterial
                        {
                            MaterialId = materials[rdm.Next(materials.Count - 1)].Id,
                            ProductId = item.Id,
                        });
                    }

                    var x1 = rdm.Next(2, sizes.Count - 1);
                    for (int j = 0; j < x1; j++)
                    {
                        productSizes.Add(new ProductSize
                        {
                            SizeId = sizes[rdm.Next(sizes.Count - 1)].Id,
                            ProductId = item.Id,
                        });
                    }
                    var x2 = rdm.Next(tags.Count - 1);
                    for (int j = 0; j < x2; j++)
                    {
                        productTags.Add(new ProductTag
                        {
                            TagId = tags[rdm.Next(tags.Count - 1)].Id,
                            ProductId = item.Id,
                        });
                    }
                }
                await _productImageService.AddAsync(productImages);
                await _productMaterialService.AddAsync(productMaterials);
                await _productSizeService.AddAsync(productSizes);
                await _productTagService.AddAsync(productTags);
            }
        }
        [Route("")]
        public async Task<JsonResult> SyncFirstData()
        {
            if (_serviceConfig.IsTestMode)
            {
                await AddSystemSettingAsync();
                await AddAdminUserAsync();
                await AddProductTypesAsync();
                await AddCatalogiesAsync();
                return Json(new { Status = "Success initial !!" });
            }
            else
            {
                using (var transactionScrope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        await AddSystemSettingAsync();
                        await AddAdminUserAsync();
                        await AddProductTypesAsync();
                        await AddCatalogiesAsync();
                        transactionScrope.Complete();
                        return Json(new { Status = "Success initial !!" });
                    }
                    catch (Exception ex)
                    {
                        transactionScrope.Dispose();
                        return Json(ex.Message);
                    }
                }
            }
        }

        private async Task AddSystemSettingAsync()
        {
            if (1 == await _systemSettingService.CountAsync(new SearchQuery<SystemSetting>()))
            {
                return;
            }

            await _systemSettingService.AddAsync(new SystemSetting 
            { 
                GoogleMapFrameUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.694323694975!2d105.82304879215766!3d21.004886849381553!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ac7fd86c983b%3A0x497d642512f0257d!2zTmjDoCAxMDQsIDkgTmcuIDk1IFAuIENow7lhIELhu5ljLCBUcnVuZyBMaeG7h3QsIMSQ4buRbmcgxJBhLCBIw6AgTuG7mWksIFZp4buHdCBOYW0!5e0!3m2!1svi!2s!4v1674068579139!5m2!1svi!2s",
                ContactPhoneNumbers = "088 6666 71 - 0898 6666 71",
                ContactAddress = "Nhà 104, Khu 12A, ngách 9/95 Chùa Bộc, Trung Liệt, Đống Đa, Hà Nội",
                Email = "cskh.lammocbao@gmail.com",
                WorkingTime = "Từ Thứ 3 - Chủ Nhật: 9h00 - 18h00",
                Facebook = "https://www.facebook.com/lammocbao",
                HighlightItemsInDays = 7,
                NumberOfHighlightItems = 5
            });
        }

        private async Task AddAdminUserAsync()
        {
            if (await _userStore.FindByNameAsync("admin", default) == null)
            {
                var createRoleResult = await _roleStore.CreateAsync(new Role { Claim = LMBClaims.CUSTOMER_CREATE, Type = LMBRoles.ADMIN }, default);
                if (createRoleResult is LMBIdentityResult identityResult)
                {
                    var user = new User
                    {
                        Email = "phatph.se@gmail.com",
                        Username = "admin",
                        PasswordHash = MD5Utilities.Hash("Admin@123"),
                        CreatedAt = DateTime.UtcNow
                    };
                    user.Roles = new List<UserRole>();
                    user.Roles.Add(new UserRole { RoleId = Guid.Parse(identityResult.Result.ToString()), CreatedAt = DateTime.UtcNow });
                    await _userStore.CreateAsync(user, default);
                }
            }

            if (await _userStore.FindByNameAsync("supperadmin", default) == null)
            {
                var createRoleResult = await _roleStore.CreateAsync(new Role { Claim = LMBClaims.CUSTOMER_CREATE, Type = LMBRoles.SUPER_ADMIN }, default);
                if (createRoleResult is LMBIdentityResult identityResult)
                {
                    var user = new User
                    {
                        Email = "phatph.se@gmail.com",
                        Username = "supperadmin",
                        PasswordHash = MD5Utilities.Hash("Admin@123"),
                        CreatedAt = DateTime.UtcNow
                    };
                    user.Roles = new List<UserRole>();
                    user.Roles.Add(new UserRole { RoleId = Guid.Parse(identityResult.Result.ToString()), CreatedAt = DateTime.UtcNow });
                    await _userStore.CreateAsync(user, default);
                }
            }
        }

        private readonly string InitialFilesPath;
        private async Task AddProductTypesAsync()
        {
            if (0 < await _productTypeService.CountAsync(new SearchQuery<ProductType>()))
            {
                return;
            }
            var path = Path.Combine(InitialFilesPath, "ProductTypes.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<Models.CreateProductTypeModel> items = JsonConvert.DeserializeObject<List<Models.CreateProductTypeModel>>(json);

                foreach (var item in items)
                {
                    var model = _mapper.Map<ProductType>(item);
                    await _productTypeService.AddAsync(model);
                }
            }
        }

        private async Task AddCatalogiesAsync()
        {
            if (0 < await _categoryService.CountAsync(new SearchQuery<Category>()))
            {
                return;
            }

            var path = Path.Combine(InitialFilesPath, "Categories.json");
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<Models.CreateCategoryModel> items = JsonConvert.DeserializeObject<List<Models.CreateCategoryModel>>(json);

                foreach (var item in items)
                {
                    var model = _mapper.Map<Category>(item);
                    await _categoryService.AddAsync(model);
                }
            }
        }
    }
}
