using FeintFramework.Db.Migrator.Fields;
namespace FeintFramework.Db.Migrator.Operations;


public class CreateModel : ModelOperation
{
    public (string Name, BaseField Field)[] Fields { get; set; } = [];
    public CreateModel(string modelName) : base(modelName)
    {
    }
}