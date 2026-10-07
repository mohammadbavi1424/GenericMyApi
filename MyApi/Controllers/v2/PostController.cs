using Data.Contract.GenericContract;
using Data.DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers.v2
{
    [ApiVersion("2")]
    public class PostController : v1.PostController
    {
        public PostController(IRepository<Post> repository)
            : base(repository)
        {
        }


        public override Task<List<PostDto>> Get(CancellationToken cancellationToken)
        {
            var res = true;
            return base.Get(cancellationToken);
        }

    }
}
