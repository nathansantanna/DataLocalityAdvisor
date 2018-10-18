using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
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
            context.RegisterOperationAction(Operaction, OperationKind.Loop);
        }

        private void Operaction(OperationAnalysisContext operationAnalysisContext)
        {
            var collections = FindLoopCollections(operationAnalysisContext);

            foreach (var collection in collections)
            {
                //collection.
                if (!CanBeusedAsArray(operationAnalysisContext, collection))
                    return;
                
                operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, collection.Locations[0]));
            }
        }


        private IEnumerable<ILocalSymbol> FindLoopCollections(OperationAnalysisContext operationAnalysisContext)
        {
            var operation = operationAnalysisContext.Operation;
            var accesses = operation.Syntax.DescendantNodesAndSelf().OfType<ElementAccessExpressionSyntax>();
            var tree = operationAnalysisContext.Operation.Syntax.SyntaxTree;
            var model = operationAnalysisContext.Compilation.GetSemanticModel(tree);
            var localSymbols = model.LookupSymbols(operation.Syntax.FullSpan.Start).OfType<ILocalSymbol>();
            foreach (var accessNode in accesses)
            {
                accessNode.DescendantNodesAndSelf().Where(IdentifierNameSyntax);
            }

            //var operation = (ILoopOperation)operationAnalysisContext.Operation;
            //var collections = new List<IdentifierNameSyntax>();
            //List<ILocalSymbol> localsymbols = new List<ILocalSymbol>();
            //var tree = operationAnalysisContext.Operation.Syntax.SyntaxTree;
            //var model = operationAnalysisContext.Compilation.GetSemanticModel(tree);

            //var elementsAcessed = operation.Syntax.DescendantNodesAndSelf().OfType<ElementAccessExpressionSyntax>();
            //var localSymbols = model.LookupSymbols(operation.Syntax.FullSpan.Start).OfType<ILocalSymbol>();
            //localsymbols.AddRange(localSymbols.
            //    Where(symbol => symbol.Type.ToString().Contains("System.Collections")));
            //foreach (var accessExpression in elementsAcessed)
            //{
            //    collections.AddRange(accessExpression.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>());
            //}

            //return localsymbols;
        }

        

        private bool CanBeusedAsArray(OperationAnalysisContext operationAnalysisContext, ILocalSymbol collection)
        {
            var methodBlock = operationAnalysisContext.ContainingSymbol.DeclaringSyntaxReferences[0].GetSyntax().ChildNodes().
                OfType<BlockSyntax>().First();

            var expresionStatements = methodBlock.ChildNodes().OfType<ExpressionStatementSyntax>();
            foreach (var expresionStatement in expresionStatements)
            {
                //if (expresionStatement.Expression.ChildTokens()
                //    .OfType<IdentifierNameSyntax>();//.Any(syntax => syntax.Identifier == collection.Identifier))
                    return false;
            }

            return true;
        }


    }
}

