using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;

namespace DataLocalityAnalyzer
{
    class CompilationAnalyzer
    {
        
        private HashSet<string> _collections;

        internal void SemanticAction(SemanticModelAnalysisContext semanticModelAnalysisContext)
        {
            throw new NotImplementedException();
        }

        public List<ISymbol> GetCollections(SemanticModelAnalysisContext semanticModelAnalysisContext)
        {
            throw new NotImplementedException();
        }

        internal void EndcompilationAction(CompilationAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}
