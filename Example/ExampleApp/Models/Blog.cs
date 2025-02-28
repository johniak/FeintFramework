

using FeintFramework.Db;
using LinqToDB.Mapping;


[Table(Name = "blogs")]
public partial class Blog : Model
{
    [Column, NotNull]
    public string Url { get; set; }
}


