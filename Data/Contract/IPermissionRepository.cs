using Data.Contract.GenericContract;
using Data.DTOs.Permission;
using Entities.Entities.ClaimTypes;

namespace Data.Contract
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task AddRangeAsync(List<PermissionDto> permissionDtos, bool saveNow = true);
    }
}
