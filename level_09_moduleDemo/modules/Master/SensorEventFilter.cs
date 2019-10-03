using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Master
{

    //Enjoyable style
    static class SensorEventFilter {

        static double temperatureThreshold = 1500;

        //where normally I would say isEventType = eventType => x => eventType === x; but currying is so messy here
        public static Func<JObject, bool> isTemperatureEvent = jObj => jObj.GetValue("eventType").ToString() == "TEMPERATURE_EVENT";
        
        public static Func<JObject, bool> isDoorEvent = jObj => 
            jObj.GetValue("eventType").ToString() == "DOOR_EVENT";

        public static Func<JObject, bool> isAirConditionEvent = jObj => 
            jObj.GetValue("eventType").ToString() == "AIRCONDITION_EVENT";
        
        public static Func<JObject, bool> isAboveTemperatureThreshold = jObj => 
            float.Parse(jObj.GetValue("temperature").ToString()) > temperatureThreshold;

        public static Func<JObject, bool> isEvenTemperature = jObj => 
            float.Parse(jObj.GetValue("temperature").ToString()) % 2 == 0;

        public static Func<JObject, bool>[] temperatureFilters = { isAboveTemperatureThreshold, isEvenTemperature };

        public static Func<JObject, bool> isImportantTemperatureEvent = jObj => temperatureFilters.Aggregate(true, (truthy, fn) => fn(jObj) && truthy);

        public static Func<JObject, bool> isImportantDoorEvent = jObj => 
            jObj.GetValue("eventLabel").ToString() == "driver";

        public static bool IsImportantEvent( JObject eventData )
        {
            if ( isTemperatureEvent(eventData) )
                return isImportantTemperatureEvent(eventData);
            if ( isDoorEvent(eventData) )
                return isImportantDoorEvent(eventData);
            if ( isAirConditionEvent(eventData) )
                return true; //just take all air condition events
            
            throw new InvalidOperationException("Event not recognized");
        }
    
    }

}