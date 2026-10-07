using Common.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using System.Reflection;
using WebFramework.Swagger;

namespace WebFramework.Configuration.SwaggerConfiguration
{
    public static class ConfigurationSwagger
    {

        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            Assert.NotNull(services, nameof(services));

            //Add services to use Example Filters in swagger
            //services.AddSwaggerExamples();
            //Add services and configuration to use swagger
            services.AddSwaggerGen(options =>
            {
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
                //show controller XML comments like summary
                options.IncludeXmlComments(xmlDocPath, true);

                options.EnableAnnotations();

                //options.DescribeAllEnumsAsStrings();
                options.DescribeAllParametersInCamelCase();
                //options.DescribeStringEnumsInCamelCase();
                //options.UseReferencedDefinitionsForEnums();
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();

                //options.SwaggerDoc("v1", new Info { version = "v1", title = "API V1" });
                //options.SwaggerDoc("v2", new Info { version = "v2", title = "API V2" });
                //options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API V1" });
                //options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "API V2" });

                #region Filters
                ////Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
                //options.ExampleFilters();

                ////Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                //options.OperationFilter<AddFileParamTypesOperationFilter>();

                ////Set summary of action if not already set
                //options.OperationFilter<ApplySummariesOperationFilter>();

                #region Add UnAuthorized to Response
                //Add 401 response and security requirements (Lock icon) to actions that need authorization
                //options.OperationFilter<UnauthorizedResponsesOperationFilter>("Bearer");
                #endregion

                #region Add Jwt Authentication
                //Add Lockout icon on top of swagger ui page to authenticate
                //options.AddSecurityDefinition();
                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer",
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        Password = new OpenApiOAuthFlow
                //        {
                //            TokenUrl = new Uri("https://localhost:5001/api/v1/Account/Login"),
                //            Scopes = new Dictionary<string, string>
                //{
                //    { "api", "Access to the API" }
                //}
                //        }
                //    }
                //});

                //options.AddSecurityRequirement

                //options.AddSecurityRequirement(document =>
                //{
                //    OpenApiSecuritySchemeReference? schemeRef = new("Bearer");
                //    OpenApiSecurityRequirement? requirement = new()
                //    {
                //        [schemeRef] = []
                //    };
                //    return requirement;
                //});

                #endregion

                #region Versioning
                // Remove version parameter from all Operations
                //options.OperationFilter<RemoveVersionParameters>();

                ////set version "api/v{version}/[controller]" from current swagger doc verion
                //options.DocumentFilter<SetVersionInPaths>();

                ////Seperate and categorize end-points by doc version
                //options.DocInclusionPredicate((docName, apiDesc) =>
                //{
                //    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                //    var versions = methodInfo.DeclaringType
                //        .GetCustomAttributes<ApiVersionAttribute>(true)
                //        .SelectMany(attr => attr.Versions);

                //    return versions.Any(v => $"v{v.ToString()}" == docName);
                //});
                #endregion

                //If use FluentValidation then must be use this package to show validation in swagger (MicroElements.Swashbuckle.FluentValidation)
                //options.AddFluentValidationRules();
                #endregion
            });

        }


        public static void UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            Assert.NotNull(app, nameof(app));

            //Swagger middleware for generate "Open API Documentation" in swagger.json
            app.UseSwagger(options =>
            {
                //options.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            //Swagger middleware for generate UI from swagger.json
            app.UseSwaggerUI(options =>
            {
                #region Customizing
                //// Display
                //options.DefaultModelExpandDepth(2);
                //options.DefaultModelRendering(ModelRendering.Model);
                //options.DefaultModelsExpandDepth(-1);
                //options.DisplayOperationId();
                //options.DisplayRequestDuration();
                //options.DocExpansion(DocExpansion.None);
                //options.EnableDeepLinking();
                //options.EnableFilter();
                //options.MaxDisplayedTags(5);
                //options.ShowExtensions();

                //// Network
                //options.EnableValidator();
                //options.SupportedSubmitMethods(SubmitMethod.Get);

                //// Other
                //options.DocumentTitle = "CustomUIConfig";
                //options.InjectStylesheet("/ext/custom-stylesheet.css");
                //options.InjectJavascript("/ext/custom-javascript.js");
                //options.RoutePrefix = "api-docs";
                #endregion

                options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2 Docs");
            });

        }


    }
}
