using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;

namespace Services.Services.Interfaces
{
    public interface ICustomerDesiringService : IService<CustomerDesiring>
    {
        SearchResult<CustomerDesiring> SearchInfo(SearchQuery<CustomerDesiring> searchQuery, EagerLoadings eagerLoadings = null);
        List<CustomerDesiring> GetCustomerPrefered(string email, string phoneNumber, string name);
        List<CustomerDesiring> GetCustomerPrefered(string email, string phoneNumber, string name, Guid ignoreId);
    }
}
