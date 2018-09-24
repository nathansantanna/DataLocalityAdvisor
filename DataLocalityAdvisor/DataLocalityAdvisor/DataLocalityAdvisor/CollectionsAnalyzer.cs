using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DataLocalityAdvisor
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CollectionsAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DataLocalityAdvisor";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Data Locality";
        //public to help to test the diagnosis
        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSemanticModelAction(Action);
            context.RegisterOperationBlockAction(OperAction);
        }

        private void OperAction(OperationBlockAnalysisContext oper)
        {
            oper.ReportDiagnostic(Diagnostic.Create(Rule,oper.OwningSymbol.Locations[0]));
            if (oper.OwningSymbol.CanBeReferencedByName && oper.OwningSymbol.Name == "ConsoleApp1")
            {
                var teste = ((INamedTypeSymbol) oper.OwningSymbol).GetMembers();
            }
      }

        private void Action(SemanticModelAnalysisContext semanticModelAnalysis)
        {
            foreach (INamespaceOrTypeSymbol nameSpace in semanticModelAnalysis.SemanticModel.LookupNamespacesAndTypes(0))
            {
                var symbols = nameSpace.GetMembers();
            }
            foreach (INamespaceOrTypeSymbol nameSpace in semanticModelAnalysis.SemanticModel.LookupNamespacesAndTypes(1))
            {
                var symbols = nameSpace.GetMembers();
            }

        }
    }
}
