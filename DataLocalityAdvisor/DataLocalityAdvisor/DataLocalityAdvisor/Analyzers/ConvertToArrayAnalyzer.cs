using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using DataLocalityAnalyzer.SupportClasses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DataLocalityAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConvertToArrayAnalyzer : DiagnosticAnalyzer
    {
        #region DiagnosticDescriptor
        public const string DiagnosticId = "ConversionToArray";
        internal static readonly LocalizableString Title = "Collection Conversion";
        internal static readonly LocalizableString MessageFormat = "You can Convert '{0}' to array and improve Data Locality";
        internal const string Category = "Data Locality";
        #endregion
        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterOperationAction(Operaction,OperationKind.Loop);
        }

        private void Operaction(OperationAnalysisContext operationAnalysisContext)
        {
            var opert = (ILoopOperation)operationAnalysisContext.Operation;
            switch (opert.LoopKind)
            {
                case LoopKind.None:
                    break;
                case LoopKind.While:
                    break;
                case LoopKind.For:
                    break;
                case LoopKind.ForTo:
                    break;
                case LoopKind.ForEach:
                    var loop = (IForEachLoopOperation) opert;
                    if (loop.Collection.Children.ToList()[0].Kind == OperationKind.LocalReference)
                    {
                        var col  = (ILocalReferenceOperation) loop.Collection.Children.ToList()[0];
                        operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, col.Local.Locations[0]));
                    }
                    break;
            }
           
        }
    }
}
