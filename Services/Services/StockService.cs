using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductStock
    {
        public string ProductName { get; set; }
        public Guid ProductId { get; set; }
        public List<ProductSizeStock> ProductSizeStocks { get; set; }
    }

    public class ProductSizeStock
    {
        public Guid ProductSizeId { get; set; }
        public string DisplayUnit { get; set; }
        public int CurrentQuantity { get; set; }
    }

    public class StockService : Service<Stock>, IService<Stock>, IStockService
    {
        public StockService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public async Task<int> ExportAsync(Guid productId, Guid productSizeId, int quantity)
        {
            var stock = DbSet.FirstOrDefault(d => d.ProductId == productId && d.ProductSizeId == productSizeId);
            if (stock != null)
            {
                if (stock.Quantity >= quantity)
                {
                    stock.Quantity -= quantity;
                    await SaveChangesAsync(default);
                    return stock.Quantity;
                }

                return stock.Quantity;

            }

            await AddAsync(new Stock
            {
                ProductId = productId,
                ProductSizeId = productSizeId,
                Quantity = 0
            });

            return 0;
        }

        public async Task<List<ProductSizeStock>> GetQuantityAsync(Guid productId)
        {
           return DbSet.Where(d => d.ProductId == productId)
                       .Select(d => new ProductSizeStock
                       {
                           ProductSizeId = d.ProductSizeId,
                           CurrentQuantity = d.Quantity
                       })
                       .ToList();
        }

        public async Task<int> ImportAsync(Guid productId, Guid productSizeId, int quantity)
        {
            var stock = DbSet.FirstOrDefault(d => d.ProductId == productId && d.ProductSizeId == productSizeId);
            if (stock != null)
            {
                stock.Quantity += quantity;
                await SaveChangesAsync(default);
                return stock.Quantity;
            }

            await AddAsync(new Stock
            {
                ProductId = productId,
                ProductSizeId = productSizeId,
                Quantity = quantity
            });

            return quantity;
        }

        public SearchResult<ProductStock> Search(SearchQuery<ProductStock> searchQuery)
        {
            var query = _context.Products.Where(d => !d.IsDeleted)
                                              .OrderBy(d => d.Name)
                                              .Skip(searchQuery.Skip)
                                              .Take(searchQuery.PageSize)
                                              .Select(d => new Product
                                              {
                                                  Name = d.Name,
                                                  Id = d.Id
                                              });

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                query = query.Where(d => d.Name.Contains(searchQuery.Search));
            }
            var products = query.ToList();
            var productIds = products.Select(d => d.Id).ToList();
            var productSizes = _context.ProductSizes.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId))
                                                    .Include(d => d.Size)
                                                    .ToList();
            var productStocks = _context.Stocks.Where(d => !d.IsDeleted && productIds.Contains(d.ProductId))
                                               .ToList();
            var total = 0;
            if (searchQuery.IncludeTotal)
            {
                total = _context.Products.Where(d => !d.IsDeleted).Count();
            }

            var data = products.Select(product => {
                var productSizeStocks = productSizes.Where(d => d.ProductId == product.Id);
                var productSizeQuantities = productStocks.Select(d => new { d.ProductId, d.Quantity, d.ProductSizeId }).ToList();
                return new ProductStock
                {
                    ProductName = product?.Name,
                    ProductId = product.Id,
                    ProductSizeStocks = productSizeStocks.Select(d => new ProductSizeStock
                    {
                        DisplayUnit = $"{d.Size.Number} {d.Size.Unit}",
                        ProductSizeId = d.Id,
                        CurrentQuantity = productSizeQuantities.FirstOrDefault(s => s.ProductId == d.ProductId && s.ProductSizeId == d.Id)?.Quantity ?? 0
                    }).ToList()
                };
            }).ToList();
            return new SearchResult<ProductStock>
            {
                Data = data,
                Total = total
            };
        }
    }
}
