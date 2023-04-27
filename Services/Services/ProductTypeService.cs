using Newtonsoft.Json;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Services.Utiltities;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Services.ModelResult;

namespace Services.Services
{
    public class ProductTypeService : Service<ProductType>, IService<ProductType>, IProductTypeService
    {
        private readonly IServiceConfig _serviceConfig;
        public ProductTypeService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IServiceConfig serviceConfig) : base(dbContext, _cache, cachingLoaders)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<List<ProductTypeTagModel>> GetProductTypeTagModelsAsync()
        {
            return DbSet.AsQueryable().Where(d => !d.IsDeleted)
                 .Include(d => d.ProductTypeTags.Where(d => !d.IsDeleted))
                 .ThenInclude(d => d.Tag)
                 .Select(d => new ProductTypeTagModel
                 {
                     Id = d.Id,
                     Tags = d.ProductTypeTags.Select(d => d.Tag).ToList()
                 }).ToList();
        }

        public async Task<List<ProductType>> GetDisplayProductTypesAsync()
        {
            Func<List<ProductType>> getListFunc = () =>
            {
                var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
                return query.OrderBy(d => d.SequenceNumber).ToList();
            };

            return _memoryCache.TryGet1Async($"{_memoryCache.GetBulkKey<ProductType>()}_HomePage", getListFunc);
        }

        public async Task<List<IdLabel>> GetComboboxListAsync()
        {
            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
            return query.OrderBy(d => d.Name).Select(d => new IdLabel
            {
                Id = d.Id,
                Label = d.Name,
                ExtraInfos = d.LinkName
            }).ToList();
        }

        public new SearchResult<ProductType> Search(SearchQuery<ProductType> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Name.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            searchQuery.OrderBy = NameCollector<ProductType>.Get(d => d.SequenceNumber);

            return base.Search(searchQuery, eagerLoadings);
        }

        public new async Task UpdateAsync(Guid id, Action<ProductType> updateAction, CancellationToken cancellationToken = default)
        {
            var current = await LoadAsync(id);
            if (current != null)
            {
                var current1 = JsonConvert.DeserializeObject<ProductType>(JsonConvert.SerializeObject(current)); // Copy to new inmemory area
                updateAction(current1); // Find swap to new state
                var swapSequenceNumber = await FindAsync(d => d.SequenceNumber == current1.SequenceNumber);
                if (swapSequenceNumber != null)
                {
                    swapSequenceNumber.SequenceNumber = current.SequenceNumber; // Swap to existing
                    await SaveChangesAsync(cancellationToken);
                }
            }
            await base.UpdateAsync(id, updateAction, cancellationToken);
        }

        public async Task<List<ProductType>> GetFullProductTypesAsync()
        {
            Func<List<ProductType>> getListFunc = () =>
            {
                var dbContext = _serviceConfig.ResolveDbContext(_memoryCache); // Resolve problem when await list tasks to more performance
                return dbContext.SetOf<ProductType>().AsQueryable()
                               .Where(d => !d.IsDeleted)
                               .OrderBy(d => d.SequenceNumber)
                               .Include(d => d.ProductTypeTags)
                               .ThenInclude(d => d.Tag)
                               .ToList();
            };
            return _memoryCache.TryGet1Async(_memoryCache.GetBulkKey<ProductType>(), getListFunc);
        }
    }
}
