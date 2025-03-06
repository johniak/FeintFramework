using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.Migrator.Fields;

public class BaseField : Attribute
{
    public bool NotNull { get; set; }
    public bool PrimaryKey { get; set; }
    public bool Unique { get; set; }
    public bool DbIndex { get; set; }

    public virtual object? DefaultValue { get; }

    public BaseField()
    {
    }

    public virtual ExpressionSyntax? GetDefaultValueAssignment()
{
    if (DefaultValue == null)
        return null;

    return AssignmentExpression(
        SyntaxKind.SimpleAssignmentExpression,
        IdentifierName("DefaultValue"),
        LiteralExpression(
            SyntaxKind.StringLiteralExpression,
            Literal(DefaultValue.ToString()!)
        )
    );
}

    public virtual List<ExpressionSyntax> GetInitializerExpressions()
    {
        var expressions = new List<ExpressionSyntax>
        {
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("NotNull"),
                LiteralExpression(NotNull ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
            ),
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("PrimaryKey"),
                LiteralExpression(PrimaryKey ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
            ),
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("Unique"),
                LiteralExpression(Unique ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
            ),
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName("DbIndex"),
                LiteralExpression(DbIndex ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)
            ),
        };
        var defaultAssigment = 
            GetDefaultValueAssignment();
        if (defaultAssigment != null)
        {
            expressions.Add(defaultAssigment);
        }
        return expressions;
    }

    public virtual ExpressionSyntax GetExpression()
    {
        var fieldCreation = ObjectCreationExpression(ParseTypeName(this.GetType().Name))
            .WithArgumentList(ArgumentList())
            .WithInitializer(
                InitializerExpression(SyntaxKind.ObjectInitializerExpression)
                    .AddExpressions(GetInitializerExpressions().ToArray())
            );

        return fieldCreation;
    }

    public static bool operator ==(BaseField? left, BaseField? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.NotNull == right.NotNull &&
               left.PrimaryKey == right.PrimaryKey &&
               left.Unique == right.Unique &&
               left.DbIndex == right.DbIndex &&
               Equals(left.DefaultValue, right.DefaultValue);
    }
    
    public static bool operator !=(BaseField? left, BaseField? right)
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
        return HashCode.Combine(NotNull, PrimaryKey, Unique, DbIndex, DefaultValue);
    }
}

[AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = true, Inherited = true)]
public class BaseField<T> : BaseField
{
    public T? DefaultValue { get; set; }

    public BaseField()
    {

    }

}

