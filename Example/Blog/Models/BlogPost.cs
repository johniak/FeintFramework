using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace Example.Blog.Models;

[Table(Name = "blog_app_blog_post")]
public partial class BlogPost : IntModel
{
    [Column, NotNull, CharField(Length = 255)]
    public string Title { get; set; }

    [Column, TextField()]
    public string Content { get; set; }

    [Association(ThisKey = nameof(Author), OtherKey = nameof(Author.Id)), ForeignKey("BlogApp.Author", ForeignKeyAction.Cascade)]
    public Author Author { get; set; }
    
}


