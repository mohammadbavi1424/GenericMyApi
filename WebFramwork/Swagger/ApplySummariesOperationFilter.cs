using Common.Utilities;
using Microsoft.AspNetCore.Mvc.Controllers;
using Pluralize.NET;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.Swagger;
using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace WebFramework.Swagger
{
    public class ApplySummariesOperationFilter : Swashbuckle.Swagger.IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return;

            var pluralizer = new Pluralizer();

            var actionName = controllerActionDescriptor.ActionName;
            var singularizeName = pluralizer.Singularize(controllerActionDescriptor.ControllerName);
            var pluralizeName = pluralizer.Pluralize(singularizeName);

            var parameterCount = operation.parameters.Where(p => p.name != "version" && p.name != "api-version").Count();

            if (IsGetAllAction())
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Returns all {pluralizeName}";
            }
            else if (IsActionName("Post", "Create"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Creates a {singularizeName}";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"A {singularizeName} representation";
            }
            else if (IsActionName("Read", "Get"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Retrieves a {singularizeName} by unique id";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"a unique id for the {singularizeName}";
            }
            else if (IsActionName("Put", "Edit", "Update"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Updates a {singularizeName} by unique id";

                //if (!operation.parameters[0].description.HasValue())
                //    operation.parameters[0].description = $"A unique id for the {singularizeName}";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"A {singularizeName} representation";
            }
            else if (IsActionName("Delete", "Remove"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Deletes a {singularizeName} by unique id";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"A unique id for the {singularizeName}";
            }

            #region Local Functions
            bool IsGetAllAction()
            {
                foreach (var name in new[] { "Get", "Read", "Select" })
                {
                    if ((actionName.Equals(name, StringComparison.OrdinalIgnoreCase) && parameterCount == 0) ||
                        actionName.Equals($"{name}All", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{pluralizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}All{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}All{pluralizeName}", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            bool IsActionName(params string[] names)
            {
                foreach (var name in names)
                {
                    if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}ById", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{singularizeName}ById", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            #endregion
        }

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var controllerActionDescriptor = apiDescription.ActionDescriptor as HttpActionDescriptor;
            if (controllerActionDescriptor == null) return;

            var pluralizer = new Pluralizer();

            var actionName = controllerActionDescriptor.ActionName;
            var singularizeName = pluralizer.Singularize(controllerActionDescriptor.ControllerDescriptor.ControllerName);
            var pluralizeName = pluralizer.Pluralize(singularizeName);

            var parameterCount = operation.parameters.Where(p => p.name != "version" && p.name != "api-version").Count();

            if (IsGetAllAction())
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Returns all {pluralizeName}";
            }
            else if (IsActionName("Post", "Create"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Creates a {singularizeName}";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"A {singularizeName} representation";
            }
            else if (IsActionName("Read", "Get"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Retrieves a {singularizeName} by unique id";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"a unique id for the {singularizeName}";
            }
            else if (IsActionName("Put", "Edit", "Update"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Updates a {singularizeName} by unique id";

                //if (!operation.parameters[0].description.HasValue())
                //    operation.parameters[0].description = $"A unique id for the {singularizeName}";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"A {singularizeName} representation";
            }
            else if (IsActionName("Delete", "Remove"))
            {
                if (!operation.summary.HasValue())
                    operation.summary = $"Deletes a {singularizeName} by unique id";

                if (!operation.parameters[0].description.HasValue())
                    operation.parameters[0].description = $"A unique id for the {singularizeName}";
            }

            #region Local Functions
            bool IsGetAllAction()
            {
                foreach (var name in new[] { "Get", "Read", "Select" })
                {
                    if ((actionName.Equals(name, StringComparison.OrdinalIgnoreCase) && parameterCount == 0) ||
                        actionName.Equals($"{name}All", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{pluralizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}All{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}All{pluralizeName}", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            bool IsActionName(params string[] names)
            {
                foreach (var name in names)
                {
                    if (actionName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}ById", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{singularizeName}", StringComparison.OrdinalIgnoreCase) ||
                        actionName.Equals($"{name}{singularizeName}ById", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            #endregion
        }
    }
}
