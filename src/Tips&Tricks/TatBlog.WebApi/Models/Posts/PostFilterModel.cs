namespace TatBlog.WebApi.Models.Posts
{
    public class PostFilterModel : PagingModel
    {
        public string Title { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }

        public string Keyword { get; set; }
    }
}