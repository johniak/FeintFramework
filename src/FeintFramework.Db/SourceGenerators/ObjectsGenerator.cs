using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FeintFramework.Db.Generator
{
    [Generator]
    public class ObjetsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver to gather candidate classes
            context.RegisterForSyntaxNotifications(() => new ModelSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is ModelSyntaxReceiver receiver))
                return;

            foreach (var classDecl in receiver.CandidateClasses)
            {
                var modelSymbol = context.Compilation.GetSemanticModel(classDecl.SyntaxTree)
                    .GetDeclaredSymbol(classDecl);
                if (modelSymbol == null)
                    continue;

                // Check for our custom attribute or base class as needed
                bool hasAttribute = false;
                foreach (var attribute in modelSymbol.GetAttributes())
                {
                    if (attribute.AttributeClass?.ToDisplayString() == "Table")
                    {
                        hasAttribute = true;
                        break;
                    }
                }

                if (!hasAttribute)
                    continue;

                // Generate the static property code
                var namespaceName = modelSymbol.ContainingNamespace.ToDisplayString();
                var className = modelSymbol.Name;

                var source = $@"
namespace {namespaceName}
{{
    public partial class {className}
    {{
        public static LinqToDB.ITable<{className}> Objects => FeintFramework.Db.Connections.Connection!.GetTable<{className}>();
    }}
}}";
                context.AddSource($"{className}_ObjectsProperty.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    // A simple syntax receiver that finds partial class declarations with attributes
    class ModelSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // We are interested in partial classes that have any attributes
            if (syntaxNode is ClassDeclarationSyntax cds &&
                cds.Modifiers.Any(mod => mod.ValueText == "partial") &&
                cds.AttributeLists.Count > 0)
            {
                CandidateClasses.Add(cds);
            }
        }
    }
}
