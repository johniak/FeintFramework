

using LinqToDB.Mapping;

[Table(Name = "Blogs")]
public class Blog
{
    [PrimaryKey, Identity]
    public int BlogId { get; set; }

    [Column, NotNull]
    public string Url { get; set; }
}