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
}";
        
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
        

    }
}


