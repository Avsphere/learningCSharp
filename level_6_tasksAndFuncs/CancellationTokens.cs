using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace level_6_tasksAndFuncs {

    //Here I am cancelling a task and its children
    public static class CancellationTokens {

        static Random random = new Random();
        static void DoWork(int taskNum, CancellationToken ct) {
            
            if ( ct.IsCancellationRequested ) {
                Console.WriteLine("Task {0} cancelled before started", taskNum);
                ct.ThrowIfCancellationRequested(); //break out of working
            }

            Task.Delay(random.Next(1000, 5000)).Wait();
            if ( ct.IsCancellationRequested ) {
                Console.WriteLine("Task {0} finished its waiting but then found that it was cancelled", taskNum);
                ct.ThrowIfCancellationRequested(); //break out of working
            }
        }

        static async Task<int> DoScaryWork( int taskNum, CancellationToken ct ) {
            bool scaryEnoughToDie = random.Next(0, 100) % 5 == 0;
            
            if ( ct.IsCancellationRequested ) {
                Console.WriteLine("Task {0} cancelled before started", taskNum);
                ct.ThrowIfCancellationRequested(); //break out of working
            } else if ( scaryEnoughToDie ) {
                return -1;
            }

            
            await Task.Delay(random.Next(0, 2000) ); //in this case the work was not that scary

            if ( ct.IsCancellationRequested ) {
                Console.WriteLine("Task {0} cancelled after it finished working", taskNum);
                ct.ThrowIfCancellationRequested(); //break out of working
            }

            Console.WriteLine("{0} finished scary work", taskNum);
            return 0; //in this case the work was successfull
        }

        static async Task WrapScaryWork( Func<int, CancellationToken, Task<int> > scaryWork, CancellationTokenSource cts) {
            CancellationTokenSource ctx = new CancellationTokenSource();
            int result = await(scaryWork(0, ctx.Token));

        }

        static void PrintTaskStatuses( Task[] tasks ) {
            foreach( Task t in tasks ) {
                Console.WriteLine("Task {0} status is now {1}", t.Id, t.Status);
            }
        }

        public static async Task Demo() {
            // await Basics();
            await ScaryTasks();
        }

        public static async Task ScaryTasks() {
            CancellationTokenSource cts = new CancellationTokenSource();
            int[] taskIds = { 1, 2, 3, 4, 5 };

            int result = await DoScaryWork(1, cts.Token);

            Console.WriteLine("result : {0}", result);

            

        }








        public static async Task Basics() {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task cancellableTask = Task.Run( () => DoWork(0, cts.Token) );

            await Task.Delay(10); //this causes it to notice it was cancelled after starting
            cts.Cancel();

            Task alreadyCancelledTask = Task.Run( () => DoWork(1, cts.Token) ); //this task will be cancelled before it has even started as it is using the same token


            try {
                await cancellableTask;
            } catch (OperationCanceledException) {
                Console.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
            } finally {
                PrintTaskStatuses( new Task[] { cancellableTask, alreadyCancelledTask });
            }

        }
    }
}