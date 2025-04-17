using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace Example.Blog.Migrations;
public class _0003_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [("BlogApp", "_0002_migration"), ("AuthApp", "_0001_migration")];
    public override MigrationOperation[] Operations => [new RemoveField("BlogPost", "CreatedBy")];
}