using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DataLocalityAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;

namespace DataLocalityAdvisor
{
    
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RoslynCommuncationAgent: DiagnosticAnalyzer
    {
        #region Localizable Strings
        public const string DiagnosticId = "DataLocalityAdvisor";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Data Locality";
        #endregion 
        //public to help to  unit test the diagnosis
        public static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction( compilationContext  =>
            {
                
                INamedTypeSymbol collectionsNameSpace = compilationContext.Compilation.GetTypeByMetadataName("System.Collections") ;
                if (collectionsNameSpace == null)
                    return;
                CompilationAnalyzer analyzer = new CompilationAnalyzer();
                compilationContext.RegisterCompilationEndAction(analyzer.EndCompilationAction);
            });
        }

    }
}

