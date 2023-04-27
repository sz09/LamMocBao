using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IKnowledgeService : IService<Knowledge>
    {
        Task<Knowledge> LoadByLinkName(string linkName);
        Task<List<Knowledge>> GetListAsync();
        new SearchResult<Knowledge> Search(SearchQuery<Knowledge> searchQuery, EagerLoadings eagerLoadings = null);
        new Task UpdateAsync(Guid id, Action<Knowledge> updateAction, CancellationToken cancellationToken = default);
    }
}
