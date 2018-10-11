using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace DataLocalityAnalyzer
{
    public static class CollectionFinder 
    {
        public static ICollection<ISymbol> GetSymbols(Compilation compilation)
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

        private static ICollection<ISymbol> GetLocalSymbolsFromMethod(IEnumerable<ISymbol> methods, Compilation compilation)
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

        public static ICollection<ISymbol> GetCollections(Compilation compilation)
        {
            IEnumerable<ISymbol> symbols = GetSymbols(compilation);
            List<ISymbol> returnSymbols = new List<ISymbol>();

            returnSymbols.AddRange(symbols.OfType<ILocalSymbol>().
                Where(s => s.Type.ToString().Contains("System.Collection"))
                );

            returnSymbols.AddRange(symbols.OfType<IPropertySymbol>().
                Where(s => s.Type.ToString().Contains("System.Collection"))
            );

            returnSymbols.AddRange(symbols.OfType<IFieldSymbol>().
                Where(s => s.Type.ToString().Contains("System.Collection"))
            );

            return returnSymbols;
        }
    }
}
