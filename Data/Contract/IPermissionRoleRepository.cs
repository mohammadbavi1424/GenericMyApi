using Data.Contract.GenericContract;
using Data.DTOs.Permission;
using Entities.Entities.ClaimTypes;

namespace Data.Contract
{
    public interface IPermissionRoleRepository : IRepository<PermissionRole>
    {
        Task<IEnumerable<PermissionDto>> GetUriByRoleId(long RoleId);
    }
}
