using Services.DbContexts;
using Services.Services.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PingService : IPingService
    {
        private readonly IDbContext _dbContext;
        private readonly IServiceConfig _serviceConfig;

        public PingService(IDbContext dbContext, IServiceConfig serviceConfig)
        {
            _dbContext = dbContext;
            _serviceConfig = serviceConfig;
        }

        public async Task PingAsync()
        {
            await PingWebSiteAsync();
            PingDB();
        }

        private void PingDB()
        {
            _dbContext.ProductTypes.Any();
        }

        private async Task PingWebSiteAsync()
        {
            using var client = new HttpClient();
            await client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_serviceConfig.PingEndpoint)
            }).ConfigureAwait(false);
        }
    }
}
