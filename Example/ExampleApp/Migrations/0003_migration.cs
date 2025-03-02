using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Operations;

namespace Example.ExampleApp.Migrations;

class _0003_migration : BaseMigration
{
    public override (string ApplicationName, string MigrationName)[] Dependencies => [
        ("ExampleApp", "_0002_migration")
    ];
    public override bool Initial => true;

    public override MigrationOperation[] Operations => [
        new AlterField("Author","FullName"){
            Field = new CharField(){
                NotNull = false,
                Length = 200,
            }
        }
    ];

}