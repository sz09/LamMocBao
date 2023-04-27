using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface ISizeService : IService<Size>
    {
        Task<List<Size>> GetSizesAsync(List<Guid> ids);
        Task<List<IdLabel>> GetComboboxListAsync();
        new SearchResult<Size> Search(SearchQuery<Size> searchQuery, EagerLoadings eagerLoadings = null);
    }
}
