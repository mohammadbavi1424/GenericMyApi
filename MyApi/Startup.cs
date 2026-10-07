using Common.SiteSetting;
using ElmahCore.Mvc;
using System.Text.Json.Serialization;
using WebFramework.Configuration;
using WebFramework.Configuration.SwaggerConfiguration;
using WebFramework.Midlewares;



namespace MyApi
{
    public class Startup
    {
        private readonly SiteSettings _siteSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //AutoMapperConfiguration.InitializeAutoMapper();
            _siteSettings = Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.AddControllers()
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.Mapp

            services.AddDatabaseContext(Configuration);

            services.AddMinimalMvc();

            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", b =>
                b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );
            });


            services.AddElmah(Configuration, _siteSettings);

            services.AddAuthenticationJwtConfig(_siteSettings);

            services.AddBaseLifeTimeServices();

            services.AddSwaggerConfiguration();

            services.AddCustomApiVersioning();





            //services.RegisterService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCustomExceptionHandler();
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseSwaggerConfiguration();
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
            //});


            app.UseHsts(env);


            app.UseStaticFiles();

            app.UseRouting();

            app.UseElmah();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseMiddleware<AuthenticationMiddleWare>();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }



    }
}

