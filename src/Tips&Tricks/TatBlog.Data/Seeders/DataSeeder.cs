using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders;

public class DataSeeder : IDataSeeder
{
    private readonly BlogDbContext _dbContext;

    public DataSeeder(BlogDbContext dbContext)
    { _dbContext = dbContext; }

    public void Initialize()
    {
        _dbContext.Database.EnsureCreated();
        if (_dbContext.Posts.Any()) return;
        var authors = AddAuthors();
        var categories = AddCategories();
        var tags = AddTags();
        var posts = AddPosts(authors, categories, tags);
    }

    private IList<Author> AddAuthors()
    {
        var authors = new List<Author>()
        {
            new()
            {
                FullName = "Jason Mouth",
                UrlSlug = "jason-mouth",
                Email = "json@gmail.com",
                JoinedDate = new DateTime(2022, 10, 21)
            },

            new()
            {
                FullName = "Jessica Wonder",
                UrlSlug = "jessica-wonder",
                Email = "jessica665@motip.com",
                JoinedDate = new DateTime(2022, 4, 19)
            },

            new()
            {
                FullName = "Author1",
                UrlSlug = "author1",
                Email = "author1@motip.com",
                JoinedDate = new DateTime(2022, 5, 19)
            },

            new()
            {
                FullName = "Author2",
                UrlSlug = "author2",
                Email = "author2@gmail.com",
                JoinedDate = new DateTime(2022, 4, 12)
            },

            new()
            {
                FullName = "Author3",
                UrlSlug = "author3",
                Email = "author3@email.com",
                JoinedDate = new DateTime(2022, 6, 19)
            }
        };
        foreach (var author in authors)
        {
            if (!_dbContext.Authors.Any(a => a.UrlSlug == author.UrlSlug))
                _dbContext.Authors.Add(author);
        }
        //_dbContext.Authors.AddRange(authors);
        _dbContext.SaveChanges();
        return authors;
    }
    private IList<Category> AddCategories()
    {
        var categories = new List<Category>()
        {
                new() {Name = ".NET Core", Description = ".NET Core", UrlSlug = "dotnet-core"},
                new() {Name = "Architecture", Description = "Architecture", UrlSlug = "architecture"},
                new() {Name = "Messaging", Description = "Messaging", UrlSlug = "messaging"},
                new() {Name = "OOP", Description = "Object-Oriented Programming", UrlSlug = "oop"},
                new() {Name = "Design Patterns", Description = "Design Patterns", UrlSlug = "design-patterns"},
                new() {Name = ".NET Core", Description = "Category 1", UrlSlug = "category-1"},
                new() {Name = "Category 2", Description = "Category 2", UrlSlug = "category-2"},
                new() {Name = "OOP", Description = "Category 3", UrlSlug = "category-3"},
                new() {Name = "Category 4", Description = "Category 4", UrlSlug = "category-4"},
                new() {Name = "Category 5", Description = "Category 5", UrlSlug = "category-5"},
        };
        foreach (var category in categories)
        {
            if (!_dbContext.Categories.Any(a => a.UrlSlug == category.UrlSlug))
                _dbContext.Categories.Add(category);
        }
        //_dbContext.Categories.AddRange(categories);
        _dbContext.SaveChanges();

        return categories;

    }
    private IList<Tag> AddTags()
    {
        var tags = new List<Tag>()
            {
                new() {Name = "Google", Description = "Google applications", UrlSlug = "google"},
                new() {Name = "ASP.NEW MVC", Description = "ASP.NEW MVC", UrlSlug = "aspdotnet-mvc"},
                new() {Name = "Razor Page", Description = "Razor Page", UrlSlug = "razor-page"},
                new() {Name = "Blazor", Description = "Blazor", UrlSlug = "blazor"},
                new() {Name = "Deep Learning", Description = "Deep Learning", UrlSlug = "deep-learning"},
                new() {Name = "Neural Network", Description = "Neural Network", UrlSlug = "neural-network"},
                new() {Name = "Tag 7", Description = "Tag 7", UrlSlug = "tag-7"},
                new() {Name = "Tag 8", Description = "Tag 8", UrlSlug = "tag-8"},
                new() {Name = "Tag 9", Description = "Tag 9", UrlSlug = "tag-9"},
                new() {Name = "Tag 10", Description = "Tag 10", UrlSlug = "tag-10"},
                new() {Name = "Tag 11", Description = "Tag 11", UrlSlug = "tag-11"},
                new() {Name = "Tag 12", Description = "Tag 12", UrlSlug = "tag-12"},
                new() {Name = "Tag 13", Description = "Tag 13", UrlSlug = "tag-13"},
                new() {Name = "Tag 14", Description = "Tag 14", UrlSlug = "tag-14"},
                new() {Name = "Tag 15", Description = "Tag 15", UrlSlug = "tag-15"},
                new() {Name = "Tag 16", Description = "Tag 16", UrlSlug = "tag-16"},
                new() {Name = "Tag 17", Description = "Tag 17", UrlSlug = "tag-17"},
                new() {Name = "Tag 18", Description = "Tag 18", UrlSlug = "tag-18"},
                new() {Name = "Tag 19", Description = "Tag 19", UrlSlug = "tag-19"},
                new() {Name = "Tag 20", Description = "Tag 20", UrlSlug = "tag-20"}
        };
        foreach (var tag in tags)
        {
            if (!_dbContext.Tags.Any(t => t.UrlSlug == tag.UrlSlug))
                _dbContext.Tags.Add(tag);
        }

        //_dbContext.Tags.AddRange(tags);
        _dbContext.SaveChanges();

        return tags;
    }
    private IList<Post> AddPosts(
        IList<Author> authors,
        IList<Category> categories,
        IList<Tag> tags)
    {
        var posts = new List<Post>()
        {
            new()
            {
                Title = "ASP.NET Core Diagnostic Scenarios",
                ShortDescription = "David and friends has a great repository",
                Description = "Here's a few great DON'T and Do examples",
                Meta = "David and friends has a great repository",
                UrlSlug = "aspdotnet-core-diagnostic-scenarios",
                Published = true,
                PostedDate = new DateTime(2021, 9, 30, 10, 20, 0),
                ModifiedDate = null,
                ViewCount = 10,
                Author = authors[0],
                Category = categories[0],
                Tags = new List<Tag>()
                {
                    tags[0]
                }
            },

            new()
            {
                Title = "Post 1",
                ShortDescription = "Post 1",
                Description = "Post 1 Discription",
                Meta = "Post 1",
                UrlSlug = "post-1",
                Published = true,
                PostedDate = new DateTime(2021, 9, 20, 10, 5, 0),
                ModifiedDate = null,
                ViewCount = 7,
                Author = authors[1],
                Category = categories[1],
                Tags = new List<Tag>()
                {
                    tags[1]
                }
            },

            new()
            {
                Title = "Post 2",
                ShortDescription = "Post 2",
                Description = "Post 2 Discription",
                Meta = "Post 2",
                UrlSlug = "post-2",
                Published = true,
                PostedDate = new DateTime(2020, 5, 20, 6, 5, 0),
                ModifiedDate = null,
                ViewCount = 4,
                Author = authors[2],
                Category = categories[2],
                Tags = new List<Tag>()
                {
                    tags[2]
                }
            },

            new()
            {
                Title = "Post 3",
                ShortDescription = "Post 3",
                Description = "Post 3 Discription",
                Meta = "Post 3",
                UrlSlug = "post-3",
                Published = true,
                PostedDate = new DateTime(2021, 9, 15, 10, 5, 2),
                ModifiedDate = null,
                ViewCount = 6,
                Author = authors[3],
                Category = categories[3],
                Tags = new List<Tag>()
                {
                    tags[3]
                }
            },
            new()
            {
                Title = "Post 4",
                ShortDescription = "Post 4",
                Description = "Post 4 Discription",
                Meta = "Post 4",
                UrlSlug = "post-4",
                Published = true,
                PostedDate = new DateTime(2021, 9, 15, 10, 13, 0),
                ModifiedDate = null,
                ViewCount = 2,
                Author = authors[4],
                Category = categories[8],
                Tags = new List<Tag>()
                {
                    tags[10]
                }
            },
            new()
            {
                Title = "Post 5",
                ShortDescription = "Post 5",
                Description = "Post 5 Discription",
                Meta = "Post 5",
                UrlSlug = "post-5",
                Published = true,
                PostedDate = new DateTime(2021, 9, 20, 12, 5, 0),
                ModifiedDate = null,
                ViewCount = 9,
                Author = authors[0],
                Category = categories[9],
                Tags = new List<Tag>()
                {
                    tags[12]
                }
            },
            new()
            {
                Title = "Post 6",
                ShortDescription = "Post 6",
                Description = "Post 6 Discription",
                Meta = "Post 6",
                UrlSlug = "post-6",
                Published = true,
                PostedDate = new DateTime(2021, 8, 20, 4, 5, 0),
                ModifiedDate = null,
                ViewCount = 4,
                Author = authors[3],
                Category = categories[9],
                Tags = new List<Tag>()
                {
                    tags[15]
                }
            },
            new()
            {
                Title = "Post 7",
                ShortDescription = "Post 7",
                Description = "Post 7 Discription",
                Meta = "Post 7",
                UrlSlug = "post-7",
                Published = true,
                PostedDate = new DateTime(2021, 9, 15, 13, 5, 0),
                ModifiedDate = null,
                ViewCount = 5,
                Author = authors[2],
                Category = categories[8],
                Tags = new List<Tag>()
                {
                    tags[19]
                }
            },
            new()
            {
                Title = "Post 8",
                ShortDescription = "Post 8",
                Description = "Post 8 Discription",
                Meta = "Post 8",
                UrlSlug = "post-8",
                Published = true,
                PostedDate = new DateTime(2022, 9, 20, 10, 5, 0),
                ModifiedDate = null,
                ViewCount = 4,
                Author = authors[4],
                Category = categories[5],
                Tags = new List<Tag>()
                {
                    tags[19]
                }
            },
            new()
            {
                Title = "Post 9",
                ShortDescription = "Post 9",
                Description = "Post 9 Discription",
                Meta = "Post 9",
                UrlSlug = "post-9",
                Published = true,
                PostedDate = new DateTime(2022, 10, 20, 10, 5, 0),
                ModifiedDate = null,
                ViewCount = 2,
                Author = authors[1],
                Category = categories[2],
                Tags = new List<Tag>()
                {
                    tags[14]
                }
            },
            new()
            {
                Title = "Post 10",
                ShortDescription = "Post 10",
                Description = "Post 10 Discription",
                Meta = "Post 10",
                UrlSlug = "post-10",
                Published = true,
                PostedDate = new DateTime(2022, 7, 15, 10, 5, 0),
                ModifiedDate = null,
                ViewCount = 7,
                Author = authors[1],
                Category = categories[6],
                Tags = new List<Tag>()
                {
                    tags[12]
                }
            },
            new()
            {
                Title = "Post 11",
                ShortDescription = "Post 11",
                Description = "Post 11 Discription",
                Meta = "Post 11",
                UrlSlug = "post-11",
                Published = true,
                PostedDate = new DateTime(2022, 4, 20, 7, 5, 0),
                ModifiedDate = null,
                ViewCount = 7,
                Author = authors[0],
                Category = categories[4],
                Tags = new List<Tag>()
                {
                    tags[16]
                }
            },
            new()
            {
                Title = "Post 12",
                ShortDescription = "Post 12",
                Description = "Post 12 Discription",
                Meta = "Post 12",
                UrlSlug = "post-12",
                Published = true,
                PostedDate = new DateTime(2022, 9, 14, 10, 5, 0),
                ModifiedDate = null,
                ViewCount = 7,
                Author = authors[1],
                Category = categories[1],
                Tags = new List<Tag>()
                {
                    tags[14]
                }
            },
            new() {
                    Title = "Post 13",
                    ShortDescription = "Post 13",
                    Description = "Post 13 description",
                    Meta = "Post 13",
                    UrlSlug = "post-13",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[10]
                    }
            },
            new() {
                    Title = "Post 14",
                    ShortDescription = "Post 14",
                    Description = "Post 14 description",
                    Meta = "Post 14",
                    UrlSlug = "post-14",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[2],
                    Category = categories[6],
                    Tags = new List<Tag>()
                    {
                        tags[10]
                    }
            },
            new() {
                    Title = "Post 15",
                    ShortDescription = "Post 15",
                    Description = "Post 15 description",
                    Meta = "Post 15",
                    UrlSlug = "post-15",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[15]
                    }
            },
            new() {
                    Title = "Post 16",
                    ShortDescription = "Post 16",
                    Description = "Post 16 description",
                    Meta = "Post 16",
                    UrlSlug = "post-16",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[3],
                    Tags = new List<Tag>()
                    {
                        tags[5]
                    }
            },
            new() {
                    Title = "Post 17",
                    ShortDescription = "Post 17",
                    Description = "Post 17 description",
                    Meta = "Post 17",
                    UrlSlug = "post-17",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[10]
                    }
            },
            new() {
                    Title = "Post 18",
                    ShortDescription = "Post 18",
                    Description = "Post 18 description",
                    Meta = "Post 18",
                    UrlSlug = "post-18",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[0],
                    Category = categories[0],
                    Tags = new List<Tag>()
                    {
                        tags[7]
                    }
            },
            new() {
                    Title = "Post 19",
                    ShortDescription = "Post 19",
                    Description = "Post 19 description",
                    Meta = "Post 19",
                    UrlSlug = "post-19",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[0],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[18]
                    }
            },
            new() {
                    Title = "Post 20",
                    ShortDescription = "Post 20",
                    Description = "Post 20 description",
                    Meta = "Post 20",
                    UrlSlug = "post-20",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[14]
                    }
            },
            new() {
                    Title = "Post 21",
                    ShortDescription = "Post 21",
                    Description = "Post 21 description",
                    Meta = "Post 21",
                    UrlSlug = "post-21",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 1, 10, 20, 4),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[1],
                    Category = categories[2],
                    Tags = new List<Tag>()
                    {
                        tags[12]
                    }
            },
            new() {
                    Title = "Post 22",
                    ShortDescription = "Post 22",
                    Description = "Post 22 description",
                    Meta = "Post 22",
                    UrlSlug = "post-22",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 7, 10, 9, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[10]
                    }
            },
            new() {
                    Title = "Post 23",
                    ShortDescription = "Post 23",
                    Description = "Post 23 description",
                    Meta = "Post 23",
                    UrlSlug = "post-23",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 20, 10, 5, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[1],
                    Category = categories[9],
                    Tags = new List<Tag>()
                    {
                        tags[6]
                    }
            },
            new() {
                    Title = "Post 24",
                    ShortDescription = "Post 24",
                    Description = "Post 24 description",
                    Meta = "Post 24",
                    UrlSlug = "post-24",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 20, 8, 0),
                    ModifiedDate = null,
                    ViewCount = 2,
                    Author = authors[0],
                    Category = categories[1],
                    Tags = new List<Tag>()
                    {
                        tags[12]
                    }
            },
            new() {
                    Title = "Post 25",
                    ShortDescription = "Post 25",
                    Description = "Post 25 description",
                    Meta = "Post 25",
                    UrlSlug = "post-25",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 10, 5, 0),
                    ModifiedDate = null,
                    ViewCount = 11,
                    Author = authors[1],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[7]
                    }
            },
            new() {
                    Title = "Post 26",
                    ShortDescription = "Post 26",
                    Description = "Post 26 description",
                    Meta = "Post 26",
                    UrlSlug = "post-26",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 13, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[1],
                    Category = categories[9],
                    Tags = new List<Tag>()
                    {
                        tags[15]
                    }
            },
            new() {
                    Title = "Post 27",
                    ShortDescription = "Post 27",
                    Description = "Post 27 description",
                    Meta = "Post 27",
                    UrlSlug = "post-27",
                    Published = true,
                    PostedDate = new DateTime(2022, 2, 3, 11, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 12,
                    Author = authors[3],
                    Category = categories[8],
                    Tags = new List<Tag>()
                    {
                        tags[11]
                    }
            },
            new() {
                    Title = "Post 28",
                    ShortDescription = "Post 28",
                    Description = "Post 28 description",
                    Meta = "Post 28",
                    UrlSlug = "post-28",
                    Published = true,
                    PostedDate = new DateTime(2022, 4, 3, 10, 19, 0),
                    ModifiedDate = null,
                    ViewCount = 0,
                    Author = authors[4],
                    Category = categories[5],
                    Tags = new List<Tag>()
                    {
                        tags[11]
                    }
            },
            new() {
                    Title = "Post 29",
                    ShortDescription = "Post 29",
                    Description = "Post 29 description",
                    Meta = "Post 29",
                    UrlSlug = "post-29",
                    Published = true,
                    PostedDate = new DateTime(2022, 4, 12, 10, 11, 0),
                    ModifiedDate = null,
                    ViewCount = 4,
                    Author = authors[4],
                    Category = categories[5],
                    Tags = new List<Tag>()
                    {
                        tags[8]
                    }
            },
            new() {
                    Title = "Post 30",
                    ShortDescription = "Post 30",
                    Description = "Post 30 description",
                    Meta = "Post 30",
                    UrlSlug = "post-30",
                    Published = true,
                    PostedDate = new DateTime(2022, 1, 11, 10, 14, 0),
                    ModifiedDate = null,
                    ViewCount = 14,
                    Author = authors[4],
                    Category = categories[5],
                    Tags = new List<Tag>()
                    {
                        tags[17]
                    }
            },
        };
        foreach (var post in posts)
        {
            if (!_dbContext.Posts.Any(a => a.UrlSlug == post.UrlSlug))
                _dbContext.Posts.Add(post);
        }
        //_dbContext.Posts.AddRange(posts);
        _dbContext.SaveChanges();

        return posts;
    }

}