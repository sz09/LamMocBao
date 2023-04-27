using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services.Interfaces
{
    public interface IAnalysisService : IService<Analysis>
    {
        Task FlushAsync();
        Task InitAsync();
        Task DeleteOldDataAsync();
        Task<List<Guid>> GetResourceOnTrendsAsync(EntityType entityType);
        Task IncreaseVisitAsync(Guid entityId, EntityType entityType);
    }
}
