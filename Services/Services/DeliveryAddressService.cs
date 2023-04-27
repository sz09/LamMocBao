using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System.Collections.Generic;

namespace Services.Services
{
    public class DeliveryAddressService : Service<DeliveryAddress>, IService<DeliveryAddress>, IDeliveryAddressService
    {
        public DeliveryAddressService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders)
        {
        }
    }
}
