using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DataLocalityAnalyzer
{
    public class CompilationAnalyzer
    {

        internal void SemanticAction(SemanticModelAnalysisContext semanticModelAnalysisContext)
        {
            throw new NotImplementedException();
        }

        public ICollection<ISymbol> GetSymbols(Compilation compilation)
        {
            List<ISymbol> returnSymbols = new List<ISymbol>();
            foreach (SyntaxTree compilationSyntaxTree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(compilationSyntaxTree);
                var methods = model.Compilation.GetSymbolsWithName(s => true, SymbolFilter.Member).
                    Where(currentSymbol => currentSymbol.Kind == SymbolKind.Method);
                foreach (var method in methods)
                {
                    int index = method.DeclaringSyntaxReferences[0].Span.End - 2;
                    returnSymbols.AddRange(model.LookupSymbols(index)
                    .Where(symbol => symbol.Kind == SymbolKind.Local));

                }

                returnSymbols.AddRange( model.Compilation.GetSymbolsWithName(s => true).
                Where(currentSymbol => currentSymbol.Kind == SymbolKind.Property || currentSymbol.Kind == SymbolKind.Field));
            }

            return returnSymbols;
        }


        internal void EndcompilationAction(CompilationAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}
