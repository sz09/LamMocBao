using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IOrderDetailService : IService<OrderDetail>
    {
        Task<List<OrderDetail>> GetByOrderIdAsync(Guid orderId);
    }
}
