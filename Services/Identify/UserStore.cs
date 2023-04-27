using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models.Identify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Identify
{
    public class UserStore<TUser> :
            IUserPasswordStore<TUser>,
            IUserRoleStore<TUser>
        where TUser : User
    {
        private readonly IDbContext _dbContext;  
        private readonly DbSet<TUser> _users;  
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly InMemoryCache _memoryCache;
        public UserStore(IDbContext dbContext, IUserService userService, IRoleService roleService, InMemoryCache memoryCache)
        {
            _userService = userService;
            _dbContext = dbContext;
            _users = dbContext.SetOf<User>() as DbSet<TUser>;
            _roleService = roleService;
            _memoryCache = memoryCache;
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            //var foundUser = await FindAsync(user.Id);
            //Guid roleId = Guid.Empty;
            //var role = 
            //if (foundUser != null)
            //{
            //    foundUser.Roles.Add(new UserRole { Name = roleName, UserId = user.Id });
            //    await _dbContext.SaveAsync();
            //}
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            await _userService.AddAsync(user, cancellationToken);
            return LMBIdentityResult.Success(user.Id);
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            await _userService.DeleteAsync(user.Id, cancellationToken);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
            
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(userId, out Guid guid))
            {
                return await LoadAsync(guid);
            }

            throw null;
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var foundUser = _users.FirstOrDefault(d => d.Username == normalizedUserName);
            return foundUser;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            var foundUser = await LoadAsync(user.Id);
            if (foundUser != null)
            {
                return foundUser.Roles.Select(d => d.Role?.Type).ToList();
            }

            return null;
        }

        public async Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            var foundUser = await LoadAsync(user.Id);
            return foundUser?.Id.ToString();
        }

        public async Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            var foundUser = await LoadAsync(user.Id);
            return foundUser?.Username;
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return _users.Where(d => d.Roles != null && d.Roles.Any(s => s.Role.Type== roleName)).ToList();
        }

        public async Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            var foundUser = await LoadAsync(user.Id);
            return string.IsNullOrWhiteSpace(foundUser?.PasswordHash);
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            return await _users.AnyAsync(d => d.Roles != null && d.Roles.Any(s => s.Role.Type == roleName));
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var foundUser = await FindAsync(user.Id);
            if (foundUser != null)
            {
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            var foundUser = await FindAsync(user.Id);
            if (foundUser != null)
            {
                foundUser.PasswordHash = passwordHash;
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            var foundUser = await FindAsync(user.Id);
            if (foundUser != null)
            {
                foundUser.Username = userName;
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            await _userService.UpdateAsync(user.Id, (u) =>
            {
                u.Email = user.Email;
                u.Username = user.Username;
                u.PhoneNumber = user.PhoneNumber;
                u.PasswordHash = user.PasswordHash;
                u.Type = user.Type;
            });
            return IdentityResult.Success;
        }

        private async Task<TUser> LoadAsync(Guid id)
        {
            return await _userService.LoadAsync(id) as TUser;
        }

        private async Task<TUser> FindAsync(Guid id)
        {
            return _users.FirstOrDefault(d => d.Id == id);
        }
    }
}