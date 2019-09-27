using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


delegate string[] FilterStringList( List<string> stringList);

namespace level_8_sandboxForModuleDemo
{
    class Program
    {
        static int Main() => MainAsync().Result;

        static ICanSense[] sensors = { new InteractionSensor(), new TemperatureSensor() };

        static Random random = new Random();
        public static void StartSensors()
        {
            foreach( ICanSense sensor in sensors ) {
                sensor.Start(100);
            }

        }
        public static void StopSensors()
        {
            foreach( ICanSense sensor in sensors ) {
                sensor.Finish();
            }
        }

        public static async Task<List<string>> StartCollectionInterval(
            CancellationTokenSource cts, 
            int collectionDelayMs=500) 
        {   
            List<string> data = new List<string>();
            while (!cts.Token.IsCancellationRequested)
            {
                foreach( ICanSense sensor in sensors )
                {
                    // Console.WriteLine(sensor.GetType().ToString() );
                    // Console.WriteLine("json " + sensor.DataToJSONString() );
                    data.Add( sensor.DataToJSONString() );
                }

                await Task.Delay(collectionDelayMs);
            }
            // Console.WriteLine("RETURNING DATA" + string.Join(" ,", data.ToArray() ) );
            return data;
        }

        public static string[] OddFilter( List<string> stringList )
        {
            return stringList.Where( s => s.Length % 2 != 0 ).ToArray();
        }
        public static string[] EvenFilter( List<string> stringList )
        {
            return stringList.Where( s => s.Length % 2 == 0 ).ToArray();
        }


        public static async Task<int> MainAsync() 
        {
            Console.WriteLine("App thread {0}", Thread.CurrentThread.ManagedThreadId);
            CancellationTokenSource cts = new CancellationTokenSource();
            FilterStringList oddFilter = new FilterStringList(OddFilter);
            FilterStringList evenFilter = new FilterStringList(EvenFilter);
            FilterStringList[] filters = { oddFilter, evenFilter };
            EventSimulator.Start();
            StartSensors();

            Task<List<string>> collectData = StartCollectionInterval(cts);
            await Task.Delay(2000);
            cts.Cancel(); //propogate cancellation among async operations -- which causes return of that task
            await collectData; //now I do the actual awaiting
            List<string> collectionOfData = collectData.Result;
            // if ( collectionOfData.IsCompleted ) {
            //     Console.WriteLine("THIS IS THE RESULT" + string.Join(",", collectionOfData.Result) );
            // }



            string[] allResults = collectionOfData.ToArray();

            JObject[] jsonObjs = allResults.Select( s => JObject.Parse(s) ).ToArray();

            foreach( JObject o in jsonObjs ) {
                // Console.WriteLine(o.ToString());s
                // JToken data = o.GetValue("data");

                JArray eventArray = (JArray)o["data"];
                
                //Are these auto optimized? or do i needs a transducer
                List<JObject> importantEvents = eventArray
                    .Where( ev => ev.ToString().Length > 0 ) //make sure the jtoken was not a newline
                    .Select( ev => JObject.Parse(ev.ToString()) ) //JToken -> Jobject 
                    .Where(Filter.IsImportantEvent) //Filter out "non-important" events
                    .ToList();
                

                if ( importantEvents.Count > 0 ) {
                    Console.WriteLine( JsonConvert.SerializeObject(importantEvents) );
                }

                // bool test = Filter.IsImportantEvent(ev);
                // Console.WriteLine(ev + " " + test);
                Console.WriteLine("------------------------");

                
            }

            // Console.WriteLine( string.Join(",",allResults) + "\n\n" + jsonObjs[1].ToString());


            return 0;
        } 
    }
}
