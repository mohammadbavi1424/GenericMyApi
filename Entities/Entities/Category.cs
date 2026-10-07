using Entities.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class Category : BaseEntity
    {

        public string Name { get; set; }
        public long? ParentCategoryId { get; set; }

        [ForeignKey(nameof(ParentCategoryId))]
        public Category ParentCategory { get; set; }
        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }

    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Table Name
            builder.ToTable("Category");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ParentCategoryId)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(c => c.Name);

            // Self Reference Relation (Parent → Children)
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // جلوگیری از Cascade Loop

            // Relation with Posts
            builder.HasMany(c => c.Posts)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
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
