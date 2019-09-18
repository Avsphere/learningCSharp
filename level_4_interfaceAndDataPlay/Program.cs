using System;

namespace level_4_interfaceAndDataPlay
{
    class Program
    {
        static void Main(string[] args)
        {
            Box b = new Box();
            Console.WriteLine(b.isCheap() + " " + b.isExpensive());

            int t  = Functionalish.addThenSquare(3);
            Console.WriteLine(t);
        }
    }
}
