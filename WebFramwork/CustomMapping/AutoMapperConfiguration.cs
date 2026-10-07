using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace WebFramework.CustomMapping
{
    public static class AutoMapperConfiguration
    {
        public static IMapper InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddCustomMappingProfile();
            });

            return config.CreateMapper();
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
        {
            var WebframeworkAssembly = typeof(IHaveCustomMapping).Assembly;
            config.AddCustomMappingProfile(Assembly.GetEntryAssembly(),WebframeworkAssembly);
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

            var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
                .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);

            config.AddProfile(profile);
        }
    }
}
