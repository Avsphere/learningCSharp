using System;


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

        public static void Demo() {
            Add add4 = AddFour;
            Add add3 = AddThree;
            Add sub4 = SubtractFour;

            Console.WriteLine("should be 8 : " + add4(4));
            Console.WriteLine("should be 6 : "+ add3(3));

            Console.WriteLine("Should be 7 : " + AddAdd(add4, add3, 0) );

            //subtract has the same signature so I can use it as well
            Console.WriteLine("Should be 0 : " + AddAdd(add4, sub4, 0) );

        }
    }
}