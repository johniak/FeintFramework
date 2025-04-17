using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeintFramework.Db.Migrator.Operations;

public abstract class FieldOperation : ModelOperation
{
    public string Name { get; set; }

    public FieldOperation(string modelName, string name) : base(modelName)
    {
        Name = name;
    }

}