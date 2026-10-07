using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Common.LifeTimeInterface;
using Data.Context;
using Data.Contract;
using Data.Contract.GenericContract;
using Data.Repository;
using Data.Repository.GenericRepository;
using Entities.Entities.User;
using Microsoft.Extensions.DependencyInjection;
using Services.Services;

namespace WebFramework
{
    public static class ServicesCollections
    {
        private static void AddServices(this ContainerBuilder container)
        {
            container.RegisterGeneric(typeof(Repository<>))
                     .As(typeof(IRepository<>))
                     .InstancePerLifetimeScope();

            var commonAssambly = typeof(IScopeDependency).Assembly;
            var servicesAssembly = typeof(JWTRepository).Assembly;
            var dataAssembly = typeof(AppDbContext).Assembly;
            var entitiesAssembly = typeof(User).Assembly;
            var repoAssembly = typeof(IPermissionRoleRepository).Assembly;

            container.RegisterAssemblyTypes(commonAssambly, servicesAssembly, dataAssembly, entitiesAssembly, repoAssembly)
                .AssignableTo<IScopeDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            container.RegisterAssemblyTypes(commonAssambly, servicesAssembly, dataAssembly, entitiesAssembly, repoAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            container.RegisterAssemblyTypes(commonAssambly, servicesAssembly, dataAssembly, entitiesAssembly, repoAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }


        public static IServiceProvider RegisterService(this IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.AddServices();

            return new AutofacServiceProvider(containerBuilder.Build());
        }

    }
}
