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
    public class ProductSizeService : Service<ProductSize>, IService<ProductSize>, IProductSizeService
    {
        public ProductSizeService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }

        public async Task<List<ProductSize>> GetSizesAsync(List<Guid> ids)
        {
            return DbSet.Where(d => !d.IsDeleted && ids.Contains(d.Id)).Include(d => d.Size).ToList();
        }
    }
}
