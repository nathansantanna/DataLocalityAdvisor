using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.FindSymbols;

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

        public ICollection<ISymbol> GetCollections(Compilation compilation)
        {
            IEnumerable<ISymbol> symbols = GetSymbols(compilation);
            List<ISymbol> returnSymbols = new List<ISymbol>();
            var t = typeof(ILocalSymbol);

            returnSymbols.AddRange(symbols.
                Where(s => (ILocalSymbol)s.Type.ToString().Contains("System.Collection"))
                );

            returnSymbols.AddRange(symbols.OfType<IPropertySymbol>().
                Where(s => s.Type.ToString().Contains("System.Collection"))
            );
            return returnSymbols;
        }

        internal void EndcompilationAction(CompilationAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}
