

using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;


[Table(Name = "example_app_author")]
public partial class Author : Model
{
    [Column(Name ="full_name"), CharField(Length = 100, NotNull = true)]
    public string FullName { get; set; }

    [Column("email"), CharField(Length = 255, NotNull = true)]
    public string Email { get; set; }
}


