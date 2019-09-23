using System;



namespace level_7_tokensAndEvents
{
    
    public interface ICanEat
    {
        //bs interface
        int Eat(int amount);

    }
    public class Foody : ICanEat {

        public int Threshold { get; private set; } //prop name thus top and Pascal cased
        public int Size { get; private set; } //prop name thus top and Pascal cased


        public event EventHandler<ThresholdReachedEventArgs> OnThresholdReached; //this is a delegate
        public event EventHandler<ConditionFailedEventArgs> OnConditionFailed;

        
        //seems annoying to make a whole new constructor function just to change one arg, but is it bad to default null?
        public Foody ( 
            EventHandler<ThresholdReachedEventArgs> thresholdHandler, 
            EventHandler<ConditionFailedEventArgs> conditionFailedHandler=null, 
            int threshold = 10 ) 
        {
            OnThresholdReached += thresholdHandler;
            Threshold = threshold;
            Size = 0;
            
            if ( conditionFailedHandler != null )
            {
                OnConditionFailed += conditionFailedHandler;
            }
            else
            {
                Console.WriteLine("Not setting on condition failed handler");
            }
        }

        public Foody() 
        {

        }



        
        Func<int, int> foodToSizeIncrease = n => n*2; //local private variable
        public int Eat(int foodAmount) 
        {
            Size += foodToSizeIncrease(foodAmount);
            
            if ( Size > Threshold ) 
            {
                OnThresholdReached(
                    this, 
                    new ThresholdReachedEventArgs { 
                        ThresholdReached = Threshold, 
                        EventTime = DateTime.Now 
                    }
                );
            } 
            
            else 
            {
                if ( OnConditionFailed != null ) {
                    OnConditionFailed(
                        this,
                        new ConditionFailedEventArgs {
                            EventTime = DateTime.Now,
                            Reason = "Threshold not met"
                        }
                    );
                }
            }
            return Size;
        }
    }
}