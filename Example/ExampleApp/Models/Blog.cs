

using FeintFramework.Db;
using FeintFramework.Db.Generator;
using LinqToDB.Mapping;


[Table(Name = "Blogs")]
public class Blog : Model
{
    [Column, NotNull]
    public string Url { get; set; }
}