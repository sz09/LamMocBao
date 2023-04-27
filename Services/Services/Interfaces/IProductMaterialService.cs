using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductMaterialService : IService<ProductMaterial>
    {
        Task<List<ProductMaterial>> GetMaterialsAsync(List<Guid> ids);
    }
}
