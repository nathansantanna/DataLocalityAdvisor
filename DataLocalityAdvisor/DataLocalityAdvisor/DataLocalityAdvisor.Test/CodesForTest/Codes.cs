using System;
using System.Collections.Generic;
using System.Text;

namespace DataLocalityAnalyzer.test.CodesForTest
{
    public static class Codes
    {
        #region DataTest1
        public const string LocalSymbolsTestString = @"using System.Collections.Generic;

namespace ConsoleApplication1
{
public class program
    {
        public void main()
        {
            List<string> local1;
            int local2 = 0;
        }
    }
}
";
        
        public const string PropertiesTestString = @"using System.Collections.Generic;

namespace ConsoleApplication1
{
public class program
    {
            List<string> property1;
            int property2 {get;set;};
        public void main()
        {
        }
    }
}

}";
        #endregion
        #region Test2

        public static string[] MultiDoc = new[] {
            @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    public class program
    {
        public void main()
        {
            List<string> local1;
            int local2 = 0;
        }
    }
}",

            @"using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class test
    {
        public List<string> memberCollection { get; set; }
        public void testmethod()
        {
            List<string> local1;
            int local2 = 0;
        }
    }
}"
        };

        #endregion
        #region Test3

        public static string[] MultiDoc2 = new[] {
            @"using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class test
    {
        public List<string> memberCollection { get; set; }
        public void testmethod()
        {
            List<string> local1 = new List<string>(); 
            int local2 = 0;
        }
    }
}",
            @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    public class program
    {
        public void main()
        {
            List<string> local1;
            int local2 = 0;
            test t = new test();
            test.memberCollection.Add(""lororo"");
            foreach (var memberstr in test.memberCollection)
            {
                Console.Writeline(memberstr)
            }   
        }
    }
}"

           
        };

        #endregion
        #region SimpleStructTestCode

        public const string ClassToStructSimpleClass = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    public class Particle
    {
        int id;
        private bool isActive;
    }
}";

        public const string ClassToStructSimpleClassFixed = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    public struct Particle
    {
        int id;
        private bool isActive;
    }
}";


        #endregion
        #region ClassTostructWithMethod
        public const string ClassWithMethod = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    public class Program
    {
        int id;
        private bool isActive;

        void Main()
        {
            Console.Write(""Should not have Diagnostic"");
        }
}
}";
        #endregion
        #region ClassTostructWithMethod
        public const string ClassWithCollection = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    public class Program
    {
        int id;
        private bool isActive;
        public List<string> teste;
}
}";
        #endregion
        #region ClassWithNonBasicMember
        public const string ClassWithNonBasicMember = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    struct teste
    {
        string name;
        List<string> collectionMember;
    }
    public class Program
    {
        int id;
        private bool isActive;
        teste customMember;
    }
}";
        #endregion
        #region ClassWithNonBasicMember
        public const string SimpleStruct = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    struct teste
    {
        string name;
    }
}";
        #endregion
        #region ClassWithNonBasicMember
        public const string ClassWithInheritance = @"using System.Collections.Generic;
namespace ConsoleApplication1
{
    class ParentClass
    {
    }

    class Teste : ParentClass
    {
        string name;
    }
}";
        #endregion
    }
}



