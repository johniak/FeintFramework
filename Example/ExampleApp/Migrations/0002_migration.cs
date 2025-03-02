using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

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
                ("id",new AutoField(){
                    PrimaryKey = true,
                    NotNull = true,
                }),
                ("title",new CharField(){
                    NotNull = true,
                    Length = 255,
                }),
                ("content",new TextField(){
                    NotNull = false,
                }),
                ("author_id",new ForeignKey("ExampleApp.Author",ForeignKeyAction.Cascade){
                    NotNull = true,
                }),
            ]
        },
        new AddField("Author","Email"){
            Field = new CharField(){
                NotNull = false,
                Length = 255,
            }
        },
        new RemoveField("Author","Name"),
        new RunSql(@"INSERT INTO example_app_author (full_name) VALUES ('John Doe');",""),
    ];

}