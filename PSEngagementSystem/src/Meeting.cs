namespace PSEngagementSystem
{
    using System;

    public enum MeetingStatus { Requested, Confirmed, Cancelled, Completed }

    public class Meeting
    {
        public int ID { get; set; }
        public int StudentID { get; set; }
        public int SupervisorID { get; set; }
        public DateTime ScheduledTime { get; set; }
        public MeetingStatus Status { get; set; }
        public string RequestedBy { get; set; }
    }

}
