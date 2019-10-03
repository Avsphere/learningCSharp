using System;


namespace level_7_tokensAndEvents
{
    interface IEventTime {
        DateTime EventTime { get; set; }
    }
    public class ThresholdReachedEventArgs : EventArgs, IEventTime {
        public int ThresholdReached { get; set; }
        public DateTime EventTime { get; set; }
    }

    public class ConditionFailedEventArgs : EventArgs, IEventTime {
        public string Reason { get; set; }
        public DateTime EventTime { get; set; }
    } 
}