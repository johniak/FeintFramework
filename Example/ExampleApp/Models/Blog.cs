

using FeintFramework.Db;
using LinqToDB.Mapping;


[Table(Name = "Blogs")]
public partial class Blog : Model
{
    [Column, NotNull]
    public string Url { get; set; }
}


