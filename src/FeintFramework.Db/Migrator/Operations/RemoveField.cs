using FeintFramework.Db.Migrator.Fields;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeintFramework.Db.Migrator.Operations;

public class RemoveField : FieldOperation
{
    public RemoveField(string modelName, string name) : base(modelName, name)
    {

    }

    public override ExpressionSyntax GenerateOperation()
    {
        throw new NotImplementedException();
    }

}