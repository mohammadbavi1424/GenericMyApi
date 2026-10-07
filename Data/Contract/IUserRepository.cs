using Data.Contract.GenericContract;
using Data.DTOs.Users;
using Entities.Entities.User;

namespace Data.Contract
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsernameAndPasss(string username, string pass);
        Task AddAsync(User user, string password, CancellationToken cancellationToken);
        Task<UserDto> CheckAuthentication(LoginDto loginDto, CancellationToken cancellationToken);
        Task<UserDto> GetUserByUserPassword(LoginDto loginDto);
        string GenerateToken(UserDto userDto);
        string UpdateToken(UserRoleDto userRoleDto);
        Task<bool> CheckSecurityStamp(long userId, string securityStamp);
        public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);

    }
}