using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace FeintFramework.Contrib.Auth.Migrations;
public class _0001_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [];
    public override MigrationOperation[] Operations => [new CreateModel("User")
    {
        Fields = [("Username", new CharField() { NotNull = true, PrimaryKey = false, Unique = true, DbIndex = false, Length = 255 }), ("Password", new CharField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, Length = 255 }), ("Email", new CharField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, Length = 255 }), ("IsActive", new BooleanField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, DefaultValue = false }), ("IsStaff", new BooleanField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, DefaultValue = false }), ("IsSuperuser", new BooleanField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, DefaultValue = false }), ("DateJoined", new DateTimeField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, AutoNowAdd = true }), ("Id", new AutoField() { NotNull = true, PrimaryKey = true, Unique = false, DbIndex = false, AutoIncrement = true })]
    }

    ];
}