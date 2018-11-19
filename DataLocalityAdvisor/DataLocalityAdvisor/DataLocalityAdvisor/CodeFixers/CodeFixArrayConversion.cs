using System;
using System.Composition;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLocalityAdvisor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace DataLocalityAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixArrayConversion)), Shared]
    public class CodeFixArrayConversion : CodeFixProvider
    {
        // TODO: Replace with actual diagnostic id that should trigger this fix.
        public const string DiagnosticId = "CodeFixArrayConversion";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = context.Document.GetSyntaxRootAsync(context.CancellationToken).Result;
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();
           
            
            var symbol = context.Document.GetSemanticModelAsync().Result.LookupSymbols(diagnosticSpan.Start, null,
                declaration.Declaration.Variables[0].Identifier.ToString()).First();
            var uses = SymbolFinder.FindReferencesAsync(symbol, context.Document.Project.Solution).Result;

            context.RegisterCodeFix(
                CodeAction.Create(
                    Resources.LampTitle,
                    c => GenericCollectionToArray(context, declaration, uses),
                    Resources.LampTitle),
                diagnostic);
        }

        private async Task<Document> GenericCollectionToArray(CodeFixContext context, LocalDeclarationStatementSyntax variableDeclaration, IEnumerable<ReferencedSymbol> usage)
        {
            var nodesToReplace = FixCount(usage,context.Document);
            nodesToReplace.Add(variableDeclaration,ArrayDeclaration(variableDeclaration));
            var root = context.Document.GetSyntaxRootAsync().Result;

            var newdoc = root.ReplaceNodes(nodesToReplace.Keys,(node, syntaxNode) => nodesToReplace[node]);
            return context.Document.WithSyntaxRoot(newdoc);
        }

        private Dictionary<SyntaxNode,SyntaxNode> FixCount(IEnumerable<ReferencedSymbol> usage, Document document)
        {
            var root = document.GetSyntaxRootAsync().Result;
            Dictionary<SyntaxNode, SyntaxNode> nodes = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var use in usage.First().Locations)
            {
                var node = root.FindNode(use.Location.SourceSpan).Parent;
                
                if (node.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var access = node as MemberAccessExpressionSyntax;
                    if (access.Name.ToString() == "Count")
                    {
                        var newName = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("Length"));
                        nodes.Add(node,access.WithName(newName));
                    }
                    else if (access.Name.ToString() == "Add")
                    {

                    }
                }
            }

            return nodes;
        }



        private SyntaxNode ArrayDeclaration(LocalDeclarationStatementSyntax collectionDeclaration)
        {
            var array = CreateArrayStatement(collectionDeclaration);
            return SyntaxFactory.LocalDeclarationStatement(array)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private VariableDeclarationSyntax CreateArrayStatement(
            LocalDeclarationStatementSyntax collectionDeclaration)
        {
            #region Tokens
            var openBracketToken = SyntaxFactory.Token(SyntaxKind.OpenBracketToken);
            var closeBracketToken = SyntaxFactory.Token(SyntaxKind.CloseBracketToken);
            var equalsToken = SyntaxFactory.Token(SyntaxKind.EqualsToken);
            //var numericToken = SyntaxFactory.Literal(10);
            //var numericLiteralExpression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, numericToken);
            #endregion
            //array Type
            var typeSyntax = collectionDeclaration.DescendantNodesAndSelf().OfType<PredefinedTypeSyntax>().First();
           

            //ArrayDeclaration
            var ommitedArraySizeExpression = SyntaxFactory.OmittedArraySizeExpression();
            var arrayRankSpecifier = SyntaxFactory.ArrayRankSpecifier(openBracketToken,
                new SeparatedSyntaxList<ExpressionSyntax>().Add(ommitedArraySizeExpression),
                closeBracketToken);
            var arrayDeclaration = SyntaxFactory.ArrayType(typeSyntax, new SyntaxList<ArrayRankSpecifierSyntax>(arrayRankSpecifier));

            //ArrayInitialization
            //var ommitedArraySizeExpression = SyntaxFactory.ArrayRankSpecifier(openBracketToken,
            //    new SeparatedSyntaxList<ExpressionSyntax>(),
            //    closeBracketToken);

            var arrayTypeWithSize = SyntaxFactory.ArrayType(typeSyntax, new SyntaxList<ArrayRankSpecifierSyntax>(arrayRankSpecifier));
            var init = SyntaxFactory.ArrayCreationExpression(arrayTypeWithSize);
            
            
            
            var arrayDeclarator =
                SyntaxFactory.VariableDeclarator(collectionDeclaration.Declaration.Variables[0].Identifier);

            var initializer = collectionDeclaration.DescendantNodesAndSelf().OfType<InitializerExpressionSyntax>();
            if (initializer.Any())
            {
                init = init.WithInitializer(initializer.First());
                var equalsClause = SyntaxFactory.EqualsValueClause(equalsToken, init);
                arrayDeclarator =
                    SyntaxFactory.VariableDeclarator(collectionDeclaration.Declaration.Variables[0].Identifier)
                        .WithInitializer(equalsClause);
            }
           
            //finalDeclaration
            return SyntaxFactory.VariableDeclaration(arrayDeclaration).
                WithVariables(new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(arrayDeclarator));
        }
    }
}
