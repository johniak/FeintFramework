using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace Example.Blog.Migrations;
public class _0002_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [("BlogApp", "_0001_migration"), ("AccountApp", "_0001_migration")];
    public override MigrationOperation[] Operations => [new AddField("BlogPost", "CreatedBy")
    {
        Field = new ForeignKey("AccountApp.User", ForeignKeyAction.Cascade)
        {
            NotNull = false,
            PrimaryKey = false,
            Unique = false,
            DbIndex = false,
            DefaultValue = 0
        }
    }

    ];
}