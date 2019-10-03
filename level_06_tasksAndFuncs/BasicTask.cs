using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace level_6_tasksAndFuncs
{
    class MetaBox {
        public long CreationTime { get; set; }
        public int Name { get; set; }
        public int ThreadNum { get; set; }
    }

    public static class TaskExample {
        public static Random random = new Random();

        //alows the passing of a variable rather than the wrapping of a method
        public static Action<object> workAction = (object obj) => {
            string taskName = obj as string;
            if ( taskName == null ) { taskName = "iWasNull"; }
            Task.Delay(random.Next(1000, 5000)).Wait(); //blocking action
            Console.WriteLine("name : {0}, ThreadId : {1} ... beep boop", taskName, Thread.CurrentThread.ManagedThreadId);
        };

        public static Action basicWorkAction = () => {
            Task.Delay(random.Next(1000, 5000)).Wait(); //blocking action
            Console.WriteLine("doingBasicWork, ThreadId : {0} ... beep boop", Thread.CurrentThread.ManagedThreadId);
        };

        public static async Task taskWork(string taskWorkName) {
            await Task.Delay(1000);
            Console.WriteLine("taskWorkName : {0}, ThreadId : {1} ... beep boop", taskWorkName, Thread.CurrentThread.ManagedThreadId);
        }

        public static async Task CancelIfNeeded(CancellationTokenSource cts, string taskWorkName) {
            CancellationToken ct = cts.Token;
            await Task.Delay(1000);
            if ( random.Next(0, 10) % 2 == 0 ) {
                Console.WriteLine("taskWorkName : {0}, ThreadId : {1} ... CANCELLING", taskWorkName, Thread.CurrentThread.ManagedThreadId);
                cts.Cancel();
            }

            if ( ct.IsCancellationRequested ) {
                Console.WriteLine("Cancellation was requested");
                ct.ThrowIfCancellationRequested();
            } else {
                Console.WriteLine("taskWorkName : {0}, ThreadId : {1} ... beep boop", taskWorkName, Thread.CurrentThread.ManagedThreadId);
            }
        }
        
        //QUESTION : I am a little confused why this does not work... I just want an anon to return a task... const fetchAndParse = url => fetch(url).then(parse) where fetch and parse are both promises meaning it returns another promise
        static Func<Task> createWork = () => new Task(workAction, "...");
        
        public static async Task ManyTasksOnDifferentThreads() {
            Task[] taskArray = new Task[10];

            //The recommended way to launch a task is with Task.Run
            //Start new is only for when you want fine grained control over long running tasks
            //here I create 10 tasks, passing in my void lambda action which accepts an Object ref as a parameter --- not sure why exactly
            //it then casts? this to my Metabox and operates upon it
            for( int i = 0; i < taskArray.Length; i++ ) {
                taskArray[i] = Task.Factory.StartNew( (Object obj) => {
                    MetaBox metaBox = obj as MetaBox;
                    metaBox.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Starting task {0}", i);
                }, new MetaBox() { Name = i, CreationTime = DateTime.Now.Ticks });
            }
            await Task.WhenAll(taskArray);
            foreach( var task in taskArray ) {
                var data = task.AsyncState as MetaBox;
                Console.WriteLine("Task {0} created at {1}, ran on thread {2}", data.Name, data.CreationTime, data.ThreadNum);
            }
        }

        public static Task RunWithAggregation() {
            Console.WriteLine("Application is running on thread {0}", Thread.CurrentThread.ManagedThreadId);
            Task basicTaskAction = Task.Run(basicWorkAction); //low customization
            Task usingTaskRun = Task.Run( async() => {
                Console.WriteLine("Task thread ID : {0}, starting some basic work", Thread.CurrentThread.ManagedThreadId);
                await Task.Run(basicWorkAction);
            });
            Task doubleTaskReturn = taskWork("doubleTaskReturn");

            Task aggregatedTask = Task.WhenAll(new Task[] { basicTaskAction, usingTaskRun, doubleTaskReturn });
            return aggregatedTask;
        }

        public static async Task Basics() {
            //IT seems this might be useful when you are initializing many tasks at once, but not neccesarily wanting to run them right away
            Task taskA = new Task( () => Console.WriteLine("taskA has finished") );
            Console.WriteLine("taskA status : " + taskA.Status);
            taskA.Start();

            Console.WriteLine("taskA status : " + taskA.Status); //at this point it is waiting to run but will run in just a few ticks
            await(taskA);
            Console.WriteLine("taskA status : " + taskA.Status);
        }

        public static async Task AwaitingAPromiseAll() {
            Task[] tasks = { new Task(workAction, "a"), new Task(workAction, "b") };

            Console.WriteLine("tasks status {0}", string.Join(" , ", tasks.Select( t => t.Status).ToArray() ) );

            foreach( Task t in tasks ) {
                t.Start();
            }

            await Task.WhenAll(tasks);
        }

        public static Task ReturningAPromiseAll() {
            Task[] tasks = { new Task(workAction, "c"), new Task(workAction, "d") };
            
            foreach( Task t in tasks ) {
                t.Start();
            }

            return Task.WhenAll(tasks);
        }

        public static Task returnUnStartedTask() { return new Task( () => Console.WriteLine("Task created with new ") ); }
        public static async Task TaskBatchesDemo() {
            int[] workIds = { 1, 2, 3, 4, 5 };
            Task[] coupleOfTasks = new Task[] { taskWork("id_" + workIds[0]), taskWork("id_" + workIds[1]) }; 

            Task basicBatch = Task.WhenAll(coupleOfTasks);
            await basicBatch;
                

                    


            Task[] selectCreatingTasks = workIds.Select( id => taskWork("id_" + id) ).ToArray();
            await Task.WhenAll(selectCreatingTasks);

            Task[] manuallyCreatedTasks = new Task[] { 
                Task.Run( () => Console.WriteLine("Task1 created with run") ),
                Task.Run( () => Console.WriteLine("Task2 created with run") ),
            };
            await Task.WhenAll(manuallyCreatedTasks);

            Task[] batchOne = workIds.Select( id => taskWork("id_" + id) ).ToArray();
            Task[] batchTwo = workIds.Select( id => taskWork("id_" + id) ).ToArray();

            Task[] aggregatedBatches = batchOne.Concat(batchTwo).ToArray();
            await Task.WhenAll(aggregatedBatches);



        }

        public static async Task CancellationTokens() {
            //The primary responsibility of which is to propagate notfications that an operation should be cancelled, the paul revere
            CancellationTokenSource cts = new CancellationTokenSource();
            int[] workIds = { 1, 2, 3, 4, 5 };

            try {
                Task[] selectCreatingTasks = workIds.Select( id => CancelIfNeeded(cts, "id_" + id) ).ToArray();
                await Task.WhenAll(selectCreatingTasks);
            } catch (OperationCanceledException) {
                Console.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
            }


        } 


        public static async Task Demo() {

            // await RunWithAggregation();
            // await AwaitingAPromiseAll();
            // await ReturningAPromiseAll();

            // await TaskBatchesDemo();
            // await ManyTasksOnDifferentThreads();

            await CancellationTokens();

            


        }
    }




}
