using Data.Contract.GenericContract;
using Data.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    public class CategoryController : CrudController<CategoryDto, Category>
    {
        public CategoryController(IRepository<Category> repository) : base(repository)
        {
        }

    }
}
