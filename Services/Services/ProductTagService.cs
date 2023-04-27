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
    public class ProductTagService : Service<ProductTag>, IService<ProductTag>, IProductTagService
    {
        public ProductTagService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }

        public async Task CleanTagsAsync(Guid productId)
        {
            foreach (var item in DbSet.Where(d => d.ProductId == productId).ToList())
            {
                DbSet.Remove(item);
            }

            await SaveChangesAsync(default);
        }
    }
}
