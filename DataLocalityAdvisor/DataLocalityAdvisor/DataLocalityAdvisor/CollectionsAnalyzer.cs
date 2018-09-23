using System.Collections.Immutable;
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
            context.RegisterOperationAction(FindCollections, OperationKind.ObjectOrCollectionInitializer);
            
        }

        private void FindCollections(OperationAnalysisContext context)
        {
            IObjectOrCollectionInitializerOperation creationExpression = (IObjectOrCollectionInitializerOperation)context.Operation;
            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Operation.Syntax.GetLocation()));
        }
    }
}
