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
    public class ProductCategoryService : Service<ProductCategory>, IService<ProductCategory>, IProductCategoryService
    {
        public ProductCategoryService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }
    }
}
