using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface INewsPaperPostService : IService<NewsPaperPost>
    {
        Task<List<NewsPaperPost>> GetListAsync();
        new SearchResult<NewsPaperPost> Search(SearchQuery<NewsPaperPost> searchQuery, EagerLoadings eagerLoadings = null);
        new Task UpdateAsync(Guid id, Action<NewsPaperPost> updateAction, CancellationToken cancellationToken = default);
    }
}
