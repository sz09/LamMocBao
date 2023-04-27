using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductService : IService<Product>
    {
        Task<List<Product>> GetProductsAsync(List<Guid> ids);
        Task<Product> LoadByLinkName(string name);
        List<Guid> LoadAvailableProductIds(List<Guid> ids);
        Task<List<Product>> SuggestionProductsAsync(List<Guid> ids, int quantity);
        SearchResult<Product> Search(SearchQuery<Product> searchQuery, ProductFilter filter = null);
        new Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
