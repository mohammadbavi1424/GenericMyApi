using Data.Contract;
using Data.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace MyApi.Controllers.v1
{
    //[Route("api/[controller]/[action]")]
    //[ApiController]
    //[ApiResultFilter]
    [ApiVersion("1")]
    public class AccountController : BaseController
    {
        private readonly IJWTRepository jWTRepository;

        public AccountController(IJWTRepository jWTRepository)
        {
            this.jWTRepository = jWTRepository;
        }


        /// <summary>
        /// This method is for generation token
        /// </summary>
        /// <param name="dto">This model need UserName of user and Password of user</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var jwt = await jWTRepository.GeneratToken(dto, cancellationToken);

            return Ok(jwt);
        }


    }
}
