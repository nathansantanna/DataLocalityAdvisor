using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using TestHelper;
using DataLocalityAdvisor;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis.CSharp;

namespace DataLocalityAdvisor.Test
{
    [TestClass]
    public class LoopDiagnosticsTests : CodeFixVerifier
    {
        [TestMethod]
        //No diagnostics expected to show up
        public void SimpleStructToClassDiagnostic()
        {

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs", 12, 29),
            };
            var expected = new DiagnosticResult(ConvertClassToStructAnalyzer.Rule,expectedDiagnosisLocation);
          
            VerifyCSharpDiagnostic(Codes.ClassToStruct,expected);
        }

        protected override Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConvertClassToStructAnalyzer();
        }
    }
}
