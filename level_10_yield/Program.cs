using System;
using System.Collections.Generic;
using System.Linq;

namespace level_10_yield
{
    class Program
    {
        static IEnumerable<int> Example0_WithoutYield()
        {
            //The main difference with yield vs traditional pushing loops is that yield creates a state machine promise to add the numbers
            List<int> list = new List<int>();
            for( int i = 0; i < 10; i++ )
            {
                list.Add(i);
            }
            return list;
        }
        static IEnumerable<int> Example0_WithYield()
        {
            //we can only use yield when returning an IEnumberable
            for( int i = 0; i < 10; i++ )
            {
                yield return i; //here we implicitly add to the return, except that it is really adding to the state machine
            }

            /*
            Steps
            1. This function is called
            2. At each yield return we build onto our state machine
            3. It returns a type of IEnumberable, except that it is actually a promise to return a seqeunce and just exposes to us an iterator
             */
        }

        public class Box
        {
            public int Value { get; set; }

            // public Box(int _value)
            // {
            //     Value = _value;
            // }
        }

        static void Example1_MultipleIterations()
        {
            IEnumerable<Box> GetBoxes()
            {
                for( int i = 0; i < 10; i++ )
                {
                    yield return new Box { Value = 5 };
                }
            }

            void DoubleBoxValues_mutation( IEnumerable<Box> boxes )
            {
                foreach( Box b in boxes )
                {
                    b.Value *= 2;
                }
            }

            IEnumerable<Box> someBoxes = GetBoxes();
            DoubleBoxValues_mutation(someBoxes); 
            Console.WriteLine(someBoxes.First().Value); //here we return 5 even though we doubled

            /*
                What is happening here is GetBoxes returns a state machine that can create boxes
                When we call the doubleBoxValues we explictly traverse this someBoxes IEnumberable so that goes ahead and actually creates the boxes from the state machine
                However, all the boxes created inside the doubleBoxVlues are discarded since there are no references to them, GC removes
                We then call someBoxes.First() which again explictly uses the iterator and generates for us a new box
                Thus the valueis 5
            
             */

        }
        static void Main(string[] args)
        {
            Example1_MultipleIterations();
        }
    }
}

