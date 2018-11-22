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
        private string[] _basicTypes =
        {
            "decimal",
            "float",
            "char",
            "int",
            "string",
            "double",
        };
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
            var collections = FindLocalCollectionsAccessedOnLoop(operationAnalysisContext);
            
            foreach (var collection in collections)
            {
                if (!CanBeusedAsArray(operationAnalysisContext, collection))
                    return;
                var diagnostic = Diagnostic.Create(Rule, collection.Locations[0]);
                    operationAnalysisContext.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<ILocalSymbol> FindLocalCollectionsAccessedOnLoop(OperationAnalysisContext operationAnalysisContext)
        {

            var body = ((IMethodBodyOperation) operationAnalysisContext.Operation).BlockBody;
            var localCollections =
                body.Locals.Where(symbol => symbol.Type.ToString().Contains("System.Collections"));
            
            return localCollections;
        }

        private bool CanBeusedAsArray(OperationAnalysisContext operationAnalysisContext, ILocalSymbol collection)
        {
            var methodBody = operationAnalysisContext.Operation as IMethodBodyOperation;
            var accesses = methodBody.Syntax.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                .Where(syntax => syntax.Identifier.ToString() == collection.Name);
            var containedTypes = collection.DeclaringSyntaxReferences[0].GetSyntax().DescendantNodesAndSelf().OfType<TypeArgumentListSyntax>();
            foreach (TypeArgumentListSyntax genericTypes in containedTypes)
            {
                if (genericTypes.Arguments.Count > 1)
                    return false;
                foreach (var type
                    in genericTypes.Arguments)
                {
                    if(!_basicTypes.Contains(type.ToString()))
                       return false;
                   
                }
                //type.DescendantNodesAndSelf()
            }
            foreach (var identifierNameSyntax in accesses)
            {
                
                var parent = identifierNameSyntax.Parent;
                if (parent.Kind() != SyntaxKind.ElementAccessExpression &&
                    parent.Kind() != SyntaxKind.VariableDeclarator &&
                    parent.Kind() != SyntaxKind.SimpleMemberAccessExpression &&
                    parent.Kind() != SyntaxKind.ForEachStatement)
                    return false;
                if (parent.Kind() == SyntaxKind.VariableDeclarator)
                {
                    var declarator = ((VariableDeclaratorSyntax) parent);
                    var types = parent.DescendantNodesAndSelf().OfType<PredefinedTypeSyntax>();
                    if (types.Count() > 1)
                        return false;
                }
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