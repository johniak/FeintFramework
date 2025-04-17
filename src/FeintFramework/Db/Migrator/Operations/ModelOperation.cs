using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace FeintFramework.Db.Migrator.Operations;
public abstract class ModelOperation : MigrationOperation
{

    public string ModelName { get; set; }

    public ModelOperation(string modelName)
    {
        ModelName = modelName;
    }

    public abstract ExpressionSyntax GenerateOperation();
    protected ExpressionSyntax BuildTypedArrayExpression(IEnumerable<ExpressionSyntax> expressions)
    {
        if (!expressions.Any())
        {
            return ParseExpression("[]");
        }
        else
        {
            // Normalize each expression and join them with a comma and a space
            var joined = string.Join(", ", expressions.Select(e => e.NormalizeWhitespace().ToFullString()));
            var code = $"[{joined}]";
            return ParseExpression(code).NormalizeWhitespace();
        }
    }
}