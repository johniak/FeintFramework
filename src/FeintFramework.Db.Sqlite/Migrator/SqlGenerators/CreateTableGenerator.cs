using FeintFramework.Db.Migrator;

namespace FeintFramework.Db.Sqlite.Migrator;

public class CreateModelGenerator : ISqlGenerator<CreateModel>
{
    public string GenerateSql(CreateModel operation)
    {
        
        return $"CREATE TABLE";
    }
}