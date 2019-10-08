using System;
using System.Collections.Generic;

namespace level_11_factory_patterns
{
    class Program
    {
        static void Main(string[] args)
        {
            // Position p = new Position { X = 2, Y = 2, Z = 1 };
            // Properties props = new Properties { Position = p, Name = "test" };

            // Button butt = new Button {
            //     Color = "blue",
            //     Html = "hello world",
            //     Properties = new Properties { 
            //         Position = new Position { X = 2, Y = 2, Z = 1 }, 
            //         Name = "test" 
            //     }
            // };

            // Console.WriteLine(Extensions.UIComponentToString(butt) );
            AbstractFactoryDemo.Demo();
        }
    }
}
