
using System.Linq;
using DataLocalityAdvisor;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;


namespace DataLocalityAnalyzer.test
{
    [TestClass]
    public class CollectionToArrayTests : CodeFixVerifier
    {
        [TestMethod]
        public void SimpleStructToClassDiagnostic()
        {
            var expectedLocation = GetDiagnosticPosition(new[] { Codes.ListOnAMethod }, "teste");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertToArrayAnalyzer.Rule, expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.ListOnAMethod, expected);
        }

        protected override Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CodeFixArrayConversion();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConvertToArrayAnalyzer();
        }

        protected LinePosition GetDiagnosticPosition(string[] sources, string symbolName)
        {
            var compilation = GetProjectCompilation(sources);
            var symbols = compilation.GetSymbolsWithName(s => true).ToList()[0].s
            return LinePosition.Zero;
            //var symbols = GetProjectCompilation(sources).symbolName, SymbolFilter.All);//.First().Locations[0].GetMappedLineSpan().Span.Start;
            //var local= symbols.ToList()[0].Locations[0].GetMappedLineSpan().Span.Start;;
            //return local;
            //return GetProjectCompilation(sources).GetSymbolsWithName(symbolName).First().Locations[0].GetMappedLineSpan().Span.Start;
        }
    }

}
