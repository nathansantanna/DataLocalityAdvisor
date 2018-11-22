using System.Composition;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DataLocalityAnalyzer.SupportClasses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;


namespace DataLocalityAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeFixArrayConversion)), Shared]
    public class CodeFixArrayConversion : CodeFixProvider
    {
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
            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    Resources.LampTitle,
                    c => GenericCollectionToArray(context),
                    Resources.LampTitle),
                diagnostic);
        }

        private async Task<Document> GenericCollectionToArray(CodeFixContext context)
        {
            var collectionAnalyzer = new CollectionAnalyzer(context);
            return collectionAnalyzer.GetDocumentWithArray();
        }
    }
}