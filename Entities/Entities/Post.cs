using Entities.Entities.BaseEntities;
using Entities.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public long AuthorId { get; set; }

        public Category Category { get; set; }
        public User Author { get; set; }
    }

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            // Table Name
            builder.ToTable("Post");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired();

            builder.Property(p => p.AuthorId)
                .IsRequired();

            // Indexes
            builder.HasIndex(p => p.Title);
            builder.HasIndex(p => p.CategoryId);
            builder.HasIndex(p => p.AuthorId);

            // Relations: Post → Category
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations: Post → Author (User)
            builder.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

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
