namespace Entities.Entities.BaseEntities
{


    public interface IEntity
    {

    }


    public class BaseEntity/*<EntityTypeKey>*/ : IEntity
    {

        public long Id { get; set; }

        public DateTime? CreateAt { get; set; }

        public long? CreateByUserId { get; set; }

        public DateTime? UpdateAt { get; set; }

        public long? UpdateByUserId { get; set; }

        public byte[] RowVersion { get; set; }

        public bool? IsDeleted { get; set; } = false;

        public long? DeletedByUserId { get; set; }
        public DateTime? DeletedAt { get; set; }


    }

    //public class BaseEntity : BaseEntity<long>
    //{

    //}

}
