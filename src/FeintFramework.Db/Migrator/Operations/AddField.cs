using FeintFramework.Db.Migrator.Fields;

namespace FeintFramework.Db.Migrator.Operations;

public class AddField : FieldOperation
{
    public BaseField Field { get; set; }

    public AddField(string modelName, string name) : base(modelName, name)
    {

    }
}