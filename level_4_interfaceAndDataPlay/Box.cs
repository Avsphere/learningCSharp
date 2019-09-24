using System;
using System.Collections.Generic;
using System.Linq;


namespace level_4_interfaceAndDataPlay {

    interface IExpensive<T> {

        int Value { get;}
        double AveragePrice();
        bool isExpensive();
        bool isCheap();
    }

    interface IColorful<T> {
        string Color { get; set; }
    }

    public class Box : IExpensive<Box> , IColorful<Box> {
        public List<int> items { get; private set; }
        public int Value { get; private set; }
        public int othervalue { get; }
        public string Color { get; set; }
        
        public int marketValue = 100;

        public Box() {
            items = Enumerable.Range(0, 100).ToList();
        }

        public int Add(int n) {
            items.Add(n);
            Value = items.Aggregate( (acc, el) => acc + el );
            return Value;
        }

        public double AveragePrice() {
            return items.Average();
        }

        public bool isExpensive() {
            return items.Average() > marketValue;
        }

        public bool isCheap() {
            return items.Average() < marketValue;
        }


    }

}