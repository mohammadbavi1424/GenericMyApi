using Common.LifeTimeInterface;
using Data.Context;
using Data.Contract;
using Data.Repository.GenericRepository;
using Entities.Entities.User;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repository
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        private readonly IUserRoleRepository userRoleRepository;

        public RoleRepository(AppDbContext dbContext, IConfiguration configuration,
            IUserRoleRepository userRoleRepository) : base(dbContext, configuration)
        {
            this.userRoleRepository = userRoleRepository;
        }

        public async Task<bool> CheckSecurityStamp(long roleId, string securityStamp)
        {
            var role = await this.GetByIdQuerAsync(roleId);
            return role.SecurityStamp == securityStamp;
        }


        public override void Add(Role entity, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            base.Add(entity, saveNow);
        }

        public override Task AddAsync(Role entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.AddAsync(entity, cancellationToken, saveNow);
        }

        public override void AddRange(IEnumerable<Role> entities, bool saveNow = true)
        {
            foreach (var entity in entities) 
                entity.SecurityStamp = Guid.NewGuid().ToString();
            base.AddRange(entities, saveNow);
        }

        public override Task AddRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            foreach(var entity in entities)
                entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.AddRangeAsync(entities, cancellationToken, saveNow);
        }

        public override Task UpdateAsync(Role entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.UpdateAsync(entity, cancellationToken, saveNow);
        }

        public override Task UpdateRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            foreach (var entity in entities) 
                entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.UpdateRangeAsync(entities, cancellationToken, saveNow);
        }

        public override void Update(Role entity, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            base.Update(entity, saveNow);
        }

        public override void UpdateRange(IEnumerable<Role> entities, bool saveNow = true)
        {
            foreach(var entity in entities)
                entity.SecurityStamp = Guid.NewGuid().ToString();
            base.UpdateRange(entities, saveNow);
        }


        public override void Delete(Role entity, bool saveNow = true)
        {
            userRoleRepository.DeleteRangeByRoleId(entity.Id);
            base.Delete(entity, saveNow);
        }

        public override void DeleteRange(IEnumerable<Role> entities, bool saveNow = true)
        {
            foreach (var entity in entities)
                userRoleRepository.DeleteRangeByRoleId(entity.Id);
            base.DeleteRange(entities, saveNow);
        }

        public override async Task DeleteAsync(Role entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            await userRoleRepository.DeleteRangeByRoleIdAsync(entity.Id, cancellationToken);
            await base.DeleteAsync(entity, cancellationToken, saveNow);
        }

        public override async Task DeleteRangeAsync(IEnumerable<Role> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            foreach( var entity in entities)
                await userRoleRepository.DeleteRangeByRoleIdAsync(entity.Id, cancellationToken);
            await base.DeleteRangeAsync(entities, cancellationToken, saveNow);
        }


    }
}
