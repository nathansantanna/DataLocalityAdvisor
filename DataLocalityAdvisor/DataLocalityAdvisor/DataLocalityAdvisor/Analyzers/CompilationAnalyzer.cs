using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

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

             IEnumerable<ISymbol>  methods = compilation.GetSymbolsWithName(s => true, SymbolFilter.Member).
                Where(currentSymbol => currentSymbol.Kind == SymbolKind.Method);
            
            IEnumerable<ISymbol> propertySymbols = compilation.GetSymbolsWithName(s => true).Where(currentSymbol =>
                currentSymbol.Kind == SymbolKind.Property ||
                currentSymbol.Kind == SymbolKind.Field && !returnSymbols.Contains(currentSymbol));

            var localSymbols = GetLocalSymbolsFromMethod(methods, compilation).Where(currentSymbol => !returnSymbols.Contains(currentSymbol));
           
            returnSymbols.AddRange(localSymbols);
            returnSymbols.AddRange(propertySymbols);

            return returnSymbols;
        }

        private ICollection<ISymbol> GetLocalSymbolsFromMethod(IEnumerable<ISymbol> methods, Compilation compilation)
        {
            List<ISymbol> returnSymbols = new List<ISymbol>();

            foreach (var method in methods)
            {
                var model = compilation.GetSemanticModel(method.Locations[0].SourceTree);
                int index = method.DeclaringSyntaxReferences[0].Span.End - 2;
                returnSymbols.AddRange(model.LookupSymbols(index)
                    .Where(symbol => symbol.Kind == SymbolKind.Local && !returnSymbols.Contains(symbol)));
            }

            return returnSymbols;
        }

        public ICollection<ISymbol> GetCollections(IEnumerable<ISymbol> symbols, Compilation compilation)
        {
            throw new NotImplementedException();
        }

        internal void EndcompilationAction(CompilationAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}
