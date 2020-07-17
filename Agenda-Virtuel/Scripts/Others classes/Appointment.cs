using System;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to create appontment to the schedule.
    /// </summary>
    [Serializable]
    public class Appointment
    {
        //Variables
        private string title; //Title of the appointment
        private DateTime startTime; //Start time of the appointment
        private DateTime endTime; //End time of the appointment

        //Properties
        /// <summary>
        /// The time of appointment beginning.
        /// </summary>
        public DateTime StartTime
        {
            get => startTime;
            set => startTime = GenerateNormalizedTime(value.Day, value.Hour, value.Minute);
        }

        /// <summary>
        /// The end time of appointment.
        /// </summary>
        public DateTime EndTime
        {
            get => endTime;
            set => endTime = GenerateNormalizedTime(value.Day, value.Hour, value.Minute);
        }

        /// <summary>
        /// The title of the appointment (the subject).
        /// </summary>
        public string Title { get => title; set => title = value; }

        //Constructor

        /// <summary>
        /// Create a new appointment for the schedule.
        /// </summary>
        /// <param name="_title">The title of the appointment (the subject).</param>
        /// <param name="_startTime">The time of appointment beginning.</param>
        /// <param name="_endTime">The end time of appointment.</param>
        public Appointment(string _title, DateTime _startTime, DateTime _endTime)
        {
            Title = _title;
            StartTime = _startTime;
            EndTime = _endTime;
        }

        /// <summary>
        /// Generate normalized time for an appoinment.
        /// </summary>
        /// <param name="day">The day of the new appointment</param>
        /// <param name="hour">The start or end hour of the new appointment</param>
        /// <param name="minute">The start or end minutes of the new appointment</param>
        /// <returns>Normalized DateTime</returns>
        public static DateTime GenerateNormalizedTime(int day, int hour, int minute)
        {
            return new DateTime(2018, 10, day, hour, minute, 0);
        }
    }
}
