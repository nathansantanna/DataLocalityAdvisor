
using System.Linq;
using DataLocalityAdvisor;
using DataLocalityAnalyzer;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;


namespace CollectionToArray.Test
{
    [TestClass]
    public class CollectionToArrayTests : CodeFixVerifier
    {
        [TestMethod]
        public void FindCollectionInForEachLoop()
        {
            var expectedLocation = GetDiagnosticPosition(new[] { Codes.ListOnAForEach }, "teste");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertToArrayAnalyzer.Rule, expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.ListOnAForEach, expected);
        }

        [TestMethod]
        public void FindCollectionInForLoop()
        {
            var expectedLocation = GetDiagnosticPosition(new[] { Codes.findCollectionOnAForLoop }, "teste");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertToArrayAnalyzer.Rule, expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.findCollectionOnAForLoop, expected);
        }

        [TestMethod]
        public void FindCollectionInWhileLoop()
        {
            var expectedLocation = GetDiagnosticPosition(new[] { Codes.findCollectionOnAWhileLoop }, "teste");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertToArrayAnalyzer.Rule, expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.findCollectionOnAWhileLoop, expected);
        }

        [TestMethod]
        public void FindCollectionInDoubleLoop()
        {
            var expectedLocation = GetDiagnosticPosition(new[] { Codes.findCollectionOnADoubleLoop }, "teste");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertToArrayAnalyzer.Rule, expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.findCollectionOnADoubleLoop, expected);
        }

        [TestMethod]
        public void FixUnitializedLists()
        {
            VerifyCSharpFix(Codes.findAndFixDoubleCollectionOnADoubleLoop,Codes.FixedDoubleCollectionOnADoubleLoop);
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
            VariableDeclaratorSyntax localvar;
            foreach (var tree in compilation.SyntaxTrees)
            {
                var localDeclaration =
                    tree.GetRoot().DescendantNodesAndSelf().OfType<VariableDeclarationSyntax>().
                        Where(syntax => syntax.Variables[0].Identifier.Text ==  symbolName);
                
                if (localDeclaration.Any())
                {
                    localvar = localDeclaration.ToArray()[0].Variables[0];
                    return localvar.GetLocation().GetMappedLineSpan().Span.Start;
                }
            }
            return LinePosition.Zero;            
        }
    }
}
