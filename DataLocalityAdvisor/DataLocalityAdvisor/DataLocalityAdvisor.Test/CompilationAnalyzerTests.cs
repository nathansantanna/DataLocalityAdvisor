using System;
using System.Collections.Generic;
using System.Text;
using DataLocalityAdvisor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLocalityAnalyzer.test
{
    class CompilationAnalyzerTests
    {
        private string _source = @"using System;
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
            public List<string> test { get; set; }
        }
    }
}";
        [TestMethod]
        public void FindNamespaceSymbols()
        {

        }
    }
}
