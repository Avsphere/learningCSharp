using System;
using System.Threading.Tasks;
using System.Threading;

namespace level_7_tokensAndEvents
{
    class Program
    {
        public static CancellationTokenSource cts = new CancellationTokenSource(); 
        public static void HandleThresholdReached(object sender, ThresholdReachedEventArgs eventArgs) {
            Console.WriteLine("Threshold of : {0} reached on {1} ", eventArgs.ThresholdReached, eventArgs.EventTime);
            cts.Cancel();
        }

        public static void HandleConditionFailed(object sender, ConditionFailedEventArgs eventArgs) {
            Console.WriteLine("Condition Failed. Reason : {0}. Failed on : {1}", eventArgs.Reason, eventArgs.EventTime);
        }


        //by using action rather than Func<int, int> I can be more flexible for this case, if I wanted to pass
        //some generic function that returns a value, I would have to be generic with Func<T> 
        public static async void CallMethodEveryNSeconds(Action instanceMethod, int seconds) {
            Console.WriteLine("Attempting call");
            while (true) {
                if ( cts.Token.IsCancellationRequested ) {
                    Console.WriteLine("Token was cancelled so not calling");
                    cts.Token.ThrowIfCancellationRequested();
                } else {
                    instanceMethod();
                }
                await Task.Delay(seconds*1000);
            }            
        }


        static async Task Main(string[] args)
        {
            Foody f = new Foody(HandleThresholdReached, HandleConditionFailed);
            f.Eat(1);

            CallMethodEveryNSeconds( () => f.Eat(1), 3);


            //This allows me to wait on the event
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            cts.Token.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), taskCompletionSource);
            await taskCompletionSource.Task;
        }
    }
}
