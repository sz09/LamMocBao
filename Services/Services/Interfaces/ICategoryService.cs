using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface ICategoryService : IService<Category>
    {
        Task<List<Category>> GetAssignableToProductComboboxListAsync();
        Task<List<Category>> GetFilterCategoriesAsync();
        Task<List<Category>> GetHomePageDisplayCategoriesAsync();
        new SearchResult<Category> Search(SearchQuery<Category> searchQuery, EagerLoadings eagerLoadings = null);
        new Task UpdateAsync(Guid id, Action<Category> updateAction, CancellationToken cancellationToken = default);
        Task<List<ProductFilterByFengShui>> GetShowOnHomepageFengshuisAsync();
    }
}
