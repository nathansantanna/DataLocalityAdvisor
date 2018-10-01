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

namespace DataLocalityAnalyzer
{
    public  class CompilationAnalyzer
    {
        private List<ISymbol> _symbols;

        internal void SemanticAction(SemanticModelAnalysisContext semanticModelAnalysisContext)
        {
            throw new NotImplementedException();
        }

        internal void EndCompilationAction(CompilationAnalysisContext obj)
        { 
            throw new NotImplementedException();
        }

        public ICollection<string> VerifyCollectionsUse(Compilation compilation, ISymbol symbol )
        {
            ICollection<string> returnList = new List<string>();
            //SymbolFinder.FindReferencesAsync(symbol, compilation.SyntaxTrees.ToList()[0].);
            var teste = "";
            var att = symbol.GetAttributes();
            var teste2 = "";
            var model = compilation.GetSemanticModel(compilation.SyntaxTrees.ToList()[0]);
            
            return returnList;
        }

    }
}
