using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ProductImageService : Service<ProductImage>, IService<ProductImage>, IProductImageService
    {
        private readonly IFileServices _fileServices;
        public ProductImageService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders, IFileServices fileServices) : base(dbContext, _cache, cachingLoaders)
        {
            _fileServices = fileServices;
        }

        public async Task<bool> DelteAsync(List<Guid> ids)
        {
            var tasks = ids.Select(async (id) =>
            {
                var productImage = await LoadAsync(id);
                if(productImage != null)
                    await _fileServices.DeleteAsync(productImage);
                await DeleteAsync(id);
            });
            try
            {
                await Task.WhenAll(tasks);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
