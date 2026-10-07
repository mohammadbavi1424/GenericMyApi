using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace WebFramework.Swagger
{
    public class SetVersionInPaths : Swashbuckle.Swagger.IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var updatedPaths = new Dictionary<string, PathItem>();

            foreach (var entry in swaggerDoc.paths)
            {
                updatedPaths.Add(
                    entry.Key.Replace("v{version}", swaggerDoc.info.version),
                    entry.Value);
            }

            swaggerDoc.paths = updatedPaths;
        }

        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var updatedPaths = new Dictionary<string, PathItem>();

            foreach (var entry in swaggerDoc.paths)
            {
                updatedPaths.Add(
                    entry.Key.Replace("v{version}", swaggerDoc.info.version),
                    entry.Value);
            }

            swaggerDoc.paths = updatedPaths;
        }
    }
}
