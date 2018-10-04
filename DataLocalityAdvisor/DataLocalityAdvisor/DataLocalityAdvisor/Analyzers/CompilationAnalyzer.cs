using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace DataLocalityAnalyzer
{
    public  class CompilationAnalyzer
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);
        private List<ISymbol> _symbols;

        internal void SemanticAction(SemanticModelAnalysisContext semanticModelAnalysisContext)
        {
            throw new NotImplementedException();
        }

        internal void EndCompilationAction(CompilationAnalysisContext compilation)
        { 

            throw new NotImplementedException();
        }

        public ICollection<string> VerifyCollectionsUse(Compilation compilation, ISymbol symbol)
        {
            var testete = compilation.ToMetadataReference();
            var refmea = compilation.GetMetadataReference(compilation.Assembly);
            var assemblyName = compilation.AssemblyName;
            var projectName = compilation.AssemblyName;
            var projectId = ProjectId.CreateNewId( assemblyName);
            var solution = new AdhocWorkspace().CurrentSolution
                .AddProject(projectId, projectName, assemblyName, LanguageNames.CSharp);
                //.AddMetadataReference(projectId, compilation.Assembly.GetMetadata().GetReference())
                //.AddMetadataReference(projectId, CorlibReference)
                //.AddMetadataReference(projectId, SystemCoreReference)
                //.AddMetadataReference(projectId, CSharpSymbolsReference)
                //.AddMetadataReference(projectId, CodeAnalysisReference);
            foreach (var tree in compilation.SyntaxTrees)
            {
                var newFileName = System.IO.Path.GetFileName(tree.FilePath);
                var documentId = DocumentId.CreateNewId(projectId, newFileName);
                solution = solution.AddDocument(documentId, newFileName, tree.GetText());
            }
            
            var teste2 = SymbolFinder.FindSimilarSymbols(symbol, compilation);

            //Dictionary<ISymbol, IEnumerable<ReferencedSymbol>> refs =
            //    new Dictionary<ISymbol, IEnumerable<ReferencedSymbol>>();

            //foreach (var symbb in teste2)
            //{
            //    var symrefs = SymbolFinder.FindReferencesAsync(symbb, solution).Result;
            //    refs.Add(symbb, symrefs);
            //}
            
            int r = 0;
            return new List<string>();
        }
    }
}
