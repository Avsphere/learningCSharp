using System;
using System.Collections.Generic;
using System.Linq;


delegate string[] FilterStringList( List<string> stringList);

namespace level_5_delegatesAndEvents {
    static public class BasicDelegate {
        public delegate int Add(int value);


        public static int AddThree(int x) {
            return x + 3;
        }
        public static int AddFour(int x) {
            return x + 4;
        }

        public static int SubtractFour(int x ) {
            return x - 4;
        }

        //here this function takes two functions that have the signature of the add delegate
        public static int AddAdd(Add fn1, Add fn2, int x) {
            return fn1(x) + fn2(x);
        }


        public static string[] OddFilter( List<string> stringList )
        {
            return stringList.Where( s => s.Length % 2 != 0 ).ToArray();
        }

        public static string[] EvenFilter( List<string> stringList ) 
        {
            return stringList.Where( s => s.Length % 2 == 0 ).ToArray();
        }

        public static Func<List<string>, string[]> OddFilter_func = strList => strList
            .Where( s => s.Length % 2 != 0 ).ToArray();

        public static Func<List<string>, string[]> EvenFilter_func = strList => strList
            .Where( s => s.Length % 2 == 0 ).ToArray();

        public static void Demo() {
            Add add4 = AddFour;
            Add add3 = AddThree;
            Add sub4 = SubtractFour;

            Console.WriteLine("should be 8 : " + add4(4));
            Console.WriteLine("should be 6 : "+ add3(3));

            Console.WriteLine("Should be 7 : " + AddAdd(add4, add3, 0) );

            //subtract has the same signature so I can use it as well
            Console.WriteLine("Should be 0 : " + AddAdd(add4, sub4, 0) );



            List<string> myStringList = new List<string>() { "test", "strings", "are", "interesting?" };

            FilterStringList oddFilterFn = new FilterStringList(OddFilter);
            FilterStringList evenFilterFn = new FilterStringList(EvenFilter);


            FilterStringList oddFilterFn_func = new FilterStringList(OddFilter_func);


            FilterStringList[] filterArray = { oddFilterFn, evenFilterFn, oddFilterFn_func };


            // Predicate<string> isEvenLength = s => s.Length % 2 == 0;

            Func<string, bool> isEvenLength = s => s.Length % 2 == 0;
            string[] areEven = myStringList.Where(isEvenLength).ToArray();

            Console.WriteLine(" areEven : " + string.Join(",", areEven) );







        }
    }
}