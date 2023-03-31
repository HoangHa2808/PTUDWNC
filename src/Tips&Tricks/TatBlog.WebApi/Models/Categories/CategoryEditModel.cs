namespace TatBlog.WebApi.Models.Categories
{
    public class CategoryEditModel
    {
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public string Description { get; set; }
        public bool ShowOnMenu { get; set; }
    }
}