using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TestHelper;
using DataLocalityAdvisor;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace DataLocalityAdvisor.Test
{
    [TestClass]
    public class StructToClassTests : CodeFixVerifier
    {
        private void SetupDiagnosticTest()
        {

        }
        [TestMethod]
        public void SimpleStructToClassDiagnostic()
        {
            var expectedLocation = GetDiagnosticPosition(new[] {Codes.ClassToStructSimpleClass}, "Particle");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertClassToStructAnalyzer.Rule,expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.ClassToStructSimpleClass,expected);
        }

        [TestMethod]
        public void SimpleStructToClassCodeFix()
        {
            VerifyCSharpFix(Codes.ClassToStructSimpleClass,Codes.ClassToStructSimpleClassFixed);
        }

        [TestMethod]
        public void DontSendDiagnosticClassWithCollection()
        {
            VerifyCSharpDiagnostic(Codes.ClassWithCollection);
        }

        [TestMethod]
        public void DontSendDiagnosticClassWithMethod()
        {
            VerifyCSharpDiagnostic(Codes.ClassWithMethod);
        }

        [TestMethod]
        public void DontSendDiagnosticClassWithCustomMember()
        {
            VerifyCSharpDiagnostic(Codes.ClassWithNonBasicMember);
        }

        [TestMethod]
        public void DontSendDiagnosticToStructs()
        {
            VerifyCSharpDiagnostic(Codes.SimpleStruct);
        }

        [TestMethod]
        public void DontSendDiagnosticToClassesWithinheritance()
        {
            var expectedLocation = GetDiagnosticPosition(new[] {Codes.ClassWithInheritance}, "ParentClass");

            var expectedDiagnosisLocation = new[]
            {
                new DiagnosticResultLocation("Test0.cs",expectedLocation.Line + 1,expectedLocation.Character + 1),
            };
            var expected = new DiagnosticResult(ConvertClassToStructAnalyzer.Rule,expectedDiagnosisLocation);
            VerifyCSharpDiagnostic(Codes.ClassWithInheritance,expected);
        }

        protected override Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new ConvertClassToStructAnalyzer();
        }

        protected LinePosition GetDiagnosticPosition(string[] sources, string symbolName)
        {
           return GetProjectCompilation(sources).GetSymbolsWithName(symbolName).First().Locations[0].GetMappedLineSpan().Span.Start;
        }
    }
}
