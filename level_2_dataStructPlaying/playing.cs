using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace PlayingWithDataStructs {

    public class DataBox {
        public string name { get; set; }
        public int val { get; set; }
        public DataBox( int val=0, string n="demoName") {
            this.val = val;
            this.name = n;
        }
    }
    public class Playing {


        //You can only pass an anonymous object to a method that acepts a dynamic type ONLY
        //passing dynamic types are not reccomended
        //anonymous types are read only at a shallow state, nearly like js except you cannot add fields
        //the scope of an anonymous types are local to the method that they are defined in
        public static Object anonExmple = new { exampleField = "exampleValue" };
        public static Object anon() {
            var withString = new { field = "val" };
            var withArray = new { field =  new[] { "values", "in", "array" } };

            Object withNested = new {
                field = new { nested = true }
            };
            withArray.field[2] = "this will be changed since it is only shallow immutable";
            // Console.WriteLine("withArray after change {0} {1}", withArray.field ); //this destructers the array and prints only the first two values
            // Console.WriteLine("withArray joining print {0}", string.Join(",", withArray.field) );

            Object ret = new { withString, withArray, withNested };

            return ret;
        }

        public static void methodThatAcceptsAnon( dynamic someAnon) {
            Console.WriteLine(someAnon);
        }

        public static void instantiationPlay() {
            DataBox d0 = new DataBox();
            DataBox d1 = new DataBox() { val = 1, name = "friend" }; //this would fail IF I ONLY DID { get; } because this first constructs THEN it sets

            var boxes = new List<DataBox>() { d0, d1 };

            var names = boxes.Select( b => b.name ); //boxes is an extension method

            
            Console.WriteLine("names as a string: {0}", string.Join(" , ", names ));

        }

    }

    class TestGeneric<T> {
        private T memberVariable;
        public TestGeneric(T value) {
            memberVariable = value;
        }

        public T genericFunction(T val) {
            // return val + memberVariable; //given that the + is overloaded
            return val;
        }

        public void showingNamedParamters(T arg) {
            Console.WriteLine(genericFunction(val : arg));
        }
    }


}