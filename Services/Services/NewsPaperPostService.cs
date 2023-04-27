using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class NewsPaperPostService : Service<NewsPaperPost>, IService<NewsPaperPost>, INewsPaperPostService
    {
        public NewsPaperPostService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public async Task<List<NewsPaperPost>> GetListAsync()
        {
            var query = DbSet.AsQueryable().Include(d => d.UploadedImage).Where(d => !d.IsDeleted);
            return query.OrderBy(d => d.SequenceNumber).ToList();
        }


        public new SearchResult<NewsPaperPost> Search(SearchQuery<NewsPaperPost> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Hint.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.Link.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            searchQuery.OrderBy = NameCollector<ProductType>.Get(d => d.SequenceNumber);
            eagerLoadings = new EagerLoadings
            {
                NavigationCollectionNames = new List<string>() {
                    NameCollector<NewsPaperPost>.Get(d => d.UploadedImage)
                }
            };
            return base.Search(searchQuery, eagerLoadings);
        }

        public new async Task UpdateAsync(Guid id, Action<NewsPaperPost> updateAction, CancellationToken cancellationToken = default)
        {
            var current = await LoadAsync(id);
            if (current != null)
            {
                var current1 = JsonConvert.DeserializeObject<NewsPaperPost>(JsonConvert.SerializeObject(current)); // Copy to new inmemory area
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
