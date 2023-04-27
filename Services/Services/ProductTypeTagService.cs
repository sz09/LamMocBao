using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Services.ModelResult;
using Services.Services.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductTypeTagService : Service<ProductTypeTag>, IService<ProductTypeTag>, IProductTypeTagService
    {
        public ProductTypeTagService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }
    }
}
