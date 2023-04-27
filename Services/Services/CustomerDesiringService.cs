using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Services
{
    public class CustomerDesiringService : Service<CustomerDesiring>, IService<CustomerDesiring>, ICustomerDesiringService
    {
        public CustomerDesiringService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders) { }

        public SearchResult<CustomerDesiring> SearchInfo(SearchQuery<CustomerDesiring> searchQuery, EagerLoadings eagerLoadings = null)
        {
            if (searchQuery == null)
            {
                EnsureDefault(searchQuery);
            }

            if (!string.IsNullOrEmpty(searchQuery.Search))
            {
                searchQuery.SearchFunc = (entity) => entity.CustomerName.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.CustomerPhoneNumber.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase) ||
                                                     entity.CustomerEmail.Contains(searchQuery.Search, StringComparison.OrdinalIgnoreCase);
            }
            searchQuery.OrderBy = searchQuery.OrderBy ?? "CustomerName";
            return Search(searchQuery, eagerLoadings);
        }

        public List<CustomerDesiring> GetCustomerPrefered(string email, string phoneNumber, string name)
        {
            return DbSet.Include(d => d.CustomerPrefereds)
                        .ThenInclude(d => d.PreferedProduct)
                        .Where(d => !d.IsDeleted)
                        .Where(d => d.CustomerPrefereds.Any(s => !s.IsDeleted && !s.PreferedProduct.IsDeleted))
                        .Where(d => (!string.IsNullOrEmpty(email) && d.CustomerEmail == email) || d.CustomerPhoneNumber == phoneNumber || d.CustomerName == name)
                        .ToList();
        }

        public List<CustomerDesiring> GetCustomerPrefered(string email, string phoneNumber, string name, Guid ignoreId)
        {
            return DbSet.Include(d => d.CustomerPrefereds)
                        .ThenInclude(d => d.PreferedProduct)
                        .Where(d => !d.IsDeleted && d.Id != ignoreId)
                        .Where(d => d.CustomerPrefereds.Any(s => !s.IsDeleted && !s.PreferedProduct.IsDeleted))
                        .Where(d => (!string.IsNullOrEmpty(email) && d.CustomerEmail == email) || d.CustomerPhoneNumber == phoneNumber || d.CustomerName == name)
                        .ToList();
        }
    }
}
