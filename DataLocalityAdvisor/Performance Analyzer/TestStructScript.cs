using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance_Analyzer
{
    using System;
    class TestScript
    {
        private string path = @"c:\dados\";
        struct ProjectileStruct
        {
            public char[] Name;
            public float Position;
            public float Velocity;
            public float Acceleration;
        }
 
        class ProjectileClass
        {
            public char[] Name;
            public float Position;
            public float Velocity;
            public float Acceleration;
        }

        public enum TestType
        {
            Estrutura_De_Dados,
            Coleção,
            Ambos
        }

        public void Start(TestType test)
        {
            int[] tests = {5000000,2500000,1000000,7500000,5000000,2500000};
            
            foreach (var t in tests)
            {
                for (int i = 0; i < 5; i++)
                {
                    DoTest(t,test);
                }
            }
        }

        public void DoTest(int repetitions, TestType test)
        {
            string tempPath = path + "tests.csv";
            int collectionsSize = repetitions;
            ProjectileStruct[] structArray = new ProjectileStruct[collectionsSize];
            ProjectileClass[] classArray = new ProjectileClass[collectionsSize];

            List<ProjectileClass> classList = new List<ProjectileClass>();

            ProjectileStruct[] testArray = new ProjectileStruct[collectionsSize];
            List<ProjectileStruct> testList = new List<ProjectileStruct>();

            for (int i = 0; i < collectionsSize; ++i)
            {
                classArray[i] = new ProjectileClass();
                classList.Add(new ProjectileClass());
                testArray[i] = new ProjectileStruct();
                testList.Add(new ProjectileStruct());
                structArray[i] = new ProjectileStruct();
            }

            Shuffle(structArray);
            Shuffle(classArray);
            Shuffle(ref classList);
            Shuffle(testArray);
            Shuffle(ref testList);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            long NotOptimized = 0;
            long Optimized = 0;

            switch (test)
            {
                case TestType.Estrutura_De_Dados:
                    #region DataStructureTest
                    for (int i = 0; i < collectionsSize; ++i)
                    {
                        Update(structArray[i]);
                    }
                    
                    Optimized = sw.ElapsedMilliseconds;
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < collectionsSize; ++i)
                    {
                        Update(classArray[i]);
                    }
                    NotOptimized = sw.ElapsedMilliseconds;
                    break;
                    #endregion
                case TestType.Coleção:
                    #region CollectionTest
                    for (int i = 0; i < collectionsSize; ++i)
                    {
                        Update( testArray[i]);
                    }
                    Optimized = sw.ElapsedMilliseconds;
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < collectionsSize; ++i)
                    {
                        Update(testList[i]);
                    }
                    NotOptimized = sw.ElapsedMilliseconds;
                    
                    #endregion
                    break;
                case TestType.Ambos:
                    #region AllTest

                    for (int i = 0; i < collectionsSize; ++i)
                    {
                        Update( structArray[i]);
                    }
                    Optimized = sw.ElapsedMilliseconds;

                    Shuffle(classList.ToArray());
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < collectionsSize; ++i)
                    {
                        Update(classList[i]);
                    }
                    NotOptimized = sw.ElapsedMilliseconds;
                    #endregion
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(test), test, null);
            }
            File.AppendAllText(tempPath,$"{test},{repetitions},{Optimized},{NotOptimized} \n");
            Console.WriteLine("test done " + test +"  " +repetitions );
            Console.WriteLine($"{test},{repetitions},{Optimized},{NotOptimized} \n");
        }
 
        void Update(ProjectileClass projectile)
        {
            projectile.Position += projectile.Acceleration;
            projectile.Position += projectile.Velocity ;
        }

        void Update(ProjectileStruct projectile)
        {
            projectile.Position += projectile.Acceleration;
            projectile.Position += projectile.Velocity ;
        }
 
        //void Update(ref ProjectileStruct projectile, float time)
        //{
        //    projectile.Position += projectile.Velocity * time;
        //}
 
        //void Update(ProjectileClass projectile, float time)
        //{
        //    projectile.Position += projectile.Velocity * time;
        //}
 
        public static void Shuffle<T>(T[] list)  
        {
            Random random = new Random();
            for (int n = list.Length; n > 1; )
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void Shuffle<T>( ref List<T> list)  
        {
            Random random = new Random();
            for (int n = list.Count; n > 1; )
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
