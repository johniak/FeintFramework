

public class MigrationDependency
{
    public Type Application { get; set; }
    public string MigrationName { get; set; }
    public MigrationDependency(Type application, string migrationName)
    {
        Application = application;
        MigrationName = migrationName;
    }
}