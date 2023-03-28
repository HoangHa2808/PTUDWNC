using TatBlog.Core.Contracts;

namespace TatBlog.WebApi.Models
{
    public class PagingModel : IPagingParams
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public string SortColumn { get; set; } = "Id";
        public string SortOrder { get; set; } = "DESC";
    }
}
