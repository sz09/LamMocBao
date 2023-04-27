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
    public class ProductMaterialService : Service<ProductMaterial>, IService<ProductMaterial>, IProductMaterialService
    {
        public ProductMaterialService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }

        public async Task<List<ProductMaterial>> GetMaterialsAsync(List<Guid> ids)
        {
            return DbSet.Where(d => !d.IsDeleted && ids.Contains(d.Id)).Include(d => d.Material).ToList();
        }
    }
}
