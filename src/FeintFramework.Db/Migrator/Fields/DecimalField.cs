using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.Migrator.Fields;

public class DecimalField : BaseField<decimal>
{
    public int MaxDigits { get; set; }
    public int DecimalPlaces { get; set; }
    public DecimalField(int maxDigits, int decimalPlaces)
    {
        MaxDigits = maxDigits;
        DecimalPlaces = decimalPlaces;
    }

    public override List<ExpressionSyntax> GetInitializerExpressions()
    {
        var expressions = base.GetInitializerExpressions();
        
        expressions.Add(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("MaxDigits"),
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(MaxDigits)
                )
            )
        );

        expressions.Add(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("DecimalPlaces"),
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(DecimalPlaces)
                )
            )
        );

        return expressions;
    }

    public static bool operator ==(DecimalField? left, DecimalField? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.NotNull == right.NotNull &&
               left.PrimaryKey == right.PrimaryKey &&
               left.Unique == right.Unique &&
               left.DbIndex == right.DbIndex &&
               Equals(left.DefaultValue, right.DefaultValue) &&
                left.MaxDigits == right.MaxDigits &&
                left.DecimalPlaces == right.DecimalPlaces;
    }

    public static bool operator !=(DecimalField? left, DecimalField? right)
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
        return HashCode.Combine(NotNull, PrimaryKey, Unique, DbIndex, DefaultValue, MaxDigits, DecimalPlaces);
    }
}