using Services.ModelResult;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductTypeService : IService<ProductType>
    {
        Task<List<ProductTypeTagModel>> GetProductTypeTagModelsAsync();
        Task<List<IdLabel>> GetComboboxListAsync();
        Task<List<ProductType>> GetDisplayProductTypesAsync();
        Task<List<ProductType>> GetFullProductTypesAsync();
        new SearchResult<ProductType> Search(SearchQuery<ProductType> searchQuery, EagerLoadings eagerLoadings = null);
        new Task UpdateAsync(Guid id, Action<ProductType> updateAction, CancellationToken cancellationToken = default);
    }
}
