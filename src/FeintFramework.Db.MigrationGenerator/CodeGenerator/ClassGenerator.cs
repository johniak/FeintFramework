using System;
using System.Collections.Generic;
using System.Linq;
using FeintFramework.Core.Apps;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace FeintFramework.Db.MigrationGenerator;
public class ClassGenerator
{
    public ClassDeclarationSyntax Class { get; protected set; }
    public List<UsingDirectiveSyntax> Using { get; protected set; }
    public FileScopedNamespaceDeclarationSyntax Namespace { get; protected set; }

    public InitializerExpressionSyntax OperationsInitializer { get; private set; }
        = InitializerExpression(SyntaxKind.ArrayInitializerExpression);

    public InitializerExpressionSyntax DependenciesInitializer { get; private set; }
        = InitializerExpression(SyntaxKind.ArrayInitializerExpression);

    protected BaseApplication app;
    protected string className;

    public ClassGenerator(BaseApplication app, string className)
    {
        this.app = app;
        this.className = className;
    }
    protected void InitClass()
    {
        Using = new[]
        {
            UsingDirective(ParseName("FeintFramework.Db.Migrator.Fields")),
            UsingDirective(ParseName("FeintFramework.Db.Migrator")),
            UsingDirective(ParseName("FeintFramework.Db.Migrator.Operations"))
        }.ToList();

        var applicationNameSpace = app.GetType().Namespace;
        Namespace = FileScopedNamespaceDeclaration(ParseName($"{applicationNameSpace}.Migrations"));

       

        Class = ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseMigration")));
    }

    protected void AddProperties(){
        var dependenciesProperty = PropertyDeclaration(
            ParseTypeName("(string ApplicationName, string MigrationName)[]"),
            "Dependencies")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .WithExpressionBody(ArrowExpressionClause(
                ImplicitArrayCreationExpression(DependenciesInitializer)
            ))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var operationsProperty = PropertyDeclaration(
            ParseTypeName("MigrationOperation[]"),
            "Operations")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .WithExpressionBody(ArrowExpressionClause(
                ImplicitArrayCreationExpression(OperationsInitializer)
            ))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        Class = Class.AddMembers(dependenciesProperty, operationsProperty);
    }

    public void AddOperation(ExpressionSyntax operationExpression)
    {
        OperationsInitializer = OperationsInitializer.AddExpressions(operationExpression);
    }

    public void AddDependency(ExpressionSyntax dependencyExpression)
    {
        DependenciesInitializer = DependenciesInitializer.AddExpressions(dependencyExpression);
    }

    public override string ToString()
    {
        InitClass();
        AddProperties();
        var compilationUnit = CompilationUnit()
            .AddUsings(Using.ToArray())
            .AddMembers(Namespace)
            .AddMembers(Class)
            .NormalizeWhitespace();
        return compilationUnit.ToFullString();
    }
}
