using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductSubTypeService : Service<ProductSubType>, IService<ProductSubType>, IProductSubTypeService
    {
        public ProductSubTypeService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }

        public async Task UpdateSubTypesAsync(Guid productTypeId, string subTypes, CancellationToken cancellationToken = default)
        {
            _memoryCache.Invalidate<ProductType>(); // Invalidate cache for parents
            var currentList = DbSet.Where(d => !d.IsDeleted && d.ProductTypeId == productTypeId).ToList();
            if (string.IsNullOrEmpty(subTypes)) // Mean delete all
            {
                foreach (var item in currentList)
                {
                    DbSet.Remove(item);
                }

                await SaveChangesAsync(cancellationToken);
            }
            else // Mean can update / delete old one
            {
                var clientSideSubTypeList = subTypes.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()).ToList();
                var currentListName = currentList.Select(d => d.Name);
                var addNewTypes = clientSideSubTypeList.Except(currentListName);
                foreach (var item in addNewTypes)
                {
                    DbSet.Add(new ProductSubType {Id = GuidGenerator.NewGuid(), Name = item, ProductTypeId = productTypeId });
                }

                var removeOldTypes = currentListName.Except(clientSideSubTypeList);
                foreach (var item in removeOldTypes)
                {
                    DbSet.Remove(currentList.First(d => d.Name == item));
                }

                await SaveChangesAsync(cancellationToken);

                // Reorder
                var currentIndex = 0;
                var items = DbSet.Where(d => !d.IsDeleted && d.ProductTypeId == productTypeId).ToList();
                foreach (var item in items.OrderBy(d => clientSideSubTypeList.IndexOf(d.Name)))
                {
                    item.SequenceNumber = ++currentIndex;
                }

                await SaveChangesAsync(cancellationToken);
            }
        }
    }
}
