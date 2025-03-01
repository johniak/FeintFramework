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
                new AutoField("id"){
                    PrimaryKey = true,
                    NotNull = true,
                },
                new CharField("FullName"){
                    NotNull = true,
                    Length = 255,
                },
            ]
        }
    ];

}