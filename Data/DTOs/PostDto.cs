using AutoMapper;
using Data.DTOs.Base;
using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTOs
{
    public class PostDto : BaseDto<PostDto,Post>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public long AuthorId { get; set; }
    }

    public class PostSelectDto : BaseDto<PostSelectDto, Post>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; } //=> Category.Name
        public string AuthorFullName { get; set; } //=> Author.FullName
        public string FullTitle { get; set; } // => mapped from "Title (Category.Name)"

        public override void CustomMappings(IMappingExpression<Post, PostSelectDto> mappingExpression)
        {
            mappingExpression.ForMember(
                    dest => dest.FullTitle,
                    config => config.MapFrom(src => $"{src.Title} ({src.Category.Name})"));
        }
    }
}
