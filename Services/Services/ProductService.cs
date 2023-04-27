using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public enum ProductSort
    {
        MoiNhat,
        BanChay,
        GiaThapToiCao,
        GiaCaoToiThap
    }

    public class ProductFilter : Filter<Product>
    {
        public CategoryGroup? Group { get; set; }
        public Guid? Type { get; set; }
        public Guid? ProductTypeId => Type;
        public Guid? Tag { get; set; }
        public Guid? ProductTypeTagId => Tag;
        public Guid? Category { get; set; }
        public Guid? CategoryId => Category;
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public ProductSort Order  { get; set; }
    }

    public class ProductService : Service<Product>, IService<Product>, IProductService
    {
        public ProductService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public SearchResult<Product> Search(SearchQuery<Product> searchQuery, ProductFilter filter = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
            if (filter != null)
            {
                var productFilter = new ProductFilter();
                Func<Product, bool> predicate = (d) => true;
                if (filter.ProductTypeId.HasValue())
                {
                    query = query.Where(d => d.ProductTypeId.HasValue && d.ProductTypeId == filter.ProductTypeId);
                }

                if (filter.ProductTypeTagId.HasValue())
                {
                    var productHasTypeTagIds = _context.ProductTags
                                                    .Where(d => !d.IsDeleted && d.ProductTypeId == filter.ProductTypeId && d.TagId == filter.ProductTypeTagId)
                                                    .Select(d => d.ProductId)
                                                    .ToList();
                    query = query.Where(d => productHasTypeTagIds.Contains(d.Id));
                }

                if (filter.CategoryId.HasValue())
                {
                    var productIdByCategorys = _context.ProductCategories
                                                    .Where(d => !d.IsDeleted && d.CategoryId == filter.CategoryId)
                                                    .Select(d => d.ProductId)
                                                    .ToList();
                    query = query.Where(d => productIdByCategorys.Contains(d.Id));
                }
                

                if (filter.Group.HasValue)
                {
                    if (filter.Group == CategoryGroup.VatPhamTheoNguHanh)
                    {
                        query = query.Where(d => d.ProductTypeId.HasValue);

                    }
                    else
                    {
                        var productIdByCategorys = _context.Categories
                                                        .Where(d => !d.IsDeleted && d.Group == filter.Group)
                                                        .Join(_context.ProductCategories.Where(d => !d.IsDeleted), d => d.Id, e => e.CategoryId, (d, s) => s.ProductId)
                                                        .ToList();
                        query = query.Where(d => productIdByCategorys.Contains(d.Id));
                    }
                }

                if (filter.PriceFrom.HasValue)
                {
                    query = query.Where(d => d.SellingPrice >= filter.PriceFrom);
                }

                if (filter.PriceTo.HasValue)
                {
                    query = query.Where(d => d.SellingPrice <= filter.PriceTo);
                }

                productFilter.Predicate = predicate;
                searchQuery.Filter = productFilter;
            }
            if (!string.IsNullOrWhiteSpace(searchQuery.Search))
            {
                query = query.Where(d => d.Name.Contains(searchQuery.Search) || d.NameWithoutUTF8.Contains(searchQuery.Search));
            }


            var result = new SearchResult<Product>();
            if (searchQuery.IncludeTotal)
            {
                result.Total = query.Count();
            }

            switch (searchQuery.OrderDirection)
            {
                case OrderDirection.Ascending:
                    query = query.OrderBy(searchQuery.Order ?? "CreatedAt");
                    break;
                case OrderDirection.Descending:
                    query = query.OrderByDescending(searchQuery.Order ?? "CreatedAt");
                    break;
                default:
                    break;
            }

            query = query.Skip(searchQuery.Skip)
                         .Take(searchQuery.PageSize);
            result.Data = query.ToList();
            var productIds = result.Data.Select(d => d.Id as Guid?).ToList();
            var listProductImages = _context.ProductImages.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).ToList();
            var listProductSizes = _context.ProductSizes.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).Include(d => d.Size).ToList();
            var listProductTags = _context.ProductTags.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).Include(d => d.Tag).ToList();
            var productTypeIds = result.Data.Where(d => d.ProductTypeId.HasValue).Select(d => d.ProductTypeId.Value).ToList();
            var productTypes = _context.ProductTypes.Where(d => !d.IsDeleted && productTypeIds.Contains(d.Id)).ToList();
            var productCategories = _context.ProductCategories.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).ToList();
            foreach (var product in result.Data)
            {
                product.ProductImages = listProductImages.Where(d => d.ProductId == product.Id).ToList();
                product.ProductSizes = listProductSizes.Where(d => d.ProductId == product.Id).ToList();
                product.ProductTags = listProductTags.Where(d => d.ProductId == product.Id).DistinctBy(d => d.TagId).ToList();
                product.ProductType = productTypes.FirstOrDefault(d => d.Id == product.ProductTypeId);
                product.ProductCategories = productCategories.Where(d => d.Id == product.Id).ToList();
            }

            return result;
        }

        public async Task<Product> LoadByLinkName(string name)
        {
            return LoadAsync(CacheKey.CacheKeyFor<Product>(name), d => d.LinkName == name); 
        }

        public List<Guid> LoadAvailableProductIds(List<Guid> ids)
        {
            var productiIds = DbSet.Where(d => ids.Contains(d.Id) && !d.IsDeleted)
                                   .Select(d => d.Id)
                                   .ToList();
            return productiIds;
        }

        public new async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var productSizes = _context.ProductSizes.Where(d => d.ProductId == id).ToList();
            foreach (var productSize in productSizes)
            {
                productSize.IsDeleted = true;
            }
            var productTags = _context.ProductTags.Where(d => d.ProductId == id).ToList();
            foreach (var productTag in productTags)
            {
                productTag.IsDeleted = true;
            }

            await SaveChangesAsync(cancellationToken);
            await base.DeleteAsync(id, cancellationToken);
        }

        public async Task<List<Product>> GetProductsAsync(List<Guid> ids)
        {
            return DbSet.AsQueryable().Where(d => !d.IsDeleted && ids.Contains(d.Id))
                        .Include(d => d.ProductImages)
                        .ToList();
        }

        // 2 by Type -> Random Material -> random Tag
        public async Task<List<Product>> SuggestionProductsAsync(List<Guid> ids, int quantity)
        {
            var result = new List<Product>();
            Random rdm = new Random();
            var products = DbSet.Where(d => !d.IsDeleted && ids.Contains(d.Id))
                                .ToList();

            var maxProductTypeMatched = products.Where(d => d.ProductTypeId.HasValue)
                                                .GroupBy(d => d.ProductTypeId.Value)
                                                .Select(d => new { d.Key, Count = d.Count() })
                                                .OrderByDescending(d => d.Count)
                                                .FirstOrDefault();
            if (maxProductTypeMatched != null)
            {
                var relatedProductsByProductType = DbSet.Where(d => d.ProductTypeId == maxProductTypeMatched.Key).ToList();

                if (relatedProductsByProductType.Any())
                {
                    result.AddRange(relatedProductsByProductType.Take(quantity));
                }
            }


            if (result.Count < quantity)
            {
                var productTypeIds = products.Select(d => d.ProductTypeId).Where(d => d.HasValue).Select(d => d.Value as Guid?).ToList();
                var productByTypes = _context.Products.Where(d => !d.IsDeleted && productTypeIds.Contains(d.ProductTypeId) && !ids.Contains(d.Id))
                                                                     .ToList();
                int take = rdm.Next(productByTypes.Count);
                if (take > 0)
                {
                    for (int i = 0; i < take; i++)
                    {
                        var randomProduct = productByTypes[rdm.Next(take)];
                        if (!result.Any(d => d.Id == randomProduct.Id))
                        {
                            result.Add(randomProduct);
                        }
                    }
                }
            }

            if (result.Count < quantity)
            {
                var materialIds = _context.ProductMaterials.Where(d => !d.IsDeleted && ids.Contains(d.ProductId))
                                                                .Select(d => d.MaterialId)
                                                                .ToList();
                var productIdsByMaterials = _context.ProductMaterials.Where(d => !d.IsDeleted && materialIds.Contains(d.MaterialId))
                                                                     .Select(d => d.ProductId)
                                                                     .ToList();
                var productByMaterials = _context.Products.Where(d => !d.IsDeleted && productIdsByMaterials.Contains(d.Id) && !ids.Contains(d.Id)).ToList();
                int take = rdm.Next(productByMaterials.Count);
                if (take > 0)
                {
                    for (int i = 0; i < take; i++)
                    {
                        var randomProduct = productByMaterials[rdm.Next(take)];
                        if (!result.Any(d => d.Id == randomProduct.Id))
                        {
                            result.Add(randomProduct);
                        }
                    }
                }
            }

            if (result.Count < quantity)
            {
                var tagIds = _context.ProductTags.Where(d => !d.IsDeleted && ids.Contains(d.ProductId))
                                                                .Select(d => d.TagId)
                                                                .ToList();
                var productIdsByTags = _context.ProductTags.Where(d => !d.IsDeleted && tagIds.Contains(d.TagId))
                                                                     .Select(d => d.ProductId)
                                                                     .ToList();
                int take = rdm.Next(productIdsByTags.Count);
                if (take > 0)
                {
                    var productByTags = _context.Products.Where(d => !d.IsDeleted && productIdsByTags.Contains(d.Id) && !ids.Contains(d.Id)).ToList();
                    for (int i = 0; i < take; i++)
                    {
                        var randomProduct = productByTags[rdm.Next(take)];
                        if (!result.Any(d => d.Id == randomProduct.Id))
                        {
                            result.Add(randomProduct);
                        }
                    }
                }
            }
            result = result.Where(d => !ids.Contains(d.Id)).DistinctBy(d => d.Id).ToList();
            var productIds = result.Select(d => d.Id).ToList();
            var productIdsNulable = result.Select(d => d.Id as Guid?).ToList();
            var productMaterials = _context.ProductMaterials.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).ToList();
            var productSizes = _context.ProductSizes.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).Include(d => d.Size).ToList();
            var productTags = _context.ProductTags.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId)).Include(d => d.Tag).ToList();
            var productTypes = _context.ProductTypes.ToList();
            var productImages = _context.ProductImages.Where(d => !d.IsDeleted && productIdsNulable.Contains(d.ProductId)).ToList();
            foreach (var product in result)
            {
                product.ProductImages = productImages.Where(d => d.ProductId == product.Id).ToList();
                product.ProductMaterials = productMaterials.Where(d => d.ProductId == product.Id).ToList();
                product.ProductSizes = productSizes.Where(d => d.ProductId == product.Id).ToList();
                product.ProductTags = productTags.Where(d => d.ProductId == product.Id).ToList();
                product.ProductType = productTypes.FirstOrDefault(d => d.Id == product.ProductTypeId);
            }
            

            return result.Take(quantity).OrderBy(d => rdm.Next(-1, 1)).ToList();
        }
    }
}
