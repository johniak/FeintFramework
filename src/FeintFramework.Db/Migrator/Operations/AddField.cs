using FeintFramework.Db.Migrator.Fields;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace FeintFramework.Db.Migrator.Operations;

public class AddField : FieldOperation
{
    public BaseField Field { get; set; }

    public AddField(string modelName, string name) : base(modelName, name)
    {

    }

    public override ExpressionSyntax GenerateOperation()
    {
        var addFieldCreation = ObjectCreationExpression(ParseTypeName("AddField"))
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
        var fieldExpression = Field.GetExpression();
        var initializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression)
            .AddExpressions(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName("Field"),
                    fieldExpression
                )
            );
        return addFieldCreation.WithInitializer(initializer);
    }
}