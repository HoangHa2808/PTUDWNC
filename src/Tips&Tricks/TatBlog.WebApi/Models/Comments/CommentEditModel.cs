namespace TatBlog.WebApi.Models.Comments
{
    public class CommentEditModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public string PostId { get; set; }
    }
}