namespace FeintFramework.Db.Migrator.Operations;
public class ModelOperation : MigrationOperation
{

    public string ModelName { get; set; }

    public ModelOperation(string modelName)
    {
        ModelName = modelName;
    }
}