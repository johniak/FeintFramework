using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FeintFramework.SourceGenerators
{
    [Generator]
    public class ObjetsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Create a syntax provider to filter for partial classes with attributes.
            var candidateClasses = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (s, _) =>
                {
                    // We are interested in partial class declarations that have attributes.
                    return s is ClassDeclarationSyntax cds &&
                           cds.Modifiers.Any(m => m.ValueText == "partial") &&
                           cds.AttributeLists.Count > 0;
                },
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            );

            // Combine the candidates with the compilation.
            var compilationAndClasses = context.CompilationProvider.Combine(candidateClasses.Collect());

            // Register a source output that generates code for each candidate class.
            context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
            {
                var (compilation, classes) = source;
                foreach (var classDecl in classes)
                {
                    var semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
                    var modelSymbol = semanticModel.GetDeclaredSymbol(classDecl);
                    if (modelSymbol == null)
                        continue;

                    
                    // Check if the class has an attribute named "Table".
                    bool hasAttribute = modelSymbol.GetAttributes()
                        .Any(a => a.AttributeClass?.ToDisplayString() == "LinqToDB.Mapping.TableAttribute");
                    var attrs = modelSymbol.GetAttributes();
                    if (!hasAttribute)
                        continue;

                    // Get namespace and class name.
                    var namespaceName = modelSymbol.ContainingNamespace.ToDisplayString();
                    var namespaceLine = "";
                    if (namespaceName != "<global namespace>")
                        namespaceLine = $"namespace {namespaceName};";
                    var className = modelSymbol.Name;
                            var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                id: "FG001",
                title: "Generator Info",
                messageFormat: $"{namespaceLine}",
                category: "SourceGeneration",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            Location.None);
        spc.ReportDiagnostic(diagnostic);
                    // Generate the static property code.
                    var sourceText = $@"
using LinqToDB;
using LinqToDB.Data;

{namespaceLine}
public partial class {className}
{{
    public static LinqToDB.ITable<{className}> Objects => FeintFramework.Db.Connections.Connection!.GetTable<{className}>();
}}
";
                    spc.AddSource($"{className}_ObjectsProperty.g.cs", SourceText.From(sourceText, Encoding.UTF8));
                }
            });
        }
    }
}
