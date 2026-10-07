using Data.DTOs.Base;
using Entities.Entities.User;

namespace Data.DTOs.Users
{
    public class RoleDto:BaseDto<RoleDto,Role>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? SecurityStamp { get; set; }
    }
}