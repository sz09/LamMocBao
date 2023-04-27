using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IStockService : IService<Stock>
    {
        SearchResult<ProductStock> Search(SearchQuery<ProductStock> searchQuery);
        Task<int> ImportAsync(Guid productId, Guid productSizeId, int quantity);
        Task<int> ExportAsync(Guid productId, Guid productSizeId, int quantity);
        Task<List<ProductSizeStock>> GetQuantityAsync(Guid productId);
    }
}
