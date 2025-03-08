



using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace FeintFramework.Contrib.Sessions.Models;

[Table(Name = "sessions_app_session")]
public partial class Session : Model<Session>
{
    public Session()
    {
        SessionKey = Guid.NewGuid().ToString();
    }

    [Column(Name = "session_key"), CharField(Length = 40, NotNull = true, Unique = true)]
    public string SessionKey { get; set; }

    [Column(Name = "session_data"), TextField(NotNull = true, DefaultValue = "{}")]
    public string SessionData { get; set; } = "{}";

    [Column("expire_date"), DateTimeField(NotNull = true)]
    public DateTime ExpireDate { get; set; }

    [Column("created_at"), DateTimeField(NotNull = true, AutoNowAdd = true)]
    public DateTime CreatedAt { get; set; }
}


