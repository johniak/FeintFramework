using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace Example.Account.Migrations;
public class _0001_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [];
    public override MigrationOperation[] Operations => [new CreateModel("User")
    {
        Fields = [("Username", new CharField() { NotNull = false, PrimaryKey = false, Unique = false, DbIndex = false, Length = 255 }), ("Password", new CharField() { NotNull = false, PrimaryKey = false, Unique = false, DbIndex = false, Length = 255 }), ("Id", new AutoField() { NotNull = true, PrimaryKey = true, Unique = false, DbIndex = false, AutoIncrement = true })]
    }

    ];
}