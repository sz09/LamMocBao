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
    public class OrderService : Service<Order>, IService<Order>, IOrderService
    {
        private readonly IStockService _stockService;
        public OrderService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IStockService stockService) : base(dbContext, _cache, cachingLoaders)
        {
            _stockService = stockService;
        }

        public async Task ExportStocksAsync(Guid id)
        {
            var order = DbSet.FirstOrDefault(d => d.Id == id);
            if (order != null)
            {
                var oderDetailWithCounts = _context.OrderDetails.Where(d => d.OrderId == id).Select(d => new
                {
                    d.ProductId,
                    d.Quantity,
                    d.SizeId
                }).ToList();

                foreach (var item in oderDetailWithCounts)
                {
                    var stock = _context.Stocks.FirstOrDefault(d => d.ProductSizeId == item.SizeId && d.ProductId == item.ProductId);
                    if (stock != null)
                    {
                        stock.Quantity -= Math.Min(item.Quantity, stock.Quantity);
                    }
                }

                await SaveChangesAsync(default);
            }
        }

        public async Task ImportStocksAsync(Guid id)
        {
            var order = DbSet.FirstOrDefault(d => d.Id == id);
            if (order != null)
            {
                var oderDetailWithCounts = _context.OrderDetails.Where(d => d.OrderId == id).Select(d => new
                {
                    d.ProductId,
                    d.Quantity,
                    d.SizeId
                }).ToList();

                foreach (var item in oderDetailWithCounts)
                {
                    var stock = _context.Stocks.FirstOrDefault(d => d.ProductSizeId == item.SizeId && d.ProductId == item.ProductId);
                    if (stock != null)
                    {
                        stock.Quantity += Math.Min(item.Quantity, stock.Quantity);
                    }
                }

                await SaveChangesAsync(default);
            }
        }

        public async Task MarkOrderSuccessAsync(Guid id)
        {
            var order = DbSet.FirstOrDefault(d => d.Id == id);

            if (order != null && order.Status == OrderStatus.Ordered)
            {
                var oderDetailWithCounts = _context.OrderDetails.Where(d => d.OrderId == id).Select(d => new
                {
                    d.ProductId,
                    d.Quantity
                }).ToList().GroupBy(d => d.ProductId);

                foreach (var item in oderDetailWithCounts)
                {
                    var product = _context.Products.FirstOrDefault(d => d.Id == item.Key);
                    product.SoldNumberCount += item.Sum(d => d.Quantity);
                }

                await _context.SaveAsync();
            }

        }

        public SearchResult<Order> Search(SearchQuery<Order> searchQuery)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Customer.FullName.Contains(searchQuery.Search, StringComparison.CurrentCultureIgnoreCase) ||
                                                    entity.Address.Contains(searchQuery.Search, StringComparison.CurrentCultureIgnoreCase) ||
                                                    entity.District.Contains(searchQuery.Search, StringComparison.CurrentCultureIgnoreCase) ||
                                                    entity.Ward.Contains(searchQuery.Search, StringComparison.CurrentCultureIgnoreCase) ||
                                                    entity.Note.Contains(searchQuery.Search, StringComparison.CurrentCultureIgnoreCase) ||
                                                    entity.Province.Contains(searchQuery.Search, StringComparison.CurrentCultureIgnoreCase);
            }

            searchQuery.OrderBy = NameCollector<Order>.Get(d => d.CreatedAt);

            var query = DbSet.AsQueryable()
                            .Where(d => !d.IsDeleted)
                            .Include(d => d.OrderDetails)
                            .ThenInclude(d => d.Product)
                            .Include(d => d.Customer);

            return base.Search(query, searchQuery);
        }
    }
}
