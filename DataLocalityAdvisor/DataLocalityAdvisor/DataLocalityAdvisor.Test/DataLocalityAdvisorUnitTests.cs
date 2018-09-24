using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using TestHelper;
using DataLocalityAdvisor;

namespace DataLocalityAdvisor.Test
{
    [TestClass]
    public class DiagnosticsTests : CodeFixVerifier
    {
        #region Props
        public string source = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {   
        public List<string> test { get; set; }
         Static void main()
        {
            string lororo = ""DataSetDateTime"";
        }
    }
}";
        #endregion

        //No diagnostics expected to show up
        [TestMethod]
        public void FindCollectionsTest()
        {
            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs", 12, 29),
                new DiagnosticResultLocation("Test0.cs", 15, 20) 
            };
            var expected = new DiagnosticResult(CollectionsAnalyzer.Rule,expectedDiagnosisLocation);
          
            VerifyCSharpDiagnostic(source,expected);
        }

        //Diagnostic and CodeFix both triggered and checked for
        public void TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "DataLocalityAdvisor",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);


            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DataLocalityAdvisorCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CollectionsAnalyzer();
        }
    }
}
