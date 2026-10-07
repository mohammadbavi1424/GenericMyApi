using Entities.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Entities.User
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public ICollection<UserRole> UserRoles { get; set; }

    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Table Name
            builder.ToTable("Role");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(r => r.Name).IsUnique();

            // Relations
            builder.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
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
