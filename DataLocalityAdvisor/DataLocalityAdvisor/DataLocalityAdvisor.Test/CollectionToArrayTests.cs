
using System.Linq;
using DataLocalityAdvisor;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            VariableDeclarationSyntax localvar;
            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var localDeclaration =
                    tree.GetRoot().DescendantNodesAndSelf().OfType<VariableDeclarationSyntax>().
                        Where(syntax => syntax.Variables[0].Identifier.Text ==  symbolName);
                if (localDeclaration.Any())
                {
                    localvar = localDeclaration.ToArray()[0];
                    return localvar.GetLocation().GetMappedLineSpan().Span.Start;
                }
            }
            return LinePosition.Zero;            
        }
    }

}
