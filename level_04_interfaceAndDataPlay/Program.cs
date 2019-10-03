using System;

namespace level_4_interfaceAndDataPlay
{
    class Program
    {
        static void Main(string[] args)
        {
            Box b = new Box();
            Console.WriteLine(b.isCheap() + " " + b.isExpensive());

            IExpensive<Box> ie = new Box();

            //notice that I cannot add items to the box
            b.Add(1000);

            Console.WriteLine("b value : " + b.Value );
            IExpensive<Box> cb = (IExpensive<Box>)b;

            Console.WriteLine("cb value : " + cb.Value ); //notice that cb is a reference to c it just has less properties

            IColorful<Box> cc = (IColorful<Box>)b; //now this only has the IColor pieces
            //Now I have essentially removed the add field

        }
    }
}
