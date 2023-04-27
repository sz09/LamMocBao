using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IMaterialService : IService<Material>
    {
        Task<List<Material>> GetMaterialsAsync(List<Guid> ids);
        Task<List<IdLabel>> GetComboboxListAsync();
        new SearchResult<Material> Search(SearchQuery<Material> searchQuery, EagerLoadings eagerLoadings = null);
    }
}
