using Humanizer;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services
{
    // MUST not use LoadOrDefaultAsync method
    public class AnalysisModel
    {
        public Guid EntityId { get; set; }
        public int AccessCount { get; set; }
        public EntityType EntityType { get; set; }
    }

    public class AnalysisService : Service<Analysis>, IAnalysisService
    {
        private readonly IServiceConfig _serviceConfig;
        public AnalysisService(IDbContext context, InMemoryCache _cache, IServiceConfig serviceConfig) : base(context, _cache)
        {
            _serviceConfig = serviceConfig;
        }

        public async Task DeleteOldDataAsync() 
        {
            var now = DateTime.UtcNow;
            var lastDateKept = now.AddDays(- _serviceConfig.OnTrendInDays);
            DbSet.Where(d => d.CreatedAt <= lastDateKept).DeleteFromQuery();
        }

        public async Task FlushAsync()
        {
            var now = DateTime.UtcNow;
            var productDict = _memoryCache.TryGet<Guid>(GetKey(EntityType.Products));
            var productIds = productDict.Select(d => d.Key).ToList();
            var products = DbSet.Where(d => d.EntityType == EntityType.Products && productIds.Contains(d.EntityId))
                                    .ToList();
            Console.WriteLine($"Flushing {products.Count} product(s)");
            foreach (var item in productDict)
            {
                var product = products.FirstOrDefault(d => d.EntityId == item.Key);
                if (product != null)
                {
                    product.AccessCount += item.Value;
                }
                else
                {
                    DbSet.Add(new Analysis
                    {
                        Id = GuidGenerator.NewGuid(),
                        AccessCount = item.Value,
                        EntityType = EntityType.Products,
                        EntityId = item.Key
                    });
                }
            }

            await SaveChangesAsync(default);

            var knowledgeDict = _memoryCache.TryGet<Guid>(GetKey(EntityType.Knowledges));
            var knowledgeIds = knowledgeDict.Select(d => d.Key).ToList();

            var knowledges = DbSet.Where(d => d.EntityType == EntityType.Knowledges && knowledgeIds.Contains(d.EntityId))
                                    .ToList();
            Console.WriteLine($"Flushing {knowledges.Count} knowledge(s)");
            foreach (var item in knowledgeDict)
            {
                var knowledge = knowledges.FirstOrDefault(d => d.EntityId == item.Key);
                if (knowledge != null)
                {
                    knowledge.AccessCount += item.Value;
                }
                else
                {
                    DbSet.Add(new Analysis
                    {
                        Id = GuidGenerator.NewGuid(),
                        AccessCount = item.Value,
                        EntityType = EntityType.Knowledges,
                        EntityId  = item.Key
                    });
                }
            }

            await SaveChangesAsync(default);
        }

        public async Task IncreaseVisitAsync(Guid entityId, EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Products:
                    IncreaseVisitForProduct(entityId);
                    break;
                case EntityType.Knowledges:
                    IncreaseVisitForKnowledge(entityId);
                    break;
            }
        }

        private void IncreaseVisitForProduct(Guid entityId)
        {
            var productDict = _memoryCache.TryGet<Guid>(GetKey(EntityType.Products));
            if (productDict.ContainsKey(entityId))
            {
                productDict[entityId]++;
            }
            else
            {
                productDict[entityId] = 1;
            }

            _memoryCache.ForceSet(GetKey(EntityType.Products), productDict);
        }

        private void IncreaseVisitForKnowledge(Guid entityId)
        {
            var knowledgeDict = _memoryCache.TryGet<Guid>(GetKey(EntityType.Knowledges));
            if (knowledgeDict.ContainsKey(entityId))
            {
                knowledgeDict[entityId]++;
            }
            else
            {
                knowledgeDict[entityId] = 1;
            }

            _memoryCache.ForceSet(GetKey(EntityType.Knowledges), knowledgeDict);
        }

        public async Task InitAsync()
        {
            _memoryCache.ForceSet(GetKey(EntityType.Products), new Dictionary<Guid, int>());
            _memoryCache.ForceSet(GetKey(EntityType.Knowledges), new Dictionary<Guid, int>());
        }

        private string GetKey(EntityType entityType) => $"Visits_{entityType.ToString().Pluralize()}";

        public async Task<List<Guid>> GetResourceOnTrendsAsync(EntityType entityType)
        {
            var result = new List<Guid>();
            result = _context.HighlightItems.Where(d => d.EntityType == entityType)
                                   .Take(_serviceConfig.NumberOfItemsOnTrend)
                                   .Select(d => d.EntityId)
                                   .ToList();
            var needTakeMore = result.Count < _serviceConfig.NumberOfItemsOnTrend;
            if (needTakeMore)
            {

                // Trend on today
                var entityDict = _memoryCache.TryGet<Guid>(GetKey(entityType)).Take(_serviceConfig.NumberOfItemsOnTrend - result.Count);
                result.AddRange(entityDict.Select(d => d.Key));
                var needTakeMore1 = result.Count < _serviceConfig.NumberOfItemsOnTrend;
                if (needTakeMore1)
                {
                    var resources = DbSet.Where(d => d.EntityType == entityType)
                                    .GroupBy(d => d.EntityId)
                                    .Select(d => new
                                    {
                                        EntityId = d.Key,
                                        AccessCount = d.Sum(s => s.AccessCount)
                                    })
                                    .OrderBy(d => d.AccessCount)
                                    .Select(d => d.EntityId)
                                    .Take(_serviceConfig.NumberOfItemsOnTrend - result.Count)
                                    .ToList();
                    result.AddRange(resources);
                }
            }

            return result;
        }
    }
}
