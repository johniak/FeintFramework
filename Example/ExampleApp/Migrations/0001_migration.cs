using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;

namespace Example.ExampleApp.Migrations;

class _0001_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [];
    public override bool Initial => true;

    public override MigrationOperation[] Operations => [
        new CreateModel("Author"){
            Fields = [
                ("id", new AutoField(){
                    PrimaryKey = true,
                    NotNull = true,
                }),
                ("FullName",new CharField(){
                    NotNull = true,
                    Length = 255,
                }),
            ]
        }
    ];

}