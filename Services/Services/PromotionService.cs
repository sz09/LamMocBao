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
    public class PromotionService : Service<Promotion>, IService<Promotion>, IPromotionService
    {
        private readonly IServiceConfig _serviceConfig;
        public PromotionService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IServiceConfig serviceConfig) : base(dbContext, _cache, cachingLoaders)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<PromotionInfo> GetByCode(string code)
        {
            var query = DbSet.AsQueryable().Where(d => d.IsActive && !d.IsDeleted);
            var promotions = query.Where(d => d.Code == code).ToList();
            if (!promotions.Any()) 
            { 
                return null;
            }

            var promotion = promotions.First();
            if (promotion.PromotionMode == PromotionMode.Period)
            {
                if (promotion.From.HasValue && DateTime.UtcNow < promotion.From.Value.ToUniversalTime() ||
                    promotion.To.HasValue && DateTime.UtcNow > promotion.To.Value.ToUniversalTime())
                {
                    return new PromotionInfo { IsExpired = true };
                }
            }

            return new PromotionInfo { Promotion = promotion };
        }

        public async Task<List<IdLabel>> GetComboboxListAsync()
        {
            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
            return query.OrderBy(d => d.Code).Select(d => new IdLabel
            {
                Id = d.Id,
                Label = d.Code
            }).ToList();
        }

        public async Task<List<Promotion>> GetFullPromotionsAsync()
        {
            Func<List<Promotion>> getListFunc = () =>
            {
                var dbContext = _serviceConfig.ResolveDbContext(_memoryCache); // Resolve problem when await list tasks to more performance
                return dbContext.SetOf<Promotion>().AsQueryable()
                               .Where(d => !d.IsDeleted)
                               .ToList();
            };
            return _memoryCache.TryGet1Async(_memoryCache.GetBulkKey<Promotion>(), getListFunc);
        }

        public new SearchResult<Promotion> Search(SearchQuery<Promotion> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Code.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.Content.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            return base.Search(searchQuery, eagerLoadings);
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var existing = await LoadAsync(id);
            existing.IsDeleted = true;
            await SaveChangesAsync(default);
        }
    }
}
