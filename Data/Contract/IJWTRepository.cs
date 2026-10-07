using Data.DTOs.Users;

namespace Data.Contract
{
    public interface IJWTRepository
    {
        Task<string> GeneratToken(LoginDto login,CancellationToken cancellationToken);
    }
}