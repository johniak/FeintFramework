
namespace FeintFramework.Db.Migrator;
public abstract class BaseMigration
{
    public abstract (string ApplicationName, string MigrationName)[] Dependencies { get; }

    public abstract MigrationOperation[] Operations { get; }

    public virtual bool Initial { get; } = false;
    public virtual bool Atomic { get; } = false;

    public virtual string Name
    {
        get
        {
            return this.GetType().Name;
        }
    }
}