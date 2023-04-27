using Newtonsoft.Json;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CategoryService : Service<Category>, IService<Category>, ICategoryService
    {
        private readonly IServiceConfig _serviceConfig;
        public CategoryService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IServiceConfig serviceConfig) : base(dbContext, _cache, cachingLoaders)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<List<Category>> GetAssignableToProductComboboxListAsync()
        {
            return DbSet.AsQueryable()
                             .Where(d => !d.IsDeleted && d.AssignableToProduct)
                             .ToList();
        }

        public async Task<List<Category>> GetFilterCategoriesAsync()
        {
            Func<List<Category>> getListFunc = () =>
            {
                var dbContext = _serviceConfig.ResolveDbContext(_memoryCache); // Resolve problem when await list tasks to more performance
                return dbContext.SetOf<Category>().AsQueryable()
                               .Where(d => !d.IsDeleted)
                               .OrderBy(d => d.HomePageSequenceNumber)
                               .ToList();
            };
            return _memoryCache.TryGet1Async($"{_memoryCache.GetBulkKey<Category>()}_GetAll", getListFunc);
        }

        public async Task<List<Category>> GetHomePageDisplayCategoriesAsync()
        {
            Func<List<Category>> getListFunc = () =>
            {
                var query = DbSet.AsQueryable().Where(d => !d.IsDeleted && d.ShowOnHomePage);
                return query.OrderBy(d => d.HomePageSequenceNumber).ToList();
            };

            return _memoryCache.TryGet1Async($"{_memoryCache.GetBulkKey<Category>()}_HomePage", getListFunc);
        }

        public async Task<List<ProductFilterByFengShui>> GetShowOnHomepageFengshuisAsync()
        {
            return DbSet.Where(d => d.OriginalType.HasValue).Select(d => new ProductFilterByFengShui
            {
                Id = d.Id,
                OriginalType = d.OriginalType.Value,
                Name = d.Name
            })
            .OrderBy(d => d.OriginalType)
            .ToList();
        }

        public new SearchResult<Category> Search(SearchQuery<Category> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Name.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            searchQuery.OrderBy = NameCollector<Category>.Get(d => d.HomePageSequenceNumber);
            return base.Search(searchQuery, eagerLoadings);
        }

        public new async Task UpdateAsync(Guid id, Action<Category> updateAction, CancellationToken cancellationToken = default)
        {
            var current = await LoadAsync(id);
            if (current != null)
            {
                var current1 = JsonConvert.DeserializeObject<Category>(JsonConvert.SerializeObject(current)); // Copy to new inmemory area
                updateAction(current1);
                if (current.ShowOnHomePage)
                {
                    var swapHomePageSequenceNumber = await FindAsync(d => d.HomePageSequenceNumber == current1.HomePageSequenceNumber);
                    if (swapHomePageSequenceNumber != null && swapHomePageSequenceNumber.HomePageSequenceNumber != 0)
                    {
                        swapHomePageSequenceNumber.HomePageSequenceNumber = current.HomePageSequenceNumber; // Swap to existing
                        await SaveChangesAsync(cancellationToken);
                    }
                }
                if (current.ShowOnFilter)
                {
                    var swapFilterSequenceNumber = await FindAsync(d => d.FilterSequenceNumber == current1.FilterSequenceNumber);
                    if (swapFilterSequenceNumber != null)
                    {
                        swapFilterSequenceNumber.FilterSequenceNumber = current.FilterSequenceNumber; // Swap to existing
                        await SaveChangesAsync(cancellationToken);
                    }
                }
            }
            _memoryCache.Invalidate<Category>();
            await base.UpdateAsync(id, updateAction, cancellationToken);
        }
    }
}
