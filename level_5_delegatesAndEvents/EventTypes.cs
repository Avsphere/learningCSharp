using System;


namespace level_5_delegatesAndEvents {
    public class ThresholdReachedEventArgs : EventArgs {
        public int Size { get; set; }
        public DateTime TimeReached { get; set; }
        public ThresholdReachedEventArgs(int s, DateTime d) {
            Size = s;
            TimeReached = d;
        }
    }

    public class ConditionFailedEventArgs : EventArgs {
        public DateTime TimeFailed { get; set; }
        public ConditionFailedEventArgs(DateTime d) {
            TimeFailed = d;
        }
    }

}