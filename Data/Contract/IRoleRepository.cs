using Data.Contract.GenericContract;
using Entities.Entities.User;

namespace Data.Contract
{
    public interface IRoleRepository: IRepository<Role>
    {
        Task<bool> CheckSecurityStamp(long roleId, string securityStamp);
    }
}