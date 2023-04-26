using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
    public interface IBlogRepository
    {
        #region Phần B. Hướng dẫn

        // Tìm bài viết có tên định danh là 'slug'
        // và được đăng vào tháng 'month' năm 'year'
        public Task<Post> GetPostAsync(
            int year,
            int month,
            string slug,
            CancellationToken cancellationToken = default);

        // Tìm top N bài viết phổ biến được nhiều người xem nhất
        public Task<IList<Post>> GetPopularArticlesAsync(
            int numPosts,
            CancellationToken cancellationToken = default);

        // Kiểm tra xem tên định danh của bài viết đã có hay chưa
        public Task<bool> IsPostSlugExistedAsync(
            int postId, string slug,
            CancellationToken cancellationToken = default);

        // Tăng số lượt xem của một bài viết
        public Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken = default);

        // Lấy danh sách chuyên mục và số lượng bài viết
        // thuộc từng chuyên mục/chủ đề
        public Task<IList<CategoryItem>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default);

        // Lấy danh sách từ khóa/thẻ và phân trang theo
        // các tham số pagingParams
        public Task<IPagedList<TagItem>> GetPagedTagsAsync(
            IPagingParams pagingParams, CancellationToken cancellationToken = default);

        #endregion Phần B. Hướng dẫn

        #region Phần C.1. Thực hành

        #region Tag

        // 1.a. Tìm một thẻ (Tag) theo tên định danh (slug)
        public Task<Tag> FindTagWithSlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<Tag> GetTagByIdAsync(int Id);

        // 1.c. Lấy danh sách tất cả các thẻ (Tag) kèm theo
        // số bài viết chứa thẻ đó. Kết quả trả về kiểu IList<TagItem>.
        public Task<IList<TagItem>> GetTagsItemsAsync(
           CancellationToken cancellationToken = default);

        // 1.d. Xóa 1 thẻ theo mã cho trước
        public Task DeleteTagWithIdAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> DeleteTagByIdAsync(
             int tagId,
             CancellationToken cancellationToken = default);

        Task<Tag> CreateOrUpdateTagAsync(
        Tag tag, CancellationToken cancellationToken = default);

        Task<bool> IsTagSlugExistedAsync(
           int tagId,
           string slug,
           CancellationToken cancellationToken = default);

        Task<IPagedList<TagItem>> GetPagedTagAsync(
          int pageNumber = 1,
          int pageSize = 10,
          CancellationToken cancellationToken = default);

        Task<IPagedList<TagItem>> GetPagedTagsAsync(
          IPagingParams pagingParams,
          string name = null,
          CancellationToken cancellationToken = default);

        Task<Tag> GetCachedTagByIdAsync(int tagId);

        Task<Tag> GetCachedTagBySlugAsync(
        string slug, CancellationToken cancellationToken = default);

        Task<bool> AddOrUpdateTagAsync(
            Tag tag,
            CancellationToken cancellationToken = default);

        #endregion Tag

        #region Category

        //1.e: Tìm một chuyên mục(Category) theo tên định danh (slug)
        Task<Category> FindCategoryByUrlAsync(string slug, CancellationToken cancellationToken = default);

        // 1.f: Tìm 1 chuyên mục theo mã số
        Task<Category> FindCategoryByIDAsync(int id, CancellationToken cancellationToken = default);

        // 1.g: Thêm hoặc cập nhật một chuyên mục
        Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default);

        Task<bool> AddOrUpdateCategoryAsync(
     Category category,
     CancellationToken cancellationToken = default);

        // 1.h: Xóa một chuyên mục theo mã số
        Task<bool> DeleteCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> ToggleShowOnMenuFlagAsync(int categoryId, CancellationToken cancellationToken = default);

        // 1.i: Kiểm tra tên định danh (slug) của
        // một chuyên mục đã tồn tại hay chưa.
        Task<bool> IsCategorySlugExistedAsync(int categoryId, string slug, CancellationToken cancellationToken = default);

        // 1.j: Lấy và phân trang danh sách chuyên mục
        Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(
         int pageNumber = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default);

        Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
          IPagingParams pagingParams,
          string name = null,
          CancellationToken cancellationToken = default);

        Task<Category> GetCachedCategoryByIdAsync(int categoryId);

        Task<Category> GetCachedCategoryBySlugAsync(
        string slug, CancellationToken cancellationToken = default);

        #endregion Category

        #region Post

        Task<bool> DeletePostsByIdAsync(int postId, CancellationToken cancellationToken = default);

        Task<bool> TogglePuslishedFlagAsync(int postId, CancellationToken cancellationToken = default);

        // 1.k: Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào.
        // Kết quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số bài viết
        Task<IList<PostItem>> CountPostsMonthAsync(
        int n, CancellationToken cancellationToken = default);

        Task<Post> GetPostByIdAsync(
        int postId, bool includeDetails = false,
        CancellationToken cancellationToken = default);
        
        Task<Post> GetPostBySlugAsync(
        string postSlug, bool includeDetails = false,
        CancellationToken cancellationToken = default);

        Task<bool> SetImageUrlAsync(
       int postId, string imageUrl,
       CancellationToken cancellationToken = default);

        Task<bool> AddOrUpdatePostsAsync(
          Post post,
          CancellationToken cancellationToken = default);

        // 1.l: Tìm một bài viết theo mã số
        Task<Post> FindPostByIDAsync(int id, CancellationToken cancellationToken = default);

        // 1.m: Thêm hay cập nhật một bài viết
        Task AddOrUpdatePostAsync(Post postsName, CancellationToken cancellationToken = default);

        Task<Post> CreateOrUpdatePostAsync(
         Post post, IEnumerable<string> tags,
         CancellationToken cancellationToken = default);

        // 1.n: Chuyển đổi trạng thái Published của bài viết
        Task PublishedAsync(
        int id,
        CancellationToken cancellationToken = default);

        // 1.o: Chuyển đổi trạng thái Published của bài viết
        Task<IList<Post>> GetRandomPostsAsync(
            int n,
            CancellationToken cancellationToken = default);

        // 1.p: Tạo lớp PostQuery để lưu trữ các điều kiện tìm kiếm bài viết. Chẳng hạn:mã tác giả,
        // mã chuyên mục, tên ký hiệu chuyên mục, năm/tháng đăng bài, từ khóa, …
        Task<IList<Post>> GetAllPostsByPostQuery(
        PostQuery pquery, CancellationToken cancellationToken = default);

        // 1.q: Tìm tất cả bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng
        // PostQuery(kết quả trả về kiểu IList<Post>)
        Task<IList<Post>> FindAllPostsWithPostQueryAsync(
            PostQuery pq,
            CancellationToken cancellationToken = default);

        // 1.r: Đếm số lượng bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng PostQuery
        Task<int> CountPostsWithPostQueryAsync(
            PostQuery pq,
            CancellationToken cancellationToken = default);

        // 1.s: Tìm và phân trang các bài viết thỏa mãn điều kiện tìm kiếm được cho trong
        // đối tượng PostQuery(kết quả trả về kiểu IPagedList<Post>)
        Task<IPagedList<Post>> GetPagedsPostAsync(PostQuery pq,
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default);

        IQueryable<Post> FilterPost(PostQuery pq);

        Task<IPagedList<T>> GetPagedPostsAsync<T>(
           PostQuery condition,
           IPagingParams pagingParams,
           Func<IQueryable<Post>, IQueryable<T>> mapper);

        Task<IPagedList<Post>> GetPagedPostAsync(
                PostQuery pq,
                int pageNumber = 1,
                int pageSize = 10,
                CancellationToken cancellationToken = default);

        //Task <IPagedList<Post>> GetPagedPostQueryAsync(
        //   PostQuery pq,
        //   IPagingParams pagingParams,
        //   CancellationToken cancellationToken = default);
        //Task <IPagedList<Post>> GetPagedPostQueryAsync(
        //    PostQuery pq,
        //    int pageNumber,
        //    int pageSize,
        //    CancellationToken cancellationToken = default);

        // 1.t: Tương tự câu trên nhưng yêu cầu trả về kiểu IPagedList<T>. Trong đó T
        // là kiểu dữ liệu của đối tượng mới được tạo từ đối tượng Post.Hàm này có
        // thêm một đầu vào là Func<IQueryable<Post>, IQueryable<T>> mapper
        // để ánh xạ các đối tượng Post thành các đối tượng T theo yêu cầu
        Task<IPagedList<T>> GetPagedPostQueryAsync<T>(
           PostQuery pq,
           Func<IQueryable<Post>, IQueryable<T>> mapper,
           CancellationToken cancellationToken = default);

        Task<Post> GetCachedPostByIdAsync(int postId);

        #endregion Post

        #endregion Phần C.1. Thực hành

        #region Comment
        Task<IList<Comment>> GetCommentsAsync(bool isApproved = false, 
            CancellationToken cancellationToken = default);
        Task<IPagedList<Comment>> GetPagedCommentAsync(
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        // Tìm 1 bình luận theo mã số
        Task<Comment> GetCommentByIDAsync(int id, CancellationToken cancellationToken = default);

        Task<Comment> GetCachedCommentByIdAsync(int commentId);

        // 1.g: Thêm hoặc cập nhật một bình luận
        Task<Comment> CreateOrUpdateCommentAsync(
        Comment comment, CancellationToken cancellationToken = default);

        Task<bool> AddOrUpdateCommentAsync(
            Comment comment,
            CancellationToken cancellationToken = default);

        Task<bool> ChangeCommentByIdAsync(
        int id, CancellationToken cancellationToken = default);

        Task<IPagedList<Comment>> GetPagedCommentsAsync(
            IPagingParams pagingParams,
            string name = null,
            CancellationToken cancellationToken = default);

        Task<IList<Comment>> GetPostCommentsAsync(
            int commentId, CancellationToken cancellationToken = default);

        Task<bool> DeleteCommentByIdAsync(int commentId,
        CancellationToken cancellationToken = default);

        Task<bool> ToggleApprovedFlagAsync(int commentId,
        CancellationToken cancellationToken = default);

        Task<bool> IsCommentExistedAsync(
           int commentId,
           string slug,
           CancellationToken cancellationToken = default);

        #endregion Comment

        #region Dashboard

        Task<int> CountPostAsync(
        CancellationToken cancellationToken = default);

        Task<int> CountUnPublicPostAsync(
        CancellationToken cancellationToken = default);

        Task<int> CountCategoryAsync(
        CancellationToken cancellationToken = default);

        Task<int> CountCommentAsync(
        CancellationToken cancellationToken = default);

        //  Task<int> CountSubAsync(
        //CancellationToken cancellationToken = default);

        //  Task<int> CountSubscriberStateAsync(
        //     CancellationToken cancellationToken = default);

        #endregion Dashboard
    }
}