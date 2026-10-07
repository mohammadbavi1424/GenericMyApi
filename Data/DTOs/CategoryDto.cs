using Data.DTOs.Base;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTOs
{
    public class CategoryDto : BaseDto<CategoryDto, Category>
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; } //=> mapped from ParentCategory.Name
    }
}
