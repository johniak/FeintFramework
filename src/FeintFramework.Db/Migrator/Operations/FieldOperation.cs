namespace FeintFramework.Db.Migrator.Operations;

public class FieldOperation : ModelOperation
{
    public string Name { get; set; }

    public FieldOperation(string modelName, string name) : base(modelName)
    {
        Name = name;
    }
}