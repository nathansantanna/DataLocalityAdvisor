
using System.Collections.Immutable;
using DataLocalityAnalyzer;
using DataLocalityAnalyzer.SupportClasses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DataLocalityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConvertClassToStructAnalyzer : DiagnosticAnalyzer
    {
        #region Localizable Strings
        public const string DiagnosticId = "DataLocalityAdvisor";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Data Locality";
        #endregion 
        //public to help to  unit test the diagnosis
        public static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSymbolAction(ClassAnalyzer, SymbolKind.NamedType);
        }

        private void ClassAnalyzer(SymbolAnalysisContext symbolAnalysisContext)
        {
            var namedSymbol = (INamedTypeSymbol)symbolAnalysisContext.Symbol;
            if(ClassUtilities.CanBeStruct(namedSymbol))
                symbolAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, symbolAnalysisContext.Symbol.Locations[0]));
        }

    }
}