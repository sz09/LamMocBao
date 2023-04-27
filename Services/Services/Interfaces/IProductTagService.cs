using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IProductTagService : IService<ProductTag>
    {
        Task CleanTagsAsync(Guid productId);
    }
}
