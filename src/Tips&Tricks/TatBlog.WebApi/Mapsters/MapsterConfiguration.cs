using Mapster;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.WebApi.Models.Authors;
using TatBlog.WebApi.Models.Categories;
using TatBlog.WebApi.Models.Posts;
using TatBlog.WebApi.Models.Tags;

namespace TatBlog.WebApi.Mapsters
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Author, AuthorDTO>();
            config.NewConfig<Author, AuthorItem>()
                .Map(dest => dest.PostCount,
                src => src.Posts == null ? 0 : src.Posts.Count);

            config.NewConfig<AuthorEditModel, Author>();

            config.NewConfig<Category, CategoryDTO>();
            config.NewConfig<Category, CategoryItem>()
                .Map(dest => dest.PostCount,
                src => src.Posts == null ? 0 : src.Posts.Count);

            config.NewConfig<Post, PostDTO>();
            config.NewConfig<Post, PostDetail>();

            config.NewConfig<Tag, TagDTO>();
            config.NewConfig<Tag, TagItem>()
                .Map(dest => dest.PostCount,
                src => src.Posts == null ? 0 : src.Posts.Count);
        }
    }
}