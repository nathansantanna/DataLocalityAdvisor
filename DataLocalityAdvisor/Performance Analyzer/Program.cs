using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance_Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Tests");
            TestScript t = new TestScript();
            t.Start(TestScript.TestType.DataStructure);
            t.Start(TestScript.TestType.Collection);
            t.Start(TestScript.TestType.All);
            Console.ReadKey();
        }
    }
}
