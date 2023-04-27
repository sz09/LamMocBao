using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class HighlightService : Service<HighlightItem>, IService<HighlightItem>, IHighlightService
    {
        private readonly object @object = new object();
        private readonly IServiceConfig _serviceConfig;
        public HighlightService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IServiceConfig serviceConfig) : base(dbContext, _cache, cachingLoaders)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<List<Guid>> HasCurrentItemHighlightAsync(List<Guid> entityIds, EntityType entityType)
        {
            var from = DateTime.UtcNow.StartOfDate();
            var to = DateTime.UtcNow.AddDays(_serviceConfig.AutoHighlightItemsInDays).EndOfDate();
            return DbSet.Where(d => d.EntityType == entityType && entityIds.Contains(d.EntityId) && d.From >= from && d.To <= to)
                        .Select(d => d.EntityId)
                        .ToList();
        }

        public async Task MakeHighlightAsync(Guid entityId, EntityType entityType, DateTime from, DateTime to)
        {
            var itemInRange = DbSet.Where(d => d.EntityType == entityType && d.EntityId == entityId)
                                   .FirstOrDefault();
            if (itemInRange != null)
            {
                await DeleteAsync(itemInRange.Id);
            }
            else
            {
                DbSet.Where(d => d.EntityType == entityType && d.EntityId == entityId).DeleteFromQuery();
                await AddAsync(new HighlightItem
                {
                    Id = GuidGenerator.NewGuid(),
                    EntityId = entityId,
                    EntityType = entityType,
                    From = from,
                    To = to
                });
            }
        }
    }
}
