using Common.LifeTimeInterface;
using Common.SiteSetting;
using Data.Contract;
using Data.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Services
{
    public class JWTRepository : IJWTRepository
    {

        private readonly IUserRepository userRepository;
        private readonly IUserRoleRepository userRoleRepository;
        private readonly SiteSettings _siteSettings;
        public JWTRepository(IUserRepository userRepository,
            IUserRoleRepository userRoleRepository, IOptionsSnapshot<SiteSettings> setting)
        {
            this.userRepository = userRepository;
            this.userRoleRepository = userRoleRepository;
            _siteSettings = setting.Value;
        }





        public async Task<string> GeneratToken(LoginDto login, CancellationToken cancellationToken)
        {

            var securityKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.Key);

            var signingCredential = new SigningCredentials(
                new SymmetricSecurityKey(securityKey),
                SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.EncryptionKey);

            //var encryptionkey = Convert.FromBase64String(_siteSettings.JwtSettings.EncryptionKey);


            var encryptingCredentials = new EncryptingCredentials(
                new SymmetricSecurityKey(encryptionkey),
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes256CbcHmacSha512);


            var claimIdentity = await GetClaims(login, cancellationToken);


            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSettings.JwtSettings.Issuer,
                Audience = _siteSettings.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.NotBeforMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.ExpiertionMinutes),
                SigningCredentials = signingCredential,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claimIdentity),
                Claims = claimIdentity.ToDictionary(c => c.Type, c => (object)c.Value)

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(descriptor);

            var jwt = tokenHandler.WriteToken(securityToken);

            return jwt;
        }


        private async Task<List<Claim>> GetClaims(LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByUserPassword(loginDto);
            UserRoleDto userRole = new UserRoleDto();
            
            if (user != null)
            {
                userRole = (await userRoleRepository.GetUserRolesByUserId(user.Id,cancellationToken)).FirstOrDefault();
            }

            //var claimsIdentityOptions = new ClaimsIdentityOptions();
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,userRole.RoleId.ToString()),
                new Claim(new ClaimsIdentityOptions().SecurityStampClaimType,user.SecurityStamp),
                new Claim("RoleSecurityStamp",userRole.Role.SecurityStamp)
            };


            return claims;
        }

    }
}
