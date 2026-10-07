using AutoMapper;
using Data.DTOs.Users;
using Entities.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;
using WebFramework.CustomMapping;

namespace WebFramework.MappingProfile
{
    public class UserCustomProfile : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<UserDto,User>().ReverseMap();
        }

    }
}
