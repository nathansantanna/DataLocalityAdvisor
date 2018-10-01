using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis;
using TestHelper;

namespace DataLocalityAnalyzer.test
{
    [TestClass]
    public class CollectionUseAnalyzerTest
    {
        private CompilationAnalyzer _compilationAnalyzer;
        [TestInitialize]
        public void StartUp()
        {
            _compilationAnalyzer = new CompilationAnalyzer();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetCollectionsSymbolsTestAsync()
        {
            Compilation compilation = await DiagnosticVerifier.GetProjectCompilationAsync(Codes.MultiDoc2);
            var collections = CollectionFinder.GetCollections(compilation);

            var uses = _compilationAnalyzer.VerifyCollectionsUse(compilation, collections.ToArray()[2]);
            

            Assert.AreEqual(2, uses.Count);
        }
    }
}
