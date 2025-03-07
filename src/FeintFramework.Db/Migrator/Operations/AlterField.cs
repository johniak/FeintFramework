using FeintFramework.Db.Migrator.Fields;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace FeintFramework.Db.Migrator.Operations;

public class AlterField : FieldOperation
{
    public BaseField Field { get; set; }
    public AlterField(string modelName, string name) : base(modelName, name)
    {

    }

    public override ExpressionSyntax GenerateOperation()
    {
        var alterFieldCreation = ObjectCreationExpression(ParseTypeName(this.GetType().Name))
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

        // Generate the Field initialization expression using GetExpression() from BaseField
        var fieldExpression = Field.GetExpression();

        // Add Field = fieldExpression to the initializer block
        var initializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression)
            .AddExpressions(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName("Field"),
                    fieldExpression
                )
            );

        // Attach the initializer to the AlterField object creation
        return alterFieldCreation.WithInitializer(initializer);
    }
}