

using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace Example.Account.Models;

[Table(Name = "example_app_user")]
public partial class User : Model<User>
{
    [Column, NotNull, CharField(Length = 255)]
    public string Username { get; set; }

    [Column, NotNull, CharField(Length = 255)]
    public string Password { get; set; }
}

