using FeintFramework.Db.Migrator.Fields;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace FeintFramework.Db.Migrator.Operations;

public class RemoveField : FieldOperation
{
    public RemoveField(string modelName, string name) : base(modelName, name)
    {

    }

    public override ExpressionSyntax GenerateOperation()
    {
        var addFieldCreation = ObjectCreationExpression(ParseTypeName("RemoveField"))
            .WithArgumentList(
                ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(ModelName))),
                            Token(SyntaxKind.CommaToken),
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(Name)))
                        }
                    )
                )
            );
        return addFieldCreation;
    }

}