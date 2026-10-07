using Entities.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Entities.User
{
    public class UserRole : BaseEntity
    {

        public long UserId { get; set; }

        public long RoleId { get; set; }

        public long LastRoleInUse_Tiks { get; set; } 

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
    }



    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Table Name
            builder.ToTable("UserRoles");

            // Primary Key
            builder.HasKey(ur => ur.Id);

            // Properties
            builder.Property(ur => ur.UserId)
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .IsRequired();

            // Unique Constraint (User cannot have same Role twice)
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // Relation: UserRole → User
            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: UserRole → Role
            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
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
