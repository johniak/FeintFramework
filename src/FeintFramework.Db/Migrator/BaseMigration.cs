

public abstract class BaseMigration
{
    public abstract MigrationDependency[] Dependencies { get; }

    public abstract MigrationOperation[] Operations { get; }

    public virtual bool Initial { get; } = false;
    public virtual bool Atomic { get; } = false;

}