using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            context.RegisterOperationAction(OnLoopOperation, OperationKind.MethodBodyOperation);
        }

        private void OnLoopOperation(OperationAnalysisContext operationAnalysisContext)
        {
            var methodBody = GetParentMethod(operationAnalysisContext);
            if (methodBody == null)
                return;

            var collections = FindLocalCollectionsAccessedOnLoop(operationAnalysisContext,methodBody);
            
            foreach (var collection in collections)
            {
                if (!CanBeusedAsArray(operationAnalysisContext, collection,methodBody))
                    return;
                var diagnostic = Diagnostic.Create(Rule, collection.Locations[0]);
                    operationAnalysisContext.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<ILocalSymbol> FindLocalCollectionsAccessedOnLoop(OperationAnalysisContext operationAnalysisContext,  IMethodBodyOperation methodBody)
        {
            //var operation = operationAnalysisContext.Operation as ILoopOperation;
            //IEnumerable<SyntaxNode> symbolsAccessedOnLoop;
            //if (operation.LoopKind == LoopKind.ForEach)
            //{
            //    symbolsAccessedOnLoop =  new  List<SyntaxNode>{((IForEachLoopOperation) operation).Collection.Syntax};
            //}
            //else
            //{
            //    symbolsAccessedOnLoop = operationAnalysisContext.Operation.Syntax.DescendantNodesAndSelf().Where(node =>
            //        node.Kind() == SyntaxKind.ElementAccessExpression );
            //}
            var body = ((IMethodBodyOperation) operationAnalysisContext.).BlockBody;
            var localCollections =
                methodBody.BlockBody.Locals.Where(symbol => symbol.Type.ToString().Contains("System.Collections"));
            
            //List<ILocalSymbol> returnList = new List<ILocalSymbol>();
            //foreach (var accessExpressionSyntax in symbolsAccessedOnLoop)
            //{
            //   var idenfier = accessExpressionSyntax.DescendantNodesAndSelf().First(node => node.Kind() == SyntaxKind.IdentifierName);
            //   var tt =  localCollections.Where(symbol => symbol.Name == idenfier.ToString());
            //    returnList.AddRange(tt);
            //}

            return localCollections;
        }

        private IMethodBodyOperation GetParentMethod(OperationAnalysisContext operationContext)
        {
            var oper  = operationContext.Operation.Parent;
            int count = 0;

            while (oper.Kind != OperationKind.IsType || count < 50)
            {
                if (oper.Kind == OperationKind.MethodBodyOperation)
                    return (IMethodBodyOperation)oper;
                    
                oper = oper.Parent;
                count++;
            }

            return null;
        }

        private bool CanBeusedAsArray(OperationAnalysisContext operationAnalysisContext, ILocalSymbol collection, IMethodBodyOperation methodBody)
        {
            var accesses = methodBody.Syntax.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                .Where(syntax => syntax.Identifier.ToString() == collection.Name);

            foreach (var identifierNameSyntax in accesses)
            {
                var parent = identifierNameSyntax.Parent;
                if (parent.Kind() != SyntaxKind.ElementAccessExpression &&
                    parent.Kind() != SyntaxKind.VariableDeclarator &&
                    parent.Kind() != SyntaxKind.SimpleMemberAccessExpression &&
                    parent.Kind() != SyntaxKind.ForEachStatement)
                    return false;

                if (parent.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccessed = ((MemberAccessExpressionSyntax) parent).Name.ToString();
                    if (memberAccessed != "Length" && memberAccessed != "Count")
                        return false;
                }
            }

            return true;
        }
    }
}

//var identifierTokens = operation.Syntax.DescendantNodesAndSelf().
//    Where(node =>  node.Kind() == SyntaxKind.IdentifierName 
//                   && node.Parent.Kind() == SyntaxKind.ElementAccessExpression);
//var tree = operationAnalysisContext.Operation.Syntax.SyntaxTree;
//var model = operationAnalysisContext.Compilation.GetSemanticModel(tree);
//var localSymbols = model.LookupSymbols(operation.Syntax.FullSpan.Start).OfType<ILocalSymbol>();
//foreach (var accessNode in accesses)
//{
//    accessNode.DescendantNodesAndSelf().Where(IdentifierNameSyntax);
//}

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


//var methodBlock = operationAnalysisContext.ContainingSymbol.DeclaringSyntaxReferences[0].GetSyntax().ChildNodes().
//    OfType<BlockSyntax>().First();

//var expresionStatements = methodBlock.ChildNodes().OfType<ExpressionStatementSyntax>();
//foreach (var expresionStatement in expresionStatements)
//{
//    //if (expresionStatement.Expression.ChildTokens()
//    //    .OfType<IdentifierNameSyntax>();//.Any(syntax => syntax.Identifier == collection.Identifier))
//        return false;
//}