using Autofac.Extensions.DependencyInjection;
using Data.Context;
using Data.Contract;
using Data.DTOs.Permission;
using MyApi.LocalServices;
using NLog.Web;
using Serilog;
using System.Net;

namespace MyApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            //Set deafult proxy
            //WebRequest.DefaultWebProxy = new WebProxy("http://127.0.0.1:8118", true) { UseDefaultCredentials = true };

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            
            try
            {
                logger.Debug("init main");
                logger.Info("Starting application...");
                //logger.Trace("");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var permissionRepository = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();


                    var claimsItems = new AreaControllerActionService().
                    ApiAreaAndActionAndControllerNamesList()
                    .Select(c => new PermissionDto()
                    {
                        AreaName = c.AreaName,
                        ControllerName = c.ControllerName,
                        ActionName = c.ActionName,
                        NeedAccess = c.NeedAccess,
                    }).ToList();

                    await permissionRepository.AddRangeAsync(claimsItems);
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }


            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging(option => option.ClearProviders())
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

}
