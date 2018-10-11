using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataLocalityAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Rename;


namespace DataLocalityAdvisor
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixStructToClass)), Shared]
    public class CodeFixStructToClass : Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ConvertClassToStructAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    Resources.LampTitle,
                    c => TransformClassInStruct(context.Document, declaration, c), 
                    Resources.LampTitle ),
                diagnostic);
        }

        private async Task<Document> TransformClassInStruct(Document document, TypeDeclarationSyntax classDeclaration, CancellationToken cancellationToken)
        {
            var newmembers = GetOnlyStructValidMembers(classDeclaration.Members);
            var structdec = ConvertClassToStructDeclaration(classDeclaration as ClassDeclarationSyntax)
                .WithMembers(newmembers)
                .WithAdditionalAnnotations(Formatter.Annotation);

            var root = document.GetSyntaxRootAsync(cancellationToken).Result;
            var newdoc = root.ReplaceNode(classDeclaration,structdec);
            
            return document.WithSyntaxRoot(newdoc);
        }

        private StructDeclarationSyntax ConvertClassToStructDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            var structdec = SyntaxFactory.StructDeclaration(classDeclaration.Identifier)
                .WithModifiers(classDeclaration.Modifiers)
                .WithAttributeLists(classDeclaration.AttributeLists)
                .WithLeadingTrivia(classDeclaration.GetLeadingTrivia())
                .WithTrailingTrivia(classDeclaration.GetTrailingTrivia())
                .WithTriviaFrom(classDeclaration);
            return structdec;
        }

        private SyntaxList<MemberDeclarationSyntax> GetOnlyStructValidMembers(SyntaxList<MemberDeclarationSyntax> currentMembers)
        {
            var newmembers = new SyntaxList<MemberDeclarationSyntax>();
            foreach (var member in currentMembers)
            {
                switch (member.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                        newmembers = newmembers.Add(member);
                        break;

                    case SyntaxKind.ConstructorDeclaration:
                        if(((ConstructorDeclarationSyntax)member).ParameterList.Parameters.Count > 0)
                            newmembers = newmembers.Add(member);
                        break;
                }
            }
            return newmembers;
        }
    }
}