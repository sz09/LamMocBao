using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.DbContexts;
using Services.Services.Interfaces;
using Shared.Models.Identify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Identify
{
    public class RoleStore<TRole> :
                 IQueryableRoleStore<TRole>,
                 IRoleClaimStore<TRole>
        where TRole : Role
    {
        private readonly IDbContext _dbContext;
        private readonly DbSet<TRole> _roles;
        private readonly IRoleService _roleService;
        public RoleStore(IRoleService roleService, IDbContext dbContext)
        {
            _roleService = roleService;
            _dbContext = dbContext;
            _roles = dbContext.SetOf<Role>() as DbSet<TRole>;
        }
        public IQueryable<TRole> Roles => _roles.AsQueryable();

        public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            var foundRole = _roles.FirstOrDefault(d => d.Id == role.Id);
            if(foundRole != null)
            {
                foundRole.Claim = claim.Value;
                foundRole.Type = claim.Type;
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            await _roleService.AddAsync(role);
            return LMBIdentityResult.Success(role.Id);
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            await _roleService.DeleteAsync(role.Id);
            return IdentityResult.Success;
        }

        public void Dispose()
        {

        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(roleId, out Guid id))
            {
                return await FindAsync(id);
            }

            return null;
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _roles.FirstOrDefault(d => d.NormalizedRole == normalizedRoleName);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default)
        {
            var roles = _roles.Where(d => d.Type == role.Type).ToList();
            return roles.Select(d => new Claim(d.Type, d.Claim)).ToList();
        }

        public async Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            var foundRole = await LoadAsync(role.Id);
            return foundRole?.NormalizedRole;
        }

        public async Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            var foundRole = await LoadAsync(role.Id);
            return foundRole?.Id.ToString();
        }

        public async Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            var foundRole = await LoadAsync(role.Id);
            return foundRole?.Type;
        }

        public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            var foundRole = await FindAsync(role.Id);
            if(foundRole != null)
            {
                _roles.Remove(foundRole);
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {

            var foundRole = await FindAsync(role.Id);
            if (foundRole != null)
            {
                foundRole.NormalizedRole = normalizedName;
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            var foundRole = await FindAsync(role.Id);
            if(foundRole != null)
            {
                foundRole.Type = roleName;
                await _dbContext.SaveAsync(cancellationToken);
            }
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            await _roleService.UpdateAsync(role.Id, (r) =>
            {
                r.NormalizedRole = role.NormalizedRole;
                r.Claim = role.Claim;
                r.Type = role.Type;
            }, cancellationToken);
            return IdentityResult.Success;
        }

        private async Task<TRole> LoadAsync(Guid id)
        {
            return await _roleService.LoadAsync(id) as TRole;
        }

        private async Task<TRole> FindAsync(Guid id)
        {
            return _roles.FirstOrDefault(d => d.Id == id);
        }
    }
}
