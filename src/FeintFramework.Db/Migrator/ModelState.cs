
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator.Operations;
namespace FeintFramework.Db.Migrator;
public class ModelState
{
    public Dictionary<string, BaseField> Fields { get; set; } = new Dictionary<string, BaseField>();

    
    public void applyOperation(MigrationOperation operation)
    {
        if (operation is CreateModel createModel)
        {
            Fields = createModel.Fields.ToDictionary(f => f.Name, f => f.Field);
        }
        else if (operation is AddField addField)
        {
            Fields[addField.Name] = addField.Field;
        }
        else if (operation is RemoveField removeField)
        {
            Fields.Remove(removeField.Name);
        }
        else if (operation is AlterField alterField)
        {
            Fields[alterField.Name] = alterField.Field;
        }
    }
}