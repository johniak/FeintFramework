using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.Migrator.Fields;

public enum ForeignKeyAction
{
    Cascade,
    SetNull,
    SetDefault,
    NoAction,
    Restrict
}

public class ForeignKey : BaseField<int>
{
    public string To { get; set; }

    public ForeignKeyAction OnDelete { get; set; }

    public ForeignKey(string to, ForeignKeyAction onDelete)
    {
        To = to;
        OnDelete = onDelete;
    }

public override ExpressionSyntax GetExpression()
{
    return ObjectCreationExpression(ParseTypeName(this.GetType().Name))
        .WithArgumentList(
            ArgumentList(
                SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                        Argument(
                            LiteralExpression(
                                SyntaxKind.StringLiteralExpression,
                                Literal(To)
                            )
                        ),
                        Token(SyntaxKind.CommaToken),
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                ParseTypeName("ForeignKeyAction"),
                                IdentifierName(OnDelete.ToString())
                            )
                        )
                    }
                )
            )
        ).WithInitializer(
                InitializerExpression(SyntaxKind.ObjectInitializerExpression)
                    .AddExpressions(GetInitializerExpressions().ToArray())
            );;
}




    public override ExpressionSyntax? GetDefaultValueAssignment()
    {
        if (DefaultValue == null)
            return null;

        return AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName("DefaultValue"),
            LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal((int)DefaultValue)
            )
        );
    }

    public static bool operator ==(ForeignKey? left, ForeignKey? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;

        return left.NotNull == right.NotNull &&
               left.PrimaryKey == right.PrimaryKey &&
               left.Unique == right.Unique &&
               left.DbIndex == right.DbIndex &&
               Equals(left.DefaultValue, right.DefaultValue) &&
               left.To == right.To &&
               left.OnDelete == right.OnDelete;
    }

    public static bool operator !=(ForeignKey? left, ForeignKey? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ForeignKey other)
            return false;

        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NotNull, PrimaryKey, Unique, DbIndex, DefaultValue, To, OnDelete);
    }
}