using FeintFramework.Db.Migrator.Fields;
namespace FeintFramework.Db.Migrator;
public class ModelOperation : MigrationOperation
{

    public string Name { get; set; }

    public ModelOperation(string name)
    {
        Name = name;
    }
}

public class CreateModel : ModelOperation
{
    public BaseField[] Fields { get; set; } = Array.Empty<BaseField>();
    public CreateModel(string name) : base(name)
    {
    }
}