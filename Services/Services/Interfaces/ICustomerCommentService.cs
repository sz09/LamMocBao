using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface ICustomerCommentService : IService<CustomerComment>
    {
        Task<List<CustomerComment>> GetListAsync();
        new SearchResult<CustomerComment> Search(SearchQuery<CustomerComment> searchQuery, EagerLoadings eagerLoadings = null);
        new Task UpdateAsync(Guid id, Action<CustomerComment> updateAction, CancellationToken cancellationToken = default);
    }
}
