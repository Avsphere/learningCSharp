using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace PlayingWithDataStructs {

    public class Box {
        public string name { get; set; }
        public int val { get; set; }
        public Box( int val=0, string n="demoName") {
            this.val = val;
            this.name = n;
        }
    }

    public static class MyLINQExtensions {
        public static double Median(this IEnumerable<double> source){
            if ( source.Count() == 0 ) {
                throw new InvalidOperationException("Beep its empty");
            }
            var sortedList = 
                from number in source
                orderby number
                select number;
            
            int halfwayIndex = (int)sortedList.Count() / 2;
            if ( sortedList.Count() % 2 == 0 ) {
                return (sortedList.ElementAt(halfwayIndex) + sortedList.ElementAt(halfwayIndex+1)) / 2;
            } else {
                return sortedList.ElementAt(halfwayIndex);
            }
        } 
    }
    public class LinqPlay {
        public static void basics() {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            var average = numbers.Average();
            IEnumerable<int> query = 
                from n in numbers
                where n > average
                select n;
            
            //A QUERY IS NOT EXECUTED UNTIL YOU ITERATE OVER THE QUERY VARIABLE

            foreach( var i in query ) {
                Console.WriteLine(i + " ");
            }

            //I could also do it the more familiar way of

            var numbersGreaterThanAverage = numbers.Where( x => x > average);

            Console.WriteLine( string.Join(", ", numbersGreaterThanAverage) );

            //append is a pure way to add an element
            var moreNumbers = numbersGreaterThanAverage.Append(1).Append(1);

            Console.WriteLine( numbersGreaterThanAverage.Count() + " " + moreNumbers.Count() );

            List<double> doubles = moreNumbers.Select<int, double>( i => i).ToList();

            Console.WriteLine( doubles.Median() );

        }

    }



}