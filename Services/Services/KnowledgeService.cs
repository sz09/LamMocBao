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
    public class KnowledgeService : Service<Knowledge>, IService<Knowledge>, IKnowledgeService
    {
        public KnowledgeService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public async Task<List<Knowledge>> GetListAsync()
        {
            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted).Include(d => d.UploadedImage);
            return query.OrderBy(d => d.SequenceNumber).ToList();
        }

        public async Task<Knowledge> LoadByLinkName(string linkName)
        {
            var @object = DbSet.Where(d => !d.IsDeleted).FirstOrDefault(d => d.LinkName == linkName);
            if (@object != null)
            {
                @object.UploadedImage = _context.UploadedImages.FirstOrDefault(d => !d.IsDeleted && d.EntityId == @object.Id);
            }

            return @object;
        }

        public new SearchResult<Knowledge> Search(SearchQuery<Knowledge> searchQuery, EagerLoadings eagerLoadings = null)
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

            searchQuery.OrderBy = NameCollector<Knowledge>.Get(d => d.SequenceNumber);

            return base.Search(searchQuery, eagerLoadings);
        }

        public new async Task UpdateAsync(Guid id, Action<Knowledge> updateAction, CancellationToken cancellationToken = default)
        {
            var current = await LoadAsync(id);
            if (current != null)
            {
                var current1 = JsonConvert.DeserializeObject<Knowledge>(JsonConvert.SerializeObject(current)); // Copy to new inmemory area
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
    }
}
