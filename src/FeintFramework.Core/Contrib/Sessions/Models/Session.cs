



using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace FeintFramework.Core.Contrib.Sessions.Models;

[Table(Name = "session_app_session")]
public partial class Session : Model
{
    [Column(Name = "session_key"), CharField(Length = 40, NotNull = true, Unique = true, DbIndex = true)]
    public string SessionKey { get; set; }

    [Column(Name = "session_data"), TextField(NotNull = true, Unique = true, DefaultValue = "{}")]
    public string SessionData { get; set; }

    [Column("expire_date"), DateTimeField(NotNull = true)]
    public DateTime ExpireDate { get; set; }

    [Column("created_at"), DateTimeField(NotNull = true, AutoNowAdd = true)]
    public DateTime CreatedAt { get; set; }
}


