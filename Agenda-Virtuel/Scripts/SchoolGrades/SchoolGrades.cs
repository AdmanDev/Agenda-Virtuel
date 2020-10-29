using System;
using System.Collections.Generic;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to save settings of the school grades system.
    /// </summary>
    [Serializable]
    public class SchoolGrades
    {
        //Options variables
        private bool showComments;
        private bool highlighResult;

        //Properties
        /// <summary>
        /// It is the average of the trimester.
        /// </summary>
        public Subject OverallAverage
        {
            get
            {
                List<Grade> grades = new List<Grade>();
                foreach (Subject cs in Subjects)
                {
                    if (cs.Average > 0)
                        grades.Add(new Grade(cs.Average, 20, cs.Coeff));
                }

                return new Subject("Moyenne générale", 1, System.Drawing.Color.Black, grades);
            }
        }

        /// <summary>
        /// Determine if a comment is displayed according subjects averages, in the school grade window.
        /// </summary>
        public bool ShowComments { get => showComments; set => showComments = value; }
        /// <summary>
        /// Determine if the array (in the school grade window) highlight subject according the average.
        /// </summary>
        public bool HighlighResult { get => highlighResult; set => highlighResult = value; }
        /// <summary>
        /// The list of user subject.
        /// </summary>
        public List<Subject> Subjects { get => Global.userData.settings.Subjects;  }

        //Constructors
        /// <summary>
        /// Instanciate new SchoolGrades object
        /// </summary>
        public SchoolGrades()
        {
            ShowComments = true;
            HighlighResult = true;
        }

        /// <summary>
        /// Delete all grades of all subjects.
        /// </summary>
        public void NewTrimester()
        {
            //Remove all subjects grades
            foreach (Subject s in Subjects)
            {
                s.grades = new List<Grade>();
                s.history = new History();
            }
        }
    }

}
