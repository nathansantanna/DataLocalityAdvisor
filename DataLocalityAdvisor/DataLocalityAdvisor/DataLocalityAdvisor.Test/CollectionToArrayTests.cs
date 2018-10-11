using System.Linq;
using DataLocalityAdvisor;
using DataLocalityAnalyzer.test.CodesForTest;
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
            var expectedLocation = GetDiagnosticPosition(new[] { Codes.ClassToStructSimpleClass }, "Particle");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertClassToStructAnalyzer.Rule, expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.ClassToStructSimpleClass, expected);
        }

        protected override Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CodeFixArrayConversion();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConvertToArraAnalyzer();
        }

        protected LinePosition GetDiagnosticPosition(string[] sources, string symbolName)
        {
            return GetProjectCompilation(sources).GetSymbolsWithName(symbolName).First().Locations[0].GetMappedLineSpan().Span.Start;
        }
    }

}
