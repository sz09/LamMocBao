using Services.Caching;
using Services.DbContexts;
using Services.ModelResult;
using Services.Services.Interfaces;
using Shared.Models;
using Shared.Models.Identify;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Services
{
    public class UserService : Service<User>, IService<User>, IUserService
    {
        public UserService(IDbContext dbContext, InMemoryCache _cache, IEnumerable<ICachingLoader> cachingLoaders) : base(dbContext, _cache, cachingLoaders) { }

        public bool AuthorizeCustomer(string username, string password, out User user)
        {
            user = null;
            var passwordHash = MD5Utilities.Hash(password);
            user = DbSet.FirstOrDefault(d => d.Type == AccountType.Customer && d.Username == username && d.PasswordHash == passwordHash);
            return user != null;
        }

        public bool AuthorizeAdminUser(string username, string password, out User user)
        {
            user = null;
            var passwordHash = MD5Utilities.Hash(password);
            var adminTypes = new List<AccountType>
            {
                AccountType.SupperAdmin,
                AccountType.Admin
            };

            user = DbSet.FirstOrDefault(d => d.Username == username && 
                                        d.PasswordHash == passwordHash &&
                                        adminTypes.Contains(d.Type));
            return user != null;
        }
    }
}
