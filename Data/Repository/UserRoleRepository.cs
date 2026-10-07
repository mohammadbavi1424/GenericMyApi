using AutoMapper;
using Common.Exceptions;
using Common.LifeTimeInterface;
using Core.Utilities.Mapping;
using Data.Context;
using Data.Contract;
using Data.DTOs.Users;
using Data.Repository.GenericRepository;
using Entities.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Repository
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        private readonly IUserRepository userRepository;

        public UserRoleRepository(AppDbContext dbContext, IConfiguration configration,
            IUserRepository userRepository) : base(dbContext, configration)
        {
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<UserRoleDto>> GetUserRolesByUserId(long userId, CancellationToken cancellationToken)
        {
            //if(userId == 0 || userId == null)
            //{
            //    throw new NotFoundException();
            //}
            var userRoles = await TableNoTracking
                .Include(r => r.Role)
                .Where(ur => ur.UserId == userId)
                .Distinct()
                .ToListAsync(cancellationToken);


            var result = userRoles.Select(ur => new UserRoleDto()
            {
                Id = ur.Id,
                UserId = ur.UserId,
                RoleId = ur.RoleId,
                User = ur.User.ConvertObject<UserDto,User>(),
                Role = ur.Role.ConvertObject<RoleDto,Role>()

            });

            foreach(var item in result)
                item.Role = (userRoles.FirstOrDefault(c => c.Id == item.Id).Role)
                    .ConvertObject<RoleDto, Role>();
            
           
            return result;
        }


        public async Task UpdateUserRoleLastRoleInUse(long userId, long roleId, CancellationToken cancellationToken)
        {
            var userRole = await TableNoTracking.FirstOrDefaultAsync(ur => ur.UserId == userId &&
            ur.RoleId == roleId, cancellationToken);
            userRole.LastRoleInUse_Tiks = DateTime.Now.Ticks;
            await base.UpdateAsync(userRole, cancellationToken);

        }


        public override async Task UpdateAsync(UserRole entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateUserAsync(entity.UserId,cancellationToken);
            await base.UpdateAsync(entity, cancellationToken, saveNow);
        }

        public override async Task UpdateRangeAsync(IEnumerable<UserRole> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            await UpdateUserRangeAsync(entities.Select(c => c.UserId).ToList(),cancellationToken);
            await base.UpdateRangeAsync(entities, cancellationToken, saveNow);
        }

        public override void Update(UserRole entity, bool saveNow = true)
        {
            UpdateUser(entity.UserId);
            base.Update(entity, saveNow);
        }

        public override void UpdateRange(IEnumerable<UserRole> entities, bool saveNow = true)
        {
            List<long> userIds = entities.Select(c => c.UserId).ToList();
            var users = userRepository.GetByRangIdQuerAsync(userIds).Result;
            
            userRepository.UpdateRange(users, false);
            base.UpdateRange(entities, saveNow);
        }




        #region UpdateUser

        private void UpdateUserRange(List<long> ids)
        {
            var users = userRepository.GetByRangIdQuerAsync(ids).Result;
            userRepository.UpdateRange(users, false);
        }

        private async Task UpdateUserRangeAsync(List<long> ids, CancellationToken cancellationToken)
        {
            var users = await userRepository.GetByRangIdQuerAsync(ids);
            await userRepository.UpdateRangeAsync(users.Distinct().ToList(), cancellationToken, false);
        }

        private async Task UpdateUserAsync(long id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdQuerAsync(id);
            await userRepository.UpdateAsync(user, cancellationToken, false);
        }

        private void UpdateUser(long id)
        {
            var user = userRepository.GetByIdQuerAsync(id).Result;
            userRepository.Update(user, false);
        }


        public async Task DeleteRangeByRoleIdAsync(long roleId ,CancellationToken cancellationToken)
        {
            var userRoles = await TableNoTracking.Where(ur => ur.RoleId == roleId).ToListAsync(cancellationToken);
            await base.DeleteRangeAsync(userRoles, cancellationToken, false);
        }

        public void DeleteRangeByRoleId(long roleId)
        {
            var userRoles = TableNoTracking.Where(ur => ur.RoleId == roleId).ToList();
            base.DeleteRange(userRoles,false);
        }

        #endregion


    }
}
