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
        new CreateModel("blogs2"){
            Fields = [
                new AutoField("id"){
                    PrimaryKey = true,
                    NotNull = true,
                },
                new CharField("url"){
                    NotNull = true,
                    Length = 255,
                }
            ]
        }
    ];

}