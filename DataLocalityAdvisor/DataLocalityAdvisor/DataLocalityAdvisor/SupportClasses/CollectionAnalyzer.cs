using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Formatting;

namespace DataLocalityAnalyzer.SupportClasses
{
    class CollectionAnalyzer
    {
        private CodeFixContext _context;
        private readonly LocalDeclarationStatementSyntax _declaration;
        private readonly SyntaxNode _root;
        private readonly ISymbol _symbol;
        private readonly List<ReferencedSymbol> _references;

        public CollectionAnalyzer(CodeFixContext context)
        {
            _context = context;

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            _root = context.Document.GetSyntaxRootAsync(_context.CancellationToken).Result;

            _declaration = _root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LocalDeclarationStatementSyntax>().First();

            _symbol = context.Document.GetSemanticModelAsync().Result.LookupSymbols(diagnosticSpan.Start, null,
                 _declaration.Declaration.Variables[0].Identifier.ToString()).First();

            _references = SymbolFinder.FindReferencesAsync(_symbol, context.Document.Project.Solution).Result.ToList();
        }
        
        public Document GetDocumentWithArray()
        {
            Dictionary<SyntaxNode, SyntaxNode> nodesToReplace = FixCount();
            nodesToReplace.Add(_declaration,TurnIntoArrayDeclaration());
            var newdoc = _root.ReplaceNodes(nodesToReplace.Keys,(node, syntaxNode) => nodesToReplace[node]);
            return _context.Document.WithSyntaxRoot(newdoc);
        }

        private Dictionary<SyntaxNode, SyntaxNode> FixCount()
        {
            Dictionary<SyntaxNode, SyntaxNode> nodes = new Dictionary<SyntaxNode, SyntaxNode>();

            foreach (var use in _references.First().Locations)
            {
                var node = _root.FindNode(use.Location.SourceSpan).Parent;

                if (node.Kind() == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var access = node as MemberAccessExpressionSyntax;
                    if (access.Name.ToString() == "Count")
                        nodes.Add(node, access.WithName(SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("Length"))));
                }
            }

            return nodes;
        }

        private SyntaxNode TurnIntoArrayDeclaration()
        {
            var array = CreateArrayStatement();
            return SyntaxFactory.LocalDeclarationStatement(array)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        private VariableDeclarationSyntax CreateArrayStatement()
        {
            #region Tokens
            var openBracketToken = SyntaxFactory.Token(SyntaxKind.OpenBracketToken);
            var closeBracketToken = SyntaxFactory.Token(SyntaxKind.CloseBracketToken);
            var equalsToken = SyntaxFactory.Token(SyntaxKind.EqualsToken);
            #endregion

            //array Type
            var typeSyntax = _declaration.DescendantNodesAndSelf().OfType<PredefinedTypeSyntax>().First();

            //TurnIntoArrayDeclaration
            var ommitedArraySizeExpression = SyntaxFactory.OmittedArraySizeExpression();
            var arrayRankSpecifier = SyntaxFactory.ArrayRankSpecifier(openBracketToken,
                new SeparatedSyntaxList<ExpressionSyntax>().Add(ommitedArraySizeExpression),
                closeBracketToken);
            var arrayDeclaration = SyntaxFactory.ArrayType(typeSyntax, new SyntaxList<ArrayRankSpecifierSyntax>(arrayRankSpecifier));

            var arrayTypeWithSize = SyntaxFactory.ArrayType(typeSyntax, new SyntaxList<ArrayRankSpecifierSyntax>(arrayRankSpecifier));
            var init = SyntaxFactory.ArrayCreationExpression(arrayTypeWithSize);

            var arrayDeclarator =
                SyntaxFactory.VariableDeclarator(_declaration.Declaration.Variables[0].Identifier);
            
            //Initialization
            var initializer = _declaration.DescendantNodesAndSelf().OfType<InitializerExpressionSyntax>();
            if (initializer.Any())
            {
                init = init.WithInitializer(initializer.First());
                var equalsClause = SyntaxFactory.EqualsValueClause(equalsToken, init);
                arrayDeclarator =
                    SyntaxFactory.VariableDeclarator(_declaration.Declaration.Variables[0].Identifier)
                        .WithInitializer(equalsClause);
            }

            //finalDeclaration
            return SyntaxFactory.VariableDeclaration(arrayDeclaration).
                WithVariables(new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(arrayDeclarator));
        }
    }
}
