using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DataLocalityAnalyzer.test
{
    [TestClass]
    public class CollectionAnalyzerTests
    {
        private CompilationAnalyzer _compilationAnalyzer;

        [TestInitialize]
        public void Setup()
        {
            _compilationAnalyzer = new CompilationAnalyzer();
        }

        [DataRow(Codes.LocalSymbolsTestString,2),
        DataRow(Codes.PropertiesTestString,2)]
        [DataTestMethod]
        public void GetSymbolsForSimpleCompilationFile(string source,int expected)
        {
            Compilation compilation = GetProjectCompilation(new[]{source});
            var localSymbols = _compilationAnalyzer.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,2);
        }

        [TestMethod]
        public void GetSymbolsFromMultipleDocumentsWihtoutDuplicatesTest()
        {
            Compilation compilation = GetProjectCompilation(Codes.MultiDoc);
            var localSymbols = _compilationAnalyzer.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,5);
        }

        protected Compilation GetProjectCompilation(string[] docs)
        {
            var project = DiagnosticVerifier.CreateProject(docs);
            return project.GetCompilationAsync().Result;
        }
    }
}
