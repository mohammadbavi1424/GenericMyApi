using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace WebFramework.Swagger
{
    public class UnauthorizedResponsesOperationFilter : IOperationFilter
    {
        private readonly bool includeUnauthorizedAndForbiddenResponses;
        private readonly string schemeName;

        public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
        {
            this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            this.schemeName = schemeName;
        }

        public void Apply(Operation operation, Swashbuckle.AspNetCore.SwaggerGen.OperationFilterContext context)
        {
            var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;

            var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter);
            if (hasAnonymous) return;

            var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter);
            if (!hasAuthorize) return;

            if (includeUnauthorizedAndForbiddenResponses)
            {
                operation.responses.TryAdd("401", new Response { description = "Unauthorized" });
                operation.responses.TryAdd("403", new Response { description = "Forbidden" });
            }

            operation.security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>> { { schemeName, new string[] { } } }
            };
        }

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            //var filter = apiDescription.ActionDescriptor.GetFilters() as List<FilterDescriptor>();
        }
    }
}
