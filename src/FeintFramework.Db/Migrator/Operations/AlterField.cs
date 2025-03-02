using FeintFramework.Db.Migrator.Fields;

namespace FeintFramework.Db.Migrator.Operations;

public class AlterField : FieldOperation
{
    public BaseField Field { get; set; }

    public AlterField(string modelName, string name) : base(modelName, name)
    {

    }
}