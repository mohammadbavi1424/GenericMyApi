using Common.LifeTimeInterface;
using Data.Context;
using Data.Contract;
using Data.DTOs.Permission;
using Data.Repository.GenericRepository;
using Entities.Entities.ClaimTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Repository
{
    public class PermissionRoleRepository : Repository<PermissionRole>, IPermissionRoleRepository
    {
        private readonly IRoleRepository roleRepository;

        public PermissionRoleRepository(AppDbContext dbContext, IConfiguration configration,
            IRoleRepository roleRepository) : base(dbContext, configration)
        {
            this.roleRepository = roleRepository;
        }

        public async Task<IEnumerable<PermissionDto>> GetUriByRoleId(long RoleId)
        {
            return await TableNoTracking
                .Include(p => p.Permission)
                .Where(p => p.RoleId == RoleId)
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    AreaName = p.Permission.AreaName,
                    ControllerName = p.Permission.ControllerName,
                    ActionName = p.Permission.ActionName,
                    Url = p.Permission.Url,
                    ApiName = p.Permission.ApiName,
                    NeedAccess = p.Permission.NeedAccess
                })
                .ToListAsync();
        }

        public override async Task AddAsync(PermissionRole entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateRoleAsync(entity.RoleId, cancellationToken);
            await base.AddAsync(entity, cancellationToken, saveNow);
        }

       

        public override async Task AddRangeAsync(IEnumerable<PermissionRole> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateRoleRangeAsync(entities.Select(r => r.RoleId).ToList(), cancellationToken);
            await base.AddRangeAsync(entities, cancellationToken, saveNow);
        }

        public override void Add(PermissionRole entity, bool saveNow = true)
        {
            UpdateRole(entity.RoleId);
            base.Add(entity, saveNow);
        }

        public override void AddRange(IEnumerable<PermissionRole> entities, bool saveNow = true)
        {
            UpdateRoleRange(entities.Select(r => r.RoleId).ToList());
            base.AddRange(entities, saveNow);
        }

        public override void Update(PermissionRole entity, bool saveNow = true)
        {
            UpdateRole(entity.RoleId);
            base.Update(entity, saveNow);
        }
        public override void UpdateRange(IEnumerable<PermissionRole> entities, bool saveNow = true)
        {
            UpdateRoleRange(entities.Select(r => r.RoleId).ToList());
            base.UpdateRange(entities, saveNow);
        }

        public override async Task UpdateAsync(PermissionRole entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateRoleAsync(entity.RoleId, cancellationToken);
            await base.UpdateAsync(entity, cancellationToken, saveNow);
        }

        public override async Task UpdateRangeAsync(IEnumerable<PermissionRole> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateRoleRangeAsync(entities.Select(r => r.RoleId).ToList(),cancellationToken);
            await base.UpdateRangeAsync(entities, cancellationToken, saveNow);
        }

        public override void Delete(PermissionRole entity, bool saveNow = true)
        {
            UpdateRole(entity.RoleId);
            base.Delete(entity, saveNow);
        }

        public override void DeleteRange(IEnumerable<PermissionRole> entities, bool saveNow = true)
        {
            UpdateRoleRange(entities.Select(r => r.RoleId).ToList());
            base.DeleteRange(entities, saveNow);
        }

        public override async Task DeleteAsync(PermissionRole entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateRoleAsync(entity.RoleId, cancellationToken);
            await base.DeleteAsync(entity, cancellationToken, saveNow);
        }

        public override async Task DeleteRangeAsync(IEnumerable<PermissionRole> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateRoleRangeAsync(entities.Select(r => r.RoleId).ToList(),cancellationToken);
            await base.DeleteRangeAsync(entities, cancellationToken, saveNow);
        }

        #region UpdateRole

        private void UpdateRoleRange(List<long> ids)
        {
            var roles = roleRepository.GetByRangIdQuerAsync(ids).Result;
            roleRepository.UpdateRange(roles, false);
        }

        private async Task UpdateRoleRangeAsync(List<long> ids, CancellationToken cancellationToken)
        {
            var roles = await roleRepository.GetByRangIdQuerAsync(ids);
            await roleRepository.UpdateRangeAsync(roles.Distinct().ToList(), cancellationToken, false);
        }

        private async Task UpdateRoleAsync(long id, CancellationToken cancellationToken)
        {
            var role = await roleRepository.GetByIdQuerAsync(id);
            await roleRepository.UpdateAsync(role, cancellationToken, false);
        }
        
        private void UpdateRole(long id)
        {
            var role = roleRepository.GetByIdQuerAsync(id).Result;
            roleRepository.Update(role, false);
        }

        #endregion

    }
}
