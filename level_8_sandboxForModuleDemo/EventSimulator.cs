using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace level_8_sandboxForModuleDemo
{
    public static class EventSimulator
        {
        public static event EventHandler<SimulatedEvent> OnDoorEvent = delegate {}; //trivial handler == no null check
        public static event EventHandler<SimulatedEvent> OnAirConditionEvent = delegate {}; 
        
        static int delayMs = 100;
        static Random random = new Random();
        static bool simulating = false;
        static string[] doorTypes =  new string[] { "driver", "passenger", "backLeft", "backRight" };

        static Action[] simulatedEvents = { RaiseDoorEvent, RaiseAirConditionEvent }; //lowercase?
        public static void RaiseDoorEvent()
        {
            OnDoorEvent(
                null,
                new SimulatedEvent {
                    EventType = "DOOR_EVENT",
                    Label = doorTypes[ random.Next(0, doorTypes.Length-1) ],
                    Data = random.Next(3) % 2 == 0 ? "closed" : "open",
                    EventTime = DateTime.Now
                }
            );
        }

        public static void RaiseAirConditionEvent()
        {
            OnAirConditionEvent(
                null,
                new SimulatedEvent {
                    EventType = "AIRCONDITION_EVENT",
                    Label = random.Next(3) % 2 == 0 ? "hotter" : "colder",
                    Data = random.Next(3) % 2 == 0 ? "1" : "-1",
                    EventTime = DateTime.Now
                }
            );
        }


        public static async Task Start()
        {
            simulating = true;
            while(simulating)
            {
                 //I know Invoke is overkill, but it feels more readable
                simulatedEvents[random.Next(simulatedEvents.Length)].Invoke();
                await Task.Delay(delayMs);
            }
        }

        public static void Stop() 
        {
            simulating = false;
        }

    }
}