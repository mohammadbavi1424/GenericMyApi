using Data.Contract.GenericContract;
using Data.DTOs;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    
    public class PostController : CrudController<PostDto, Post>
    {
        public PostController(IRepository<Post> repository) 
            : base(repository)
        {
        }
    }
}
