using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator;

class _0001_migration : BaseMigration
{
    public override MigrationDependency[] Dependencies => [];

    public override MigrationOperation[] Operations => [
        new CreateModel("blogs"){
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