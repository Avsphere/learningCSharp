using System;
using System.Collections.Generic;
using System.Linq;


namespace level_4_interfaceAndDataPlay {

    interface IExpensive<T> {
        int value {get;}
        int averagePrice { get; set; }
        bool isExpensive();
        bool isCheap();
    }

    public class Box : IExpensive<Box>{
        public List<int> items { get; private set; }
        
        public int value { get; }
        public int averagePrice { get; set; } = 5;
        public Box() {
            items = Enumerable.Range(0, 100).ToList();
        }

        public bool isExpensive() {
            return items.Average() > averagePrice;
        }

        public bool isCheap() {
            return items.Average() < averagePrice;
        }


    }

    public static class Extensions {
        public static Func<T, TReturn2> Compose<T, TReturn1, TReturn2>(this Func<TReturn1, TReturn2> func1, Func<T, TReturn1> func2 ){
            return x => func1(func2(x));
        }
    }
    public class Functionalish {
        public static Func<int, int> square = x => x * x;
        public static Func<int, int> add3 = x => x + 3;
        public static Func<int, int> addThenSquare = x => add3.Compose(square);
    }
}