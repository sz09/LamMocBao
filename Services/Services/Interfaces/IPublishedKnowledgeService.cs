using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IPublishedKnowledgeService : IService<PublishedKnowledge>
    {
        Task<PublishedKnowledge> LoadByLinkName(string linkName);
        Task<List<PublishedKnowledge>> GetKnowledgesAsync(List<Guid> ids);
        Task<Dictionary<Guid, Guid?>> GetKnowledgesBySourceAsync(List<Guid> ids);
        Task<List<PublishedKnowledge>> GetHomepageRawItemsAsync();
        new SearchResult<PublishedKnowledge> Search(SearchQuery<PublishedKnowledge> searchQuery, EagerLoadings eagerLoadings = null);
        new Task UpdateAsync(Guid id, Action<PublishedKnowledge> updateAction, CancellationToken cancellationToken = default);
        Task<List<PublishedKnowledge>> GetSuggestKnowledgesAsync(int quantity);
    }
}
