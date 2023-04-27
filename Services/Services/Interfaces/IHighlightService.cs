using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IHighlightService : IService<HighlightItem>
    {
        Task<List<Guid>> HasCurrentItemHighlightAsync(List<Guid> entityIds, EntityType entityType);
        Task MakeHighlightAsync(Guid entityId, EntityType entityType, DateTime from, DateTime to);
    }
}
