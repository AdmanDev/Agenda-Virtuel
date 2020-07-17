using System;
using System.Collections.Generic;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class is used to create graphic.
    /// It allows to save history of a subject and dates with averages.
    /// </summary>
    [Serializable]
    public class History
    {
        //Variables
        /// <summary>
        /// Dates on which the subject average has been changed.
        /// </summary>
        public List<DateTime> dates;
        /// <summary>
        /// List of subject averages connected with the "dates" variable.
        /// </summary>
        public List<float> averages;

        /// <summary>
        /// Instanciate a new history
        /// </summary>
        public History()
        {
            dates = new List<DateTime>();
            averages = new List<float>();
        }

        /// <summary>
        /// Add a key in  the history.
        /// </summary>
        /// <param name="_date">Date on which average has been changed.</param>
        /// <param name="_value">Average value.</param>
        public void Add(DateTime _date, float _value)
        {
            if (dates == null)
                dates = new List<DateTime>();

            if (!dates.Contains(_date))
            {
                dates.Add(_date);
                averages.Add(_value);
            }
        }
    }
}
