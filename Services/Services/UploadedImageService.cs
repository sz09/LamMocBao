using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System.Collections.Generic;

namespace Services.Services
{
    public class UploadedImageService : Service<UploadedImage>, IService<UploadedImage>, IUploadedImageService
    {
        public UploadedImageService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }
    }
}
