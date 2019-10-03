using System;

namespace level_5_delegatesAndEvents
{
    class Program
    {
        //Here I actually create the handlers. In the namespace,(EventTypes.cs) resides the event arg data structures
        //Foody can raise a ThresholdReached event if the stomach is full, so inside of Foody one must create two things
        //1. The EventHandler delegate that eventually stores a reference of our handling method
        //2. The OnEvent virtual which then checks to see if the delegate has a valid reference, if it does, then it calls this method
        //I then create the handlers outside the context of the class, passing in a context reference "object sender"
    
        
        //where sender is the object that raises the event, in this case some instance of Foody
        static void HandleThresholdReached(object sender, ThresholdReachedEventArgs e) {
            Console.WriteLine("Handling threshold reached: size {0} at {1}", e.Size, e.TimeReached);
        }

        static void HandleConditionFailed(object sender, ConditionFailedEventArgs e) {
            Console.WriteLine("Condition from object {0} at {1}", sender.GetHashCode(), e.TimeFailed);
        }
        static void Main(string[] args)
        {
            BasicDelegate.Demo();
            Foody f = new Foody(10);
            f.ThresholdReached_delegate += HandleThresholdReached;
            f.ConditionFailed_delegate += HandleConditionFailed;

            f.Eat(2);
            f.Eat(2);
            f.Eat(2);

            f.ThresholdReached_delegate += (object s, ThresholdReachedEventArgs e) => { Console.WriteLine("Another handler!"); };
            f.Eat(2);

        }
    }
}
