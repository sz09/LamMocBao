using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SystemSettingService : Service<SystemSetting>, IService<SystemSetting>, ISystemSettingService
    {
        public SystemSettingService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public async Task<SystemSetting> LoadAsync()
        {
            return _memoryCache.TryGet1Async(_memoryCache.GetBulkKey<SystemSetting>(), () => DbSet.FirstOrDefault());
        }
    }
}
