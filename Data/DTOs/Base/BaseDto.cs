using AutoMapper;
using Common.Utilities;

namespace Data.DTOs.Base;

public abstract class BaseDto<TDto, TEntity, TKey>
    where TDto : class, new()
    where TEntity : class, new()
{
    protected IMapper _mapper => MapperProvider.Mapper;


    public TKey? Id { get; set; }

    public byte[] RowVersion { get; set; }

    public TEntity ToEntity()
    {
        return _mapper.Map<TEntity>(CastToDerivedClass(this));
    }

    public TEntity ToEntity(TEntity entity)
    {
        return _mapper.Map(CastToDerivedClass(this), entity);
    }

    public static TDto FromEntity(TEntity model, IMapper mapper)
    {
        return mapper.Map<TDto>(model);
    }

    protected TDto CastToDerivedClass(BaseDto<TDto, TEntity, TKey> baseInstance)
    {
        return _mapper.Map<TDto>(baseInstance);
    }

    public void CreateMappings(Profile profile)
    {
        var mappingExpression = profile.CreateMap<TDto, TEntity>();

        var dtoType = typeof(TDto);
        var entityType = typeof(TEntity);

        foreach (var property in entityType.GetProperties())
        {
            if (dtoType.GetProperty(property.Name) == null)
                mappingExpression.ForMember(property.Name, opt => opt.Ignore());
        }

        CustomMappings(mappingExpression.ReverseMap());
    }

    public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
    {
    }
}

public abstract class BaseDto<TDto, TEntity> : BaseDto<TDto,TEntity,long>
    where TDto : class, new()
    where TEntity : class, new()
{
    
}
