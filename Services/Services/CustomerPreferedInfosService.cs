using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System.Collections.Generic;

namespace Services.Services
{
    public class CustomerPreferedInfosService : Service<CustomerPreferedInfos>, IService<CustomerPreferedInfos>, ICustomerPreferedInfosService
    {
        public CustomerPreferedInfosService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders) { }
    }
}
