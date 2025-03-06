using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.Migrator.Fields;

public partial class CharField : BaseField<string>
{
    public int Length { get; set; }
    public CharField()
    {
    }
    public override List<ExpressionSyntax> GetInitializerExpressions()
    {
        var expressions = base.GetInitializerExpressions();
        expressions.Add(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("Length"),
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(Length)
                )
            )
        );
        return expressions;
    }

    public static bool operator ==(CharField? left, CharField? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.NotNull == right.NotNull &&
               left.PrimaryKey == right.PrimaryKey &&
               left.Unique == right.Unique &&
               left.DbIndex == right.DbIndex &&
               Equals(left.DefaultValue, right.DefaultValue)&&
                left.Length == right.Length;
    }

     public static bool operator !=(CharField? left, CharField? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseField other)
            return false;

        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NotNull, PrimaryKey, Unique, DbIndex, DefaultValue, Length);
    }
}