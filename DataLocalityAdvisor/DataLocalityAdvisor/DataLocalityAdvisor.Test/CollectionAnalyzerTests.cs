using DataLocalityAnalyzer.test.CodesForTest;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DataLocalityAnalyzer.test
{
    [TestClass]
    public class CollectionAnalyzerTests
    {
        [DataRow(Codes.LocalSymbolsTestString,2),
        DataRow(Codes.PropertiesTestString,2)]
        [DataTestMethod]
        public void GetSymbolsForSimpleCompilationFile(string source,int expected)
        {
            Compilation compilation = DiagnosticVerifier.GetProjectCompilation(new[]{source});
            var localSymbols = CollectionFinder.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,2);
        }

        [TestMethod]
        public void GetSymbolsFromMultipleDocumentsTest()
        {
            Compilation compilation = DiagnosticVerifier.GetProjectCompilation(Codes.MultiDoc);
            var localSymbols = CollectionFinder.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,5);
        }

        [TestMethod]
        public void GetSymbolsFromMultipleDocumentsWithoutDuplicatesTest2()
        {
            Compilation compilation = DiagnosticVerifier.GetProjectCompilation(Codes.MultiDoc2);
            var localSymbols = CollectionFinder.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,6); 
        }

        [TestMethod]
        public void GetCollectionsSymbolsTest()
        {
            Compilation compilation = DiagnosticVerifier.GetProjectCompilation(Codes.MultiDoc2);
            var collections = CollectionFinder.GetCollections(compilation);
            Assert.AreEqual(collections.Count,3);
        }
    }
}
 