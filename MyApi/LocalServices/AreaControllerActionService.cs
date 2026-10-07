using Data.DTOs.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WebFramework.Api;

namespace MyApi.LocalServices
{
    public class AreaControllerActionService
    {
        public IList<PermissionDto> AreaAndActionAndControllerNamesList()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            var methods = asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Select(method => new
                {
                    Controller = method.DeclaringType,
                    Method = method,
                    Area = method.DeclaringType?.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                    NeedAccess =
                        method.DeclaringType.GetCustomAttribute<AuthorizeAttribute>() != null ||
                        method.GetCustomAttribute<AuthorizeAttribute>() != null
                });

            var list = new List<PermissionDto>();

            foreach (var item in methods)
            {
                list.Add(new PermissionDto
                {
                    AreaName = item.Area ?? "NoArea",
                    ControllerName = item.Controller.Name.Replace("Controller", ""),
                    ActionName = item.Method.Name,
                    NeedAccess = item.NeedAccess
                });
            }

            return list;
        }

        public IList<PermissionDto> GetAuthorizedControllers()
        {
            return AreaAndActionAndControllerNamesList()
                .Where(x => x.NeedAccess.Value)
                .ToList();
        }

        public IList<PermissionDto> GetUnAuthorizedControllers()
        {
            return AreaAndActionAndControllerNamesList()
                .Where(x => !x.NeedAccess.Value)
                .ToList();
        }


        public IList<PermissionDto> ApiAreaAndActionAndControllerNamesList()
        {
            // فقط اسمبلی پروژهٔ API خودت
            var asm = Assembly.GetEntryAssembly();

            var methods = asm.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type) ||
                    (type.BaseType != null && type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(CrudController<,>))) // ApiController + MVC
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                .Select(method => new
                {
                    Controller = method.DeclaringType,
                    Method = method,
                    Area = method.DeclaringType?.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                    NeedAccess =
                        method.DeclaringType.GetCustomAttribute<AuthorizeAttribute>() != null ||
                        method.GetCustomAttribute<AuthorizeAttribute>() != null
                });

            var list = new List<PermissionDto>();

            foreach (var item in methods)
            {
                list.Add(new PermissionDto
                {
                    AreaName = item.Area ?? "NoArea",
                    ControllerName = item.Controller.Name.Replace("Controller", ""),
                    ActionName = item.Method.Name,
                    NeedAccess = item.NeedAccess
                });
            }

            return list;
        }



    }
}
