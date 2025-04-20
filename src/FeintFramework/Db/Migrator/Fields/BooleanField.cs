using FeintFramework.Forms.Fields;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.Migrator.Fields;

public class BooleanField : BaseField<bool>
{
    public BooleanField()
    {
    }
    public override ExpressionSyntax? GetDefaultValueAssignment()
    {
        if (DefaultValue == null)
            return null;

        return AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName("DefaultValue"),
            LiteralExpression(
               this.DefaultValue ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression
            )
        );
    }
    public override BaseFormField? FormField
    {
        get
        {
            return new BooleanFormField()
            {
                Required = this.NotNull,
            };
        }
    }
}