using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MaterialService : Service<Material>, IService<Material>, IMaterialService
    {
        public MaterialService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public async Task<List<IdLabel>> GetComboboxListAsync()
        {
            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);
            return query.ToList().Select(d => new IdLabel
            {
                Id = d.Id,
                Label = d.Name  
            }).ToList();
        }

        public new SearchResult<Material> Search(SearchQuery<Material> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.Name.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.Description.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }

            return base.Search(searchQuery, eagerLoadings);
        }

        public async Task<List<Material>> GetMaterialsAsync(List<Guid> ids)
        {
            return DbSet.AsQueryable().Where(d => !d.IsDeleted && ids.Contains(d.Id))
                        .ToList();
        }

    }
}
