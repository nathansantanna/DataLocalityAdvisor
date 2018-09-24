using System;
using System.Collections;
using System.Collections.Generic;
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
            context.RegisterSymbolAction(SymbolAction, SymbolKind.Property,SymbolKind.NamedType,SymbolKind.Method);
        }

        private void SymbolAction(SymbolAnalysisContext symbolContext)
        {
            var symbol = symbolContext.Symbol;
            var compilation = symbolContext.Compilation;
            switch (symbolContext.Symbol.Kind)
            {
                case SymbolKind.Property:
                    var list = new List<string>();
                        if(symbolContext.Symbol.Kind == SymbolKind.ArrayType)
                            symbolContext.ReportDiagnostic(Diagnostic.Create(Rule,symbolContext.Symbol.Locations[0]));
                    var propSymbol = (IPropertySymbol) symbol;
                    var we = propSymbol.IsWriteOnly;
                    break;
               case SymbolKind.NamedType:
                   var typeSymbol = (ITypeSymbol) symbolContext.Symbol;
                   foreach (var child  in typeSymbol.GetMembers())
                   {
                       if(symbolContext.Symbol.Kind == SymbolKind.ArrayType)
                           symbolContext.ReportDiagnostic(Diagnostic.Create(Rule,symbolContext.Symbol.Locations[0]));
                   }
                   break;
               case SymbolKind.Method:
                   var methodSymbol = (IMethodSymbol) symbolContext.Symbol;
                   foreach (var child  in methodSymbol.GetAttributes())
                   {
                       if(symbolContext.Symbol.Kind == SymbolKind.ArrayType)
                           symbolContext.ReportDiagnostic(Diagnostic.Create(Rule,symbolContext.Symbol.Locations[0]));
                   }
                   break;
            }
        }

        public ISymbol[] GetLocalVariables(IMethodSymbol symbol)
        {
            throw new NotImplementedException();
        }

        private void CompilationAction(CompilationAnalysisContext compilation)
        {
            var symbols = compilation.Compilation.GetSymbolsWithName(s => true);
            var arrays = symbols.OfType<IArrayTypeSymbol>().ToList();
        }
    }
}
