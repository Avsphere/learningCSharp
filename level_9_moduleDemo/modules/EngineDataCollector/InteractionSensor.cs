using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EngineDataCollector
{


    //This sensor uses a dummy event raiser to populate it, rather than a constant read
    class InteractionSensor : ICanSense 
    {
        const string label = "interactionSensor";
        static int storeIndex = 0;
        const int maximumDataStoreSize = 10;
        ConcurrentDictionary<int, string> dataStore; //dict of 100 pairs
        bool sensing = false;
        Random random = new Random();


        public InteractionSensor(){
            int processorCount = Environment.ProcessorCount;
            int concurrencyLevel = processorCount * 2; //https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2?view=netframework-4.8
            dataStore = new ConcurrentDictionary<int, string>(processorCount, concurrencyLevel);
        }

        /* while I could collapse into a generic handler -- going to leave for demo */
        public void HandleDoorEvent (object sender, SimulatedEvent eventArgs) 
        {
            JObject json = JObject.FromObject( new {
                eventType = eventArgs.EventType,
                eventLabel = eventArgs.Label,
                data = eventArgs.Data,
                eventTime = eventArgs.EventTime
            });
            //I think even though it is concurrent I still need to increase the index atomically
            int index = Interlocked.Increment(ref storeIndex) % maximumDataStoreSize;
            dataStore[index] = json.ToString(Newtonsoft.Json.Formatting.None);
        }

        public void HandleAirConditionEvent (object sender, SimulatedEvent eventArgs) 
        {
            JObject json = JObject.FromObject( new {
                eventType = eventArgs.EventType,
                eventLabel = eventArgs.Label,
                data = eventArgs.Data,
                eventTime = eventArgs.EventTime
            });
            //I think even though it is concurrent I still need to increase the index atomically
            int index = Interlocked.Increment(ref storeIndex) % maximumDataStoreSize;
            dataStore[index] = json.ToString(Newtonsoft.Json.Formatting.None);
        }


        public void Start(int delayMs=200)
        {
            Console.WriteLine("Interaction sensor delay ms " + delayMs);
            EventSimulator.OnDoorEvent += HandleDoorEvent;
            EventSimulator.OnAirConditionEvent += HandleAirConditionEvent;
        }
        public void Finish()
        {
            EventSimulator.OnDoorEvent -= HandleDoorEvent;
            EventSimulator.OnAirConditionEvent -= HandleAirConditionEvent;
        }

        public string DataToJSONString()
        {
            JObject json = JObject.FromObject( new {
                data = dataStore.Values.ToArray()
            });

            return json.ToString(Newtonsoft.Json.Formatting.None);
        }

    }

}