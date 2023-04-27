using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductImageService : IService<ProductImage>
    {
        Task<bool> DelteAsync(List<Guid> ids);
    }
}
