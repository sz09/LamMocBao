using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models;
using System.Collections.Generic;

namespace Services.Services
{
    public class AppointmentService : Service<Appointment>, IService<Appointment>, IAppointmentService
    {
        public AppointmentService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders): base(dbContext, _cache, cachingLoaders) { }
    }
}
