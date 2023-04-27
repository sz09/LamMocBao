using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IPromotionService : IService<Promotion>
    {
        Task SoftDeleteAsync(Guid id);
        Task<List<IdLabel>> GetComboboxListAsync();
        Task<List<Promotion>> GetFullPromotionsAsync();
        new SearchResult<Promotion> Search(SearchQuery<Promotion> searchQuery, EagerLoadings eagerLoadings = null);
        Task<PromotionInfo> GetByCode(string code);
    }
}
