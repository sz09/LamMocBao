using Newtonsoft.Json;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PublishedKnowledgeService : Service<PublishedKnowledge>, IService<PublishedKnowledge>, IPublishedKnowledgeService
    {
        private readonly IServiceConfig _serviceConfig;
        public PublishedKnowledgeService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IServiceConfig serviceConfig) : base(dbContext, _cache, cachingLoaders)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task<List<PublishedKnowledge>> GetHomepageRawItemsAsync()
        {
            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
            var items = query.OrderBy(d => d.SequenceNumber).Take(_serviceConfig.NumerHomepageKnowledges).Include(d => d.UploadedImage).ToList();
            return items;
        }

        public async Task<List<PublishedKnowledge>> GetSuggestKnowledgesAsync(int quantity)
        {
            var take = _serviceConfig.RandomRatio * quantity;
            var query = DbSet.Where(d => !d.IsDeleted).Take(take).ToList();
            return query.Take(quantity).ToList();
        }

        public async Task<List<PublishedKnowledge>> GetKnowledgesAsync(List<Guid> ids)
        {
            return DbSet.AsQueryable().Where(d => !d.IsDeleted && ids.Contains(d.Id))
                        .ToList();
        }

        public async Task<PublishedKnowledge> LoadByLinkName(string linkName)
        {
            var @object = DbSet.Where(d => !d.IsDeleted).FirstOrDefault(d => d.LinkName == linkName);
            if (@object != null)
            {
                @object.UploadedImage = _context.UploadedImages.FirstOrDefault(d => !d.IsDeleted && d.EntityId == @object.Id);
            }

            return @object;
        }

        public new SearchResult<PublishedKnowledge> Search(SearchQuery<PublishedKnowledge> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Name.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.Content.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            searchQuery.OrderBy = NameCollector<PublishedKnowledge>.Get(d => d.SequenceNumber);
            eagerLoadings = new EagerLoadings
            {
                NavigationValueNames = new List<string>
                {
                    NameCollector<PublishedKnowledge>.Get(d => d.UploadedImage)
                }
            };

            return base.Search(searchQuery, eagerLoadings);
        }

        public new async Task UpdateAsync(Guid id, Action<PublishedKnowledge> updateAction, CancellationToken cancellationToken = default)
        {
            var current = await LoadAsync(id);
            if (current != null)
            {
                var current1 = JsonConvert.DeserializeObject<PublishedKnowledge>(JsonConvert.SerializeObject(current)); // Copy to new inmemory area
                updateAction(current1);
                var swapSequenceNumber = await FindAsync(d => d.SequenceNumber == current1.SequenceNumber);
                if (swapSequenceNumber != null)
                {
                    swapSequenceNumber.SequenceNumber = current.SequenceNumber; // Swap to existing
                    await SaveChangesAsync(cancellationToken);
                }
            }
            await base.UpdateAsync(id, updateAction, cancellationToken);
        }

        public async Task<Dictionary<Guid, Guid?>> GetKnowledgesBySourceAsync(List<Guid> ids)
        {
            var items = DbSet.Where(d => ids.Contains(d.OriginKnowledgeId)).ToList();
            var map = ids.Select(d => new { Source = d, Destinition = items.FirstOrDefault(s => s.OriginKnowledgeId == d)?.Id });
            return map.ToDictionary(d => d.Source, e => e.Destinition);
        }
    }
}
