using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace level_8_sandboxForModuleDemo
{


    //This sensor uses a dummy event raiser to populate it, rather than a constant read
    class InteractionSensor : ICanSense 
    {
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

    class TemperatureSensor : ICanSense {
        bool sensing = false;
        const int maximumDataStoreSize = 10;
        string[] dataStore = new string[maximumDataStoreSize];

        const string eventType = "TEMPERATURE_EVENT";
        Random random = new Random();

        public TemperatureSensor() {}

        //is a void whilst using task here bad practice?
        public async void Start(int delayMs=100) 
        {
            sensing = true;
            int i = 0;

            while(sensing)
            {
                JObject json = JObject.FromObject( new {
                    eventType = eventType,
                    eventTime = DateTime.Now,
                    temperature = random.Next(0, 5000)
                });
                dataStore[ i % maximumDataStoreSize ] = json.ToString(Newtonsoft.Json.Formatting.None);
                await Task.Delay(delayMs);
                i++;
            }

        }
        public void Finish() 
        {
            /*
                Do important sensor closing logic here
             */
           sensing = false; 
        }

        public string DataToJSONString() {
            JObject json = JObject.FromObject( new {
                data = dataStore
            });
            return json.ToString(Newtonsoft.Json.Formatting.None);
        }
    }     
}