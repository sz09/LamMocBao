using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models.Identify;
using System.Collections.Generic;

namespace Services.Services
{
    public class RoleService : Service<Role>, IService<Role>, IRoleService
    {
        public RoleService(IDbContext context, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(context, _cache, cachingLoaders)
        {
        }
    }
}
