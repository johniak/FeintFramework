

using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;


[Table(Name = "example_app_blog_post")]
public partial class BlogPost : Model
{
    [Column, NotNull, CharField(Length = 255)]
    public string Title { get; set; }

    [Column, TextField()]
    public string Content { get; set; }

    [Association(ThisKey = nameof(Author), OtherKey=nameof(Author.Id)), ForeignKey("ExampleApp.Author", ForeignKeyAction.Cascade)]
    public Author Author { get; set; }
}


