using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeintFramework.Db.Migrator.Operations;
public abstract class ModelOperation : MigrationOperation
{

    public string ModelName { get; set; }

    public ModelOperation(string modelName)
    {
        ModelName = modelName;
    }

    public abstract ExpressionSyntax GenerateOperation();
}