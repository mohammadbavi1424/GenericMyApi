using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Data.DTOs.Base;
using Entities.Entities.User;

namespace Data.DTOs.Users
{
    public class UserRoleDto :BaseDto<UserRoleDto,UserRole>
    {
        

        public long UserId { get; set; }
        public long RoleId { get; set; }

        [Ignore]
        public UserDto User { get; set; }
        [Ignore]
        public RoleDto Role { get; set; }
    }
}
