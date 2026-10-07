using Core.Utilities.Mapping;
using Data.Contract;
using Data.Contract.GenericContract;
using Data.DTOs.Base;
using Data.DTOs.Users;
using ElmahCore;
using Entities.Entities.BaseEntities;
using Entities.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.Filters;

namespace WebFramework.Api
{
    [ApiVersion("1")]
    public class CrudController<TDto,TDtoSelect,Tentity> : BaseController
        where TDto : BaseDto<TDto,Tentity>, new()
        where TDtoSelect : BaseDto<TDtoSelect,Tentity>, new()
        where Tentity : BaseEntity, new()
    {

        private readonly IRepository<Tentity> repository;

        public CrudController(IRepository<Tentity> repository)
        {
            this.repository = repository;
        }

        // GET: api/<UsersController>
        [HttpGet]

        public virtual async Task<List<TDto>> Get(CancellationToken cancellationToken)
        {
            //var result = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
            var result = await repository.GetByQueryAsync(cancellationToken);
            return result.ToList().ConvertListObject<TDto,Tentity>();

        }

        [HttpGet]

        public virtual async Task<ActionResult<List<TDto>>> GetDeleted(CancellationToken cancellationToken)
        {
            var result = await repository.TableNoTrackingDeleted.ToListAsync(cancellationToken);
            if (result == null || result.Count == 0) return NotFound();

            return Ok(result.ConvertListObject<TDto,Tentity>());
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]

        public virtual async Task<TDto> Get(long id, CancellationToken cancellationToken)
        {

            var dto = await repository.GetDtos<TDto>(c => c.Id == id, cancellationToken);
            var res = dto.FirstOrDefault();

            return res;

            //var result = await _userRepository.GetByIdQuerAsync(id);
            //if (result == null) return NotFound();
            //var userDto = result.Convertentityect<UserDto, User>();
            //return userDto;
        }

        [HttpGet("{id}")]

        public virtual async Task<ActionResult<TDto>> GetDeleted(long id, CancellationToken cancellationToken)
        {
            var result = (await repository
                .GetByIdDeletedAsync(cancellationToken, id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST api/<UsersController>
        [HttpPost]

        public virtual async Task<ActionResult<TDto>> Add(TDto dto, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var entity = dto.ConvertObject<Tentity, TDto>();
                await repository.AddAsync(entity, cancellationToken);
                return Ok(dto);
            }
            return BadRequest();
        }
        



        // PUT api/<UsersController>/5
        [HttpPut]

        public virtual async Task<ActionResult<TDto>> Update(TDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var entity = dto.ConvertObject<Tentity, TDto>();
            await repository.UpdateAsync(entity, cancellationToken);
            return Ok(dto);
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]

        public virtual async Task<ActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            var entity = await repository.GetByIdAsync(cancellationToken, id);
            await repository.DeleteAsync(entity, cancellationToken);
            return Ok();
        }
    }

    public class CrudController<TDto, Tentity> : CrudController<TDto, TDto, Tentity>
        where TDto : BaseDto<TDto, Tentity>, new()
        where Tentity : BaseEntity, new()
    {
        public CrudController(IRepository<Tentity> repository)
            : base( repository)
        {
        }
    }

}
