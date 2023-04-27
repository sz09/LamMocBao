using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductSizeService : IService<ProductSize>
    {
        Task<List<ProductSize>> GetSizesAsync(List<Guid> ids);
    }
}
