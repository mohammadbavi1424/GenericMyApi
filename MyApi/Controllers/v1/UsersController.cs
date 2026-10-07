
using AutoMapper;
using Core.Utilities.Mapping;
using Data.Contract;
using Data.Contract.GenericContract;
using Data.DTOs.Users;
using ElmahCore;
using Entities.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Api;
using WebFramework.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class UsersController : BaseController
    {

        private readonly IUserRepository _userRepository;
        private readonly IRepository<User> repository;

        public UsersController(IUserRepository userRepository,IRepository<User> repository)
        {
            _userRepository = userRepository;
            this.repository = repository;
        }

        // GET: api/<UsersController>
        [HttpGet]

        public async Task<List<User>> Get(CancellationToken cancellationToken)
        {
            //var result = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
            var result = await _userRepository.GetByQueryAsync(cancellationToken);
            return result.ToList();

        }

        [HttpGet]

        public async Task<ActionResult<List<User>>> GetDeleted(CancellationToken cancellationToken)
        {
            var result = (await _userRepository.TableNoTrackingDeleted
                .ToListAsync(cancellationToken));
            if (result == null || result.Count == 0) return NotFound();
            return Ok(result);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]

        public async Task<ActionResult<UserDto>> Get(long id, CancellationToken cancellationToken)
        {

            var dto = await repository.GetDtos<UserDto>(c => c.Id == id, cancellationToken);
            var res = dto.FirstOrDefault();

            return res;

            var result = await _userRepository.GetByIdQuerAsync(id);
            if (result == null) return NotFound();
            var userDto = result.ConvertObject<UserDto, User>();
            return userDto;
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<User>> GetDeleted(long id, CancellationToken cancellationToken)
        {
            var result = (await _userRepository
                .GetByIdDeletedAsync(cancellationToken, id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]

        public async Task<ActionResult<User>> Add(UserDto userDto, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                await HttpContext.RiseError(new Exception("متد Add فراخوانی شد."));

                var user = userDto.ConvertObject<User, UserDto>();
                await _userRepository.AddAsync(user, userDto.Password, cancellationToken);
                return Ok(user);
            }
            return BadRequest();
        }
        //[HttpPost]
        //public async Task<IActionResult> Add(string user, CancellationToken cancellationToken)
        //{
        //    var userItem = JsonSerializer.Deserialize<User>(user);
        //    await _userRepository.AddAsync(userItem, cancellationToken);
        //    return Ok();
        //}
        // PUT api/<UsersController>/5
        [HttpPut]

        public async Task<ActionResult<User>> Update(User user, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _userRepository.UpdateAsync(user, cancellationToken);
            return Ok(user);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            await _userRepository.DeleteAsync(user, cancellationToken);
            return Ok();
        }
    }
}
