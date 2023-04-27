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
    public class TagService : Service<Tag>, IService<Tag>, ITagService
    {
        private readonly IServiceConfig _serviceConfig;
        public TagService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IServiceConfig serviceConfig) : base(dbContext, _cache, cachingLoaders)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<List<IdLabel>> GetComboboxListAsync()
        {
            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
            return query.OrderBy(d => d.Name).Select(d => new IdLabel
            {
                Id = d.Id,
                Label = d.Name
            }).ToList();
        }

        public async Task<List<Tag>> GetFullTagsAsync()
        {
            Func<List<Tag>> getListFunc = () =>
            {
                var dbContext = _serviceConfig.ResolveDbContext(_memoryCache); // Resolve problem when await list tasks to more performance
                return dbContext.SetOf<Tag>().AsQueryable()
                               .Where(d => !d.IsDeleted)
                               .ToList();
            };
            return _memoryCache.TryGet1Async(_memoryCache.GetBulkKey<Tag>(), getListFunc);
        }

        public new SearchResult<Tag> Search(SearchQuery<Tag> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Name.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.Label.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            return base.Search(searchQuery, eagerLoadings);
        }

    }
}
