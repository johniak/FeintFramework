using FeintFramework.Db.Migrator.Fields;

namespace FeintFramework.Db.Migrator.Operations;

public class RemoveField : FieldOperation
{
    public RemoveField(string modelName, string name) : base(modelName, name)
    {

    }
}