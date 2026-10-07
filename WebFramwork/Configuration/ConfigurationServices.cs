using AutoMapper;
using Common;
using Common.Api;
using Common.Exceptions;
using Common.SiteSetting;
using Common.Utilities;
using Data.Context;
using Data.Contract;
using Data.Contract.GenericContract;
using Data.Repository;
using Data.Repository.GenericRepository;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Services.Services;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace WebFramework.Configuration
{
    public static class ConfigurationServices
    {
        public static void AddBaseLifeTimeServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IJWTRepository, JWTRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionRoleRepository, PermissionRoleRepository>();
        }

        public static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
            });

        }

        public static void AddElmah(this IServiceCollection services,IConfiguration configuration,SiteSettings _siteSettings)
        {
            services.AddElmah<SqlErrorLog>(option =>
            {
                option.Path = _siteSettings.ElmahPath;
                option.ConnectionString = configuration.GetConnectionString("ElmahSqlServerConnection");
            });

        }

        public static void AddAuthenticationJwtConfig(this IServiceCollection services, SiteSettings _siteSettings )
        {

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.Key)),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true,
                    ValidAudience = _siteSettings.JwtSettings.Audience,

                    ValidateIssuer = true,
                    ValidIssuer = _siteSettings.JwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.EncryptionKey))
                };

                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                option.TokenValidationParameters = validationParameters;

                option.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("Authentication failed.", context.Exception);

                        if (context.Exception != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        //var applicationSignInManager = context.HttpContext.RequestServices.GetRequiredService<IApplicationSignInManager>();
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        var securityStamp = claimsIdentity.FindFirstValue("UserSecurityStamp");
                        if (!securityStamp.HasValue())
                            context.Fail("This token has no secuirty stamp");

                        //Find user and token from database and perform your custom validation
                        var userId = long.Parse(claimsIdentity.FindFirstValue("UserId").ToString());
                        var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                        if (user.SecurityStamp != securityStamp)
                            context.Fail("Token secuirty stamp is not valid.");

                        //var validatedUser = await applicationSignInManager.ValidateSecurityStampAsync(context.Principal);
                        //if (validatedUser == null)
                        //    context.Fail("Token secuirty stamp is not valid.");

                        if (user.IsDeleted.Value)
                            context.Fail("User is not active.");

                        await userRepository.UpdateLastLoginDateAsync(user, context.HttpContext.RequestAborted);
                    },
                    OnChallenge = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                        if (context.AuthenticateFailure != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                        //return Task.CompletedTask;
                    }
                };


            });

        }

        public static void AddMinimalMvc(this IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                options.Filters.Add(new AuthorizeFilter());

                //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
                //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
                //Use this filter when use cookie 
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                //options.UseYeKeModelBinder();

            })
            .AddApiExplorer()
            .AddAuthorization()
            .AddFormatterMappings()
            .AddDataAnnotations()
            //.AddJsonFormatters(/*options =>
            //{
            //    options.Formatting = Newtonsoft.Json.Formatting.Indented;
            //    options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            //}*/)
            .AddCors()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        //public static void AddSwagger(this IServiceCollection services)
        //{

        //    services.AddSwaggerGen(options =>
        //    {
        //        var provider = services.BuildServiceProvider()
        //                               .GetRequiredService<IApiVersionDescriptionProvider>();

        //        foreach (var desc in provider.ApiVersionDescriptions)
        //        {
        //            options.SwaggerDoc(desc.GroupName, new OpenApiInfo
        //            {
        //                Title = "MyApi",
        //                Version = desc.GroupName
        //            });
        //        }

        //        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //        {
        //            Name = "Authorization",
        //            Type = SecuritySchemeType.Http,
        //            Scheme = "Bearer",
        //            BearerFormat = "JWT",
        //            In = ParameterLocation.Header,
        //            Description = "Enter: Bearer {your token}"
        //        });
        //    });

        //}

        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            //Microsoft.Extensions.DependencyInjection
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ReportApiVersions = true;

            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        //public static void AddAutoMapper(this IServiceCollection services,params Assembly[] assemblies)
        //{

        //    var mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
        //    MapperProvider.Configure(mapper);
        //}

    }
}
