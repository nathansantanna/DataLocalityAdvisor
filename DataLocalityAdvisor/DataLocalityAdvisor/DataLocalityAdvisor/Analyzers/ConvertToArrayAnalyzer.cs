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
            var collection = FindLoopCollection(operationAnalysisContext);
            
            if (!collection.Type.ToString().Contains("System.Collections") || 
                !CanBeusedAsArray(operationAnalysisContext,(IdentifierNameSyntax)collection.Syntax))
                return;

            var location = FindCollectionDeclarationLocation((IdentifierNameSyntax)collection.Syntax);
            operationAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, location));
        }


        private IOperation FindLoopCollection(OperationAnalysisContext operationAnalysisContext)
        {
            var operation = (ILoopOperation)operationAnalysisContext.Operation;
            switch (operation.LoopKind)
            {
                case LoopKind.None:
                    break;
                case LoopKind.While:
                    break;
                case LoopKind.For:
                    var forLoop = (IForLoopOperation) operation;
                    int i = 2;
                    break;
                case LoopKind.ForEach:
                    var foreachOperation 
                        = (IForEachLoopOperation)operation;
                    return foreachOperation.Collection;
            }

            return null;
        }

        private bool CanBeusedAsArray(OperationAnalysisContext operationAnalysisContext, IdentifierNameSyntax collection)
        {
            var methodBlock = operationAnalysisContext.ContainingSymbol.DeclaringSyntaxReferences[0].GetSyntax().ChildNodes().
                OfType<BlockSyntax>().First();

            var expresionStatements = methodBlock.ChildNodes().OfType<ExpressionStatementSyntax>();
            
            foreach (var expresionStatement in expresionStatements)
            {
                if (expresionStatement.Expression.ChildTokens()
                    .OfType<IdentifierNameSyntax>().Any(syntax => syntax.Identifier == collection.Identifier))
                    return false;
            }

            return true;
        }

        private Location FindCollectionDeclarationLocation(IdentifierNameSyntax collection)
        {
            var root = collection.SyntaxTree.GetRoot();
            var collectionSymbol = root.DescendantNodesAndSelf().OfType<VariableDeclarationSyntax>()
                .FirstOrDefault(syntax => syntax.Variables[0].Identifier.Text == collection.Identifier.Text);
            return collectionSymbol?.GetLocation();
        }
    }
}
