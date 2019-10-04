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

        static void Example2_InfiniteLoop()
        {
            IEnumerable<Box> GetInfiniteBoxes()
            {
                while(true)
                {
                    yield return new Box { Value = 5 };
                }
            }

            foreach( Box b in GetInfiniteBoxes().Take(10) )
                Console.WriteLine(b.Value);

            //even though we have an infinite loop recall that we aren't actually running the loop until explictly called
            //and when we are explictly calling we are only iterating over it 10 times
        }

        static void Example3_IRL_CustomIteration()
        {
            //The idea here is simply that we do not have to create an aux list to hold the data and only have to iterate once
            //This is in-fact how linq is written
            IEnumerable<int> WhereGt5 (List<int> ints)
            {
                foreach( int i in ints )
                {
                    if ( i > 5 )
                        yield return i;    
                }
            }

            foreach( int i in WhereGt5( new List<int> { 3,4,5,6,7,8} ) )
                Console.WriteLine(i);
        }


        static void Example4_IRL_StatefulIteration()
        {
            //The method containing yield will be paused after it stops iterating (ie you take 5 from a 10 list, when it comes back it starts on #6)
            //Thus the state is maintained
            //This seems kind of obvious
            IEnumerable<int> ShowingStateMaintain(List<int> ints)
            {
                int total = 0;
                foreach( int i in ints )
                {
                    total += i;
                    yield return total;
                }
            }
            var yieldedList = ShowingStateMaintain( new List<int> { 1,2,3,4,5 } );
            Console.WriteLine(yieldedList.Take(1).Last() );
            Console.WriteLine(yieldedList.Take(2).Last() ); // now it is 3 since we resumed execution
        }

        static void Example5_IRL_DeferredExecution()
        {
            //This I see as the nice example
            IEnumerable<Box> GetBoxes()
            {
                for( int i = 3; i < 10000; i++ )
                {
                    yield return new Box { Value = i };
                }
            }

            string ValueToColor(int v)
            {
                return v < 5 ? "blue" : "green";
            }

            //Get boxes is ~10000 long
            //This allows us to just take 5 rather than all 10000

            //REMEMBER THAT THIS IS NOT A TRANSDUCER REPLACEMENT

            var colors = GetBoxes()
                .Take(5)
                .Select( b => b.Value )
                .OrderByDescending( v => v )
                .Take(4)
                .Select( v => ValueToColor(v) );

            foreach( string c in colors )
                Console.WriteLine(c);

        }

        static void Main(string[] args)
        {
            // Example1_MultipleIterations();
            // Example2_InfiniteLoop();
            /*
                IRL there only a few supposed main use cases (besides being able to get away with anotherwise infinite loop)
            */
            // Example4_IRL_StatefulIteration();
            Example5_IRL_DeferredExecution();

        }
    }
}

