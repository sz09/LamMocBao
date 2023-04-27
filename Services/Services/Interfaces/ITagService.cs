using Shared.Models;
using Shared.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface ITagService : IService<Tag>
    {
        Task<List<IdLabel>> GetComboboxListAsync();
        Task<List<Tag>> GetFullTagsAsync();
        new SearchResult<Tag> Search(SearchQuery<Tag> searchQuery, EagerLoadings eagerLoadings = null);
    }
}
