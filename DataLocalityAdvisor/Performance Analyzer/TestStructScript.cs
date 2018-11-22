using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Performance_Analyzer
{
    using System;
    class TestScript 
    {
        struct ProjectileStruct 
        {
            public float Position;
            public float Velocity;
        }
 
        class ProjectileClass
        {
            public float Position;
            public float Velocity;
        }

        public enum TestType
        {
            DataStructure,
            Collection,
            All
        }
 
        public void Start(TestType test)
        {
            const int count = 10000000;
            ProjectileStruct[] structArray = new ProjectileStruct[count];
            ProjectileClass[] classArray = new ProjectileClass[count];

            List<ProjectileClass> classList = new List<ProjectileClass>();

            float[] testArray = new float[10000000];
            List<float> testList = new List<float>();

            for (int i = 0; i < count; ++i)
            {
                classArray[i] = new ProjectileClass();
                classList.Add(new ProjectileClass());
                testArray[i] = i;
                testList.Add(i);
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
                case TestType.DataStructure:
                    #region DataStructureTest
                    for (int i = 0; i < count; ++i)
                    {
                        UpdateProjectile(ref structArray[i], 0.5f);
                    }
                    Optimized = sw.ElapsedMilliseconds;
 
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < count; ++i)
                    {
                        UpdateProjectile(classArray[i], 0.5f);
                    }
                    NotOptimized = sw.ElapsedMilliseconds;
                    break;
                    #endregion
                case TestType.Collection:
                    #region CollectionTest
                    for (int i = 0; i < count; ++i)
                    {
                        var tt =  testArray[i];
                    }
                    Optimized = sw.ElapsedMilliseconds;
                    
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < count; ++i)
                    {
                        var tt =  testList[i];
                    }
                    NotOptimized = sw.ElapsedMilliseconds;
                    #endregion
                    break;
                case TestType.All:
                    #region AllTest

                    for (int i = 0; i < count; ++i)
                    {
                        UpdateProjectile( ref structArray[i], 0.5f);
                    }
                    Optimized = sw.ElapsedMilliseconds;

                    Shuffle(classList.ToArray());
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < count; ++i)
                    {
                        UpdateProjectile(classList[i], 0.5f);
                    }
                    NotOptimized = sw.ElapsedMilliseconds;

                    #endregion
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(test), test, null);
            }
            
 
            string report = string.Format(
                "###################################\n"+
                "{2}\n" +
                "NotOptimized,{0}\n" +
                "Optimized,{1}\n"+
                "###################################\n",
                NotOptimized,
                Optimized,
                test.ToString()
            );
            Console.WriteLine(report);
        }
 
        void UpdateProjectile(ref ProjectileStruct projectile, float time)
        {
            projectile.Position += projectile.Velocity * time;
        }
 
        void UpdateProjectile(ProjectileClass projectile, float time)
        {
            projectile.Position += projectile.Velocity * time;
        }
 
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
