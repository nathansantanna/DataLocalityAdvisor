using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLocalityAnalyzer.test
{
    [TestClass]
    public class CollectionAnalyzerTests
    {
        #region Source String
        private string _source = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{
            public class Particle
        {
            public int id;
            public bool isActive;

            public void Render()
            {
            }
        }


        private Dictionary<int, Particle> PackageMyData { get; set; }
        private List<string> CanTurnToArray { get; set; };
        private int RandomMember { get; set; }

        public void main()
        {
            CanTurnToArray = new List<string>();
            CanTurnToArray.AddRange(new[]{"""","""",""""});
            List<string> NotValidToTurnInArray;
            NotValidToTurnInArray = new List<string>();
            NotValidToTurnInArray.Add(""""lululu"""");
            int i = 0;
        foreach (var VARIABLE in NotValidToTurnInArray)
        {
            Console.Write(""""Teste"""");
        }
        NotValidToTurnInArray.Add(""""lululu"""");
        foreach (var VARIABLE in PackageMyData)
        {
            if(VARIABLE.Value.isActive)
                VARIABLE.Value.Render();

        }
    }

}";
        #endregion

        private CompilationAnalyzer _compilationAnalyzer;
        [TestInitialize]
        public void Setup()
        {
            _compilationAnalyzer  = new CompilationAnalyzer();
        }

        [TestMethod]
        public void TestIfFindCorrectlyOnMultipleDocuments()
        {
            #region source
            string doc1 = @"using System.Collections.Generic;

namespace ConsoleApplication1
{
        public void main()
        {
            List<string> local1;
            int local2 = 0;
        }
    }

}";
            
string doc2 = @"using System.Collections.Generic;

namespace ConsoleApplication1
{
        public void main()
        {
            List<string> local1;
            int local2 = 0;
        }
    }

}";
            #endregion
        }

        [TestMethod]
        public void TestIfFindLocalSymbols()
        {
            #region source
            string source = @"using System.Collections.Generic;

namespace ConsoleApplication1
{
        public void main()
        {
            List<string> local1;
            int local2 = 0;
        }
    }

}";
            #endregion

            Compilation compilation = GetCompilation(source);
            var localSymbols = _compilationAnalyzer.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,2);
        }

        [TestMethod]
        public void TestIfFindProperties()
        {
            #region source
            string source = @"using System.Collections.Generic;

namespace ConsoleApplication1
{
            List<string> property1;
            int property2 {get;set;};
        public void main()
        {
        }
    }

}";
            #endregion

            Compilation compilation = GetCompilation(source);
            var localSymbols = _compilationAnalyzer.GetSymbols(compilation);
            Assert.AreEqual(localSymbols.Count,2);
        }


        private Compilation GetCompilation(string source)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(source);
            var compilation = CSharpCompilation.Create(_source, new[]{tree});
            return compilation;
        }
    }
}
