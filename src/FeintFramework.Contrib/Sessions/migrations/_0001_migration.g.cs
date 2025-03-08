using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace FeintFramework.Contrib.Sessions.Migrations;
public class _0001_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [];
    public override MigrationOperation[] Operations => [new CreateModel("Session")
    {
        Fields = [("SessionKey", new CharField() { NotNull = true, PrimaryKey = false, Unique = true, DbIndex = false, Length = 40 }), ("SessionData", new TextField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false }), ("ExpireDate", new DateTimeField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, AutoNowAdd = false }), ("CreatedAt", new DateTimeField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, AutoNowAdd = true }), ("Id", new AutoField() { NotNull = true, PrimaryKey = true, Unique = false, DbIndex = false, AutoIncrement = true })]
    }

    ];
}