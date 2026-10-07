using Entities.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Entities.Entities.User
{
    public class User : BaseEntity
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string NationalCode { get; set; }

        public Gender Gender { get; set; }

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public DateTimeOffset LastLoginDate { get; set; } = DateTime.Now;

        //public long? LastLoginRoleId { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new Collection<Post>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();


    }

    public enum Gender
    {
        [Display(Name = "مرد")]
        Male = 0,

        [Display(Name = "زن")]
        Famale = 1,

        [Display(Name = "نا مشخص")]
        Unknow = 2,
    }


    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table Name
            builder.ToTable("User");

            // Primary Key
            builder.HasKey(u => u.Id);

            // Properties
            builder.Property(u => u.FirstName)
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .HasMaxLength(100);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Email)
                .HasMaxLength(150);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.NationalCode)
                .HasMaxLength(10);

            builder.Property(u => u.Gender)
                .IsRequired();

            // Indexes
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique(false);
            builder.HasIndex(u => u.PhoneNumber).IsUnique(false);

            // Relations
            builder.HasMany(u => u.Posts)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.CreateByUserId)
                .IsRequired(false);
            builder.Property(p => p.CreateAt)
                .IsRequired(false);
            builder.Property(p => p.UpdateByUserId)
                .IsRequired(false);
            builder.Property(p => p.UpdateAt)
                .IsRequired(false);
            builder.Property(p => p.DeletedByUserId)
                .IsRequired(false);
            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
            builder.Property(p => p.DeletedAt)
                .IsRequired(false);

        }
    }

}
