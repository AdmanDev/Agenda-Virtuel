using System;
using System.Collections.Generic;

namespace Agenda_Virtuel
{
    /// <summary>
    /// Construct a homework.
    /// </summary>
    [Serializable]
    public class Homework
    {
        //Variables
        /// <summary>
        /// Homework ID in the database (if user is logged)
        /// </summary>
        private int id;
        /// <summary>
        /// Date of this homework
        /// </summary>
        private DateTime? date;
        /// <summary>
        /// The subject of the homework.
        /// </summary>
        private string subject;
        /// <summary>
        /// What the student have to do.
        /// </summary>
        private string job;
        /// <summary>
        /// The student have to prepare an exam ?
        /// </summary>
        private bool isTest;

        //Properties
        /// <summary>
        /// Homework ID in the database (if user is logged)
        /// </summary>
        internal int ID { get => id; set => id = value; }
        /// <summary>
        /// Date of homework.
        /// </summary>
        public DateTime? Date { get => date; set => date = value; }

        /// <summary>
        /// The subject of the homework.
        /// </summary>
        public string Subject { get => subject; set => subject = value; }

        /// <summary>
        /// What the student have to do.
        /// </summary>
        public string Job { get => job; set => job = value; }

        /// <summary>
        /// The student have to prepare an exam ?
        /// </summary>
        public bool IsTest { get => isTest; set => isTest = value; }


        //Constructors
        /// <summary>
        /// Instantiate new homework
        /// </summary>
        public Homework()
        {
            ID = -1;
        }

        /// <summary>
        /// Instantiate new homework
        /// </summary>
        /// <param name="_Date">Date of homework</param>
        /// <param name="_Subject">The subject of homework</param>
        /// <param name="_Job">The content of homework</param>
        /// <param name="_IsTest">True if the student must prepare an exam</param>
        public Homework(DateTime? _Date, string _Subject, string _Job, bool _IsTest)
        {
            ID = -1;
            Date = _Date;
            Subject = _Subject;
            Job = _Job;
            IsTest = _IsTest;
        }

        /// <summary>
        /// Compare two homeworks.
        /// </summary>
        /// <param name="obj">Must be a Homework object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Homework hm = (Homework)obj;

            if (hm == null || !this.Date.HasValue || !hm.Date.HasValue)
                return false;

            return (hm.Subject == this.Subject && hm.Job == this.Job && hm.IsTest == this.IsTest && DateTime.Compare(hm.Date.Value, this.Date.Value) == 0);
        }
    }

}