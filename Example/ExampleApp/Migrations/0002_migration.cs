using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;

namespace Example.ExampleApp.Migrations;

class _0002_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [
        ("ExampleApp", "_0001_migration")
    ];
    public override bool Initial => true;

    public override MigrationOperation[] Operations => [
        new CreateModel("BlogPost"){
            Fields = [
                new AutoField("id"){
                    PrimaryKey = true,
                    NotNull = true,
                },
                new CharField("title"){
                    NotNull = true,
                    Length = 255,
                },
                new TextField("content"){
                    NotNull = false,
                },
                new ForeignKey("author_id","ExampleApp.Author",ForeignKeyAction.Cascade){
                    NotNull = true,
                },
            ]
        }
    ];

}