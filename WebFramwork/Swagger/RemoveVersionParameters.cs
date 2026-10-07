using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http.Description;

namespace WebFramework.Swagger
{
    public class RemoveVersionParameters : Swashbuckle.Swagger.IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Remove version parameter from all Operations
            var versionParameter = operation.parameters.SingleOrDefault(p => p.name == "version");
            if (versionParameter != null)
                operation.parameters.Remove(versionParameter);
        }

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // Remove version parameter from all Operations
            var versionParameter = operation.parameters.SingleOrDefault(p => p.name == "version");
            if (versionParameter != null)
                operation.parameters.Remove(versionParameter);
        }
    }
}
