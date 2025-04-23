using FeintFramework.Forms.Fields;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.Migrator.Fields;

public class DateTimeField : BaseField<DateTime>
{
    public bool AutoNowAdd { get; set; }
    public String? DefaultValue { get; set; }
    public DateTimeField()
    {
    }
    public override List<ExpressionSyntax> GetInitializerExpressions()
    {
        var expressions = base.GetInitializerExpressions();
        expressions.Add(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("AutoNowAdd"),
                LiteralExpression(
                    this.AutoNowAdd ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression
                )
            )
        );
        return expressions;
    }

    public static bool operator ==(DateTimeField? left, DateTimeField? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.NotNull == right.NotNull &&
               left.PrimaryKey == right.PrimaryKey &&
               left.Unique == right.Unique &&
               left.DbIndex == right.DbIndex &&
               Equals(left.DefaultValue, right.DefaultValue) &&
                left.AutoNowAdd == right.AutoNowAdd;
    }

    public static bool operator !=(DateTimeField? left, DateTimeField? right)
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
        return HashCode.Combine(NotNull, PrimaryKey, Unique, DbIndex, DefaultValue, AutoNowAdd);
    }
    public override BaseFormField? FormField
    {
        get
        {
            return new DateTimeFormField()
            {
                Required = this.NotNull,
            };
        }
    }
}