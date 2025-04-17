

using Example.Blog.Models;
using FeintFramework.Contrib.Admin;

namespace Example.Blog;

public class AuthorAdmin : ModelAdmin<Author>
{
    public override string[]? ListDisplay => new[] { nameof(Author.FullName), nameof(Author.Email) };
}

public class BlogPostAdmin : ModelAdmin<BlogPost>
{
}