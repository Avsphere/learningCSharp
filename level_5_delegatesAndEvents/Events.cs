using System;


namespace level_5_delegatesAndEvents {

    public class Foody {
        private int totalEaten;
        private int foodTillFull;

        public Foody(int threshold) {
            foodTillFull = threshold;
        }

        public event EventHandler<ThresholdReachedEventArgs> ThresholdReached_delegate;
        public event EventHandler<ConditionFailedEventArgs> ConditionFailed_delegate;
        


        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e) {
            EventHandler<ThresholdReachedEventArgs> handler = ThresholdReached_delegate;
            if ( handler != null ) {
                handler(this, e);
            } else {
                Console.WriteLine("Hmmm OnThreshold does not appear to have a handler yet");
            }
        }

        protected virtual void OnConditionFailed(ConditionFailedEventArgs e) {
            EventHandler<ConditionFailedEventArgs> handler = ConditionFailed_delegate;
            if ( handler != null) {
                handler(this, e);
            } else {
                Console.WriteLine("OnConditionFailed does not have a handler yet");
            }
        }
        
        

        private int CalculateFoodSize(int x ) {
            //very impressive business logic
            return x*2;
        }
        public void Eat(int x) {
            totalEaten += CalculateFoodSize(x);
            if ( totalEaten > foodTillFull ) {
                ThresholdReachedEventArgs args = new ThresholdReachedEventArgs(totalEaten, DateTime.Now);
                OnThresholdReached(args);
            } else {
                ConditionFailedEventArgs args = new ConditionFailedEventArgs(DateTime.Now);
                OnConditionFailed(args);
            }
        }



    }


}