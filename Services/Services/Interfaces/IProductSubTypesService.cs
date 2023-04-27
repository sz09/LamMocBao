using Shared.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductSubTypeService : IService<ProductSubType>
    {
        Task UpdateSubTypesAsync(Guid productTypeId, string subTypes, CancellationToken cancellationToken = default);
    }
}
