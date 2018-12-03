using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance_Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"c:\dados\";
            string tempPath = path + "tests.csv";
            File.AppendAllText(tempPath,"Tipo de Teste,Repetições,Otimizado,Não Otimizado \n");
            Console.WriteLine("Starting Tests");
            TestScript t = new TestScript();
            t.Start(TestScript.TestType.Estrutura_De_Dados);
            t.Start(TestScript.TestType.Coleção);
            t.Start(TestScript.TestType.Ambos);
            Console.WriteLine("Tests Ended");
            Console.ReadKey();
        }
    }
}
