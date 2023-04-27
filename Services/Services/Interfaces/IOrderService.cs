using Shared.Models;
using Shared.Models.Identify;
using Shared.Utilities;
using System;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IOrderService : IService<Order>
    {
        SearchResult<Order> Search(SearchQuery<Order> searchQuery);
        Task MarkOrderSuccessAsync(Guid id);
        Task ExportStocksAsync(Guid id);
        Task ImportStocksAsync(Guid id);
    }
}
