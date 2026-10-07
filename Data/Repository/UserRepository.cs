using AutoMapper;
using Common.Exceptions;
using Common.LifeTimeInterface;
using Common.Utilities;
using Core.Utilities.Mapping;
using Data.Context;
using Data.Contract;
using Data.DTOs.Users;
using Data.Repository.GenericRepository;
using Entities.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Data.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {

        private readonly IConfiguration _config;

        public UserRepository(AppDbContext dbContext, IConfiguration config
            ) : base(dbContext, config)
        {
            _config = config;
        }


        public async Task<User> GetUserByUsernameAndPasss(string username, string pass)
        {
            var passHash = pass.GetSha256Hash();
            return await TableNoTracking.FirstOrDefaultAsync(u => u.UserName == username && u.Password == passHash);

        }

        public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTimeOffset.Now;
            return UpdateAsync(user, cancellationToken);
        }

        

        public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
        {
            var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName);
            if (exists)
                throw new BadRequestException("نام کاربری تکراری است");
            user.SecurityStamp = Guid.NewGuid().ToString();
            var passwordHash = SecurityHelper.GetSha256Hash(password);
            user.Password = passwordHash;
            await base.AddAsync(user, cancellationToken);
        }


        public async Task<UserDto> CheckAuthentication(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(loginDto.Password);
            var user = await TableNoTracking.FirstOrDefaultAsync(p => p.UserName == loginDto.UserName
            && p.Password == passwordHash, cancellationToken);
            if (user == null)
                throw new Exception("نام کاربری تکراری است");

            return user.ConvertObject<UserDto, User>();
        }


        public string GenerateToken(UserDto userDto)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim("UserId", userDto.Id.ToString()),
            new Claim("UserName", userDto.UserName)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string UpdateToken(UserRoleDto userRoleDto)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("UserId", userRoleDto.UserId.ToString()),
                new Claim("UserName", userRoleDto.User.UserName),
                new Claim("RoleId", userRoleDto.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserDto> GetUserByUserPassword(LoginDto loginDto)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(loginDto.Password);

            var user = await TableNoTracking.FirstOrDefaultAsync(u =>
            (u.UserName == loginDto.UserName || u.Email == loginDto.UserName ||
            u.PhoneNumber == loginDto.UserName) && u.Password == passwordHash);

            return user.ConvertObject<UserDto, User>();
        }

        public async Task<bool> CheckSecurityStamp(long userId, string securityStamp)
        {
            var user = await this.GetByIdQuerAsync(userId);
            return user.SecurityStamp == securityStamp;
        }


        public override void Add(User entity, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            base.Add(entity, saveNow);
        }

        public override Task AddAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.AddAsync(entity, cancellationToken, saveNow);
        }

        public override void AddRange(IEnumerable<User> entities, bool saveNow = true)
        {
            foreach(var entity in entities)
                entity.SecurityStamp = Guid.NewGuid().ToString();
            base.AddRange(entities, saveNow);
        }

        public override Task AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            foreach (var entity in entities)
                entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.AddRangeAsync(entities, cancellationToken, saveNow);
        }

        

        public override void Update(User entity, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();

            base.Update(entity, saveNow);
        }

        public override void UpdateRange(IEnumerable<User> entities, bool saveNow = true)
        {
            foreach (var entity in entities)
                entity.SecurityStamp = Guid.NewGuid().ToString();
            base.UpdateRange(entities, saveNow);
        }

        public override Task UpdateAsync(User entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.UpdateAsync(entity, cancellationToken, saveNow);
        }

        public override Task UpdateRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            foreach (var entity in entities)
                entity.SecurityStamp = Guid.NewGuid().ToString();
            return base.UpdateRangeAsync(entities, cancellationToken, saveNow);
        }



    }
}
