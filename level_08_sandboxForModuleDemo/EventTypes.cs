using System;
namespace level_8_sandboxForModuleDemo
{
    public class SimulatedEvent : EventArgs, IEventTime 
    {
        public string EventType { get; set; }
        public string Label { get; set; }
        public string Data { get; set; }
        public DateTime EventTime { get; set; }

    }
    
}