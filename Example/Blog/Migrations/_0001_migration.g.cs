using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace Example.Blog.Migrations;
public class _0001_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [];
    public override MigrationOperation[] Operations => [new CreateModel("Author")
    {
        Fields = [("FullName", new CharField() { NotNull = false, PrimaryKey = false, Unique = false, DbIndex = false, Length = 200 }), ("Email", new CharField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, Length = 255 }), ("CreatedAt", new DateTimeField() { NotNull = true, PrimaryKey = false, Unique = false, DbIndex = false, AutoNowAdd = false }), ("Id", new AutoField() { NotNull = true, PrimaryKey = true, Unique = false, DbIndex = false, AutoIncrement = true })]
    }, new CreateModel("BlogPost")
    {
        Fields = [("Title", new CharField() { NotNull = false, PrimaryKey = false, Unique = false, DbIndex = false, Length = 255 }), ("Content", new TextField() { NotNull = false, PrimaryKey = false, Unique = false, DbIndex = false }), ("Author", new ForeignKey("BlogApp.Author", ForeignKeyAction.Cascade) { NotNull = false, PrimaryKey = false, Unique = false, DbIndex = false, DefaultValue = 0 }), ("Id", new AutoField() { NotNull = true, PrimaryKey = true, Unique = false, DbIndex = false, AutoIncrement = true })]
    }

    ];
}