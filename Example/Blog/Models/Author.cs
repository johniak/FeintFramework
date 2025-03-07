

using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace Example.Blog.Models;

[Table(Name = "example_app_author")]
public partial class Author : Model
{
    [Column(Name = "full_name"), CharField(Length = 200, NotNull = false)]
    public string FullName { get; set; }

    [Column("email"), CharField(Length = 255, NotNull = true)]
    public string Email { get; set; }

    [Column("created_at"), DateTimeField(NotNull = true, DefaultValue = "2021-01-01")]
    public DateTime CreatedAt { get; set; }
}


