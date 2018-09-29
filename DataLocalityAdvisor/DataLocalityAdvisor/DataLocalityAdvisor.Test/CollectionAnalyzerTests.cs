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
        public async System.Threading.Tasks.Task GetSymbolsForSimpleCompilationFileAsync(string source,int expected)
        {
            Compilation compilation = await GetProjectCompilationAsync(new[]{source});
            var localSymbols = CollectionFinder.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,2);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetSymbolsFromMultipleDocumentsTestAsync()
        {
            Compilation compilation = await GetProjectCompilationAsync(Codes.MultiDoc);
            var localSymbols = CollectionFinder.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,5);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetSymbolsFromMultipleDocumentsWithoutDuplicatesTest2Async()
        {
            Compilation compilation = await GetProjectCompilationAsync(Codes.MultiDoc2);
            var localSymbols = CollectionFinder.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,6);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetCollectionsSymbolsTestAsync()
        {
            Compilation compilation = await GetProjectCompilationAsync(Codes.MultiDoc2);
            var collections = CollectionFinder.GetCollections(compilation);
            Assert.AreEqual(collections.Count,3);
        }
     


        protected async System.Threading.Tasks.Task<Compilation> GetProjectCompilationAsync(string[] docs)
        {
            var project = DiagnosticVerifier.CreateProject(docs);
            return await project.GetCompilationAsync();
        }
    }
}
