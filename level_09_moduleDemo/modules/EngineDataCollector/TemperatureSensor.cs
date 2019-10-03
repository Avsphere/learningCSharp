using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EngineDataCollector
{
    class TemperatureSensor : ICanSense {

        const string label = "temperatureSensor";
        const string eventType = "TEMPERATURE_EVENT"; //to stay consistent with my interaction sensor
        bool sensing = false;
        int maximumDataStoreSize;
        string[] dataStore;
        Random random = new Random();

        public TemperatureSensor(int dataStorageSize=10) {
            maximumDataStoreSize = dataStorageSize;
            dataStore = new string[maximumDataStoreSize];
        }

        //is a void whilst using task here bad practice?
        public async void Start(int delayMs=100) 
        {
            sensing = true;
            int i = 0;

            while(sensing)
            {
                JObject json = JObject.FromObject( new {
                    eventTime = DateTime.Now,
                    eventType = eventType,
                    temperature = random.Next(1000, 2000)
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