using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderDetailService : Service<OrderDetail>, IService<OrderDetail>, IOrderDetailService
    {
        public OrderDetailService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }

        public async Task<List<OrderDetail>> GetByOrderIdAsync(Guid orderId)
        {
            var orderDetails = DbSet.Where(x => !x.IsDeleted && x.OrderId == orderId)
                       .Include(d => d.Product)
                       .ToList();
            var productSizeIds = orderDetails.Select(d => d.SizeId).ToList();
            var productSizes = _context.ProductSizes.Where(d => productSizeIds.Contains(d.Id)).Include(d => d.Size).ToList();
            foreach (var orderDetail in orderDetails)
            {
                orderDetail.ProductSize = productSizes.FirstOrDefault(d => d.Id == orderDetail.SizeId);
            }

            return orderDetails;
        }
    }
}
