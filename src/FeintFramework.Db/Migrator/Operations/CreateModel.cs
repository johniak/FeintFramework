using FeintFramework.Db.Migrator.Fields;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace FeintFramework.Db.Migrator.Operations;


public class CreateModel : ModelOperation
{
    public (string Name, BaseField Field)[] Fields { get; set; } = [];
    public CreateModel(string modelName) : base(modelName)
    {
    }

    public override ExpressionSyntax GenerateOperation()
    {
        var createModelExpression = ObjectCreationExpression(ParseTypeName(this.GetType().Name))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(ModelName))
                        )
                    )
                )
            );

        var fieldExpressions = Fields.Select(field =>
                TupleExpression(
                    SeparatedList<ArgumentSyntax>(new[]
                    {
                        Argument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(field.Name))
                        ),
                        Argument(field.Field.GetExpression())
                    })
                )
            )
            .ToArray();
        var initializer = InitializerExpression(SyntaxKind.ObjectInitializerExpression)
            .AddExpressions(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName("Fields"),
                    ImplicitArrayCreationExpression(
                        InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                            .AddExpressions(fieldExpressions)
                    )
                )
            );
        return createModelExpression.WithInitializer(initializer);
    }
}