using Data.Contract.GenericContract;
using Data.DTOs.Users;
using Entities.Entities.User;

namespace Data.Contract
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {

        Task<IEnumerable<UserRoleDto>> GetUserRolesByUserId(long userId, CancellationToken cancellationToken);

        Task UpdateUserRoleLastRoleInUse(long userId, long roleId, CancellationToken cancellationToken);

        Task DeleteRangeByRoleIdAsync(long roleId, CancellationToken cancellationToken);

        void DeleteRangeByRoleId(long roleId);

    }
}
