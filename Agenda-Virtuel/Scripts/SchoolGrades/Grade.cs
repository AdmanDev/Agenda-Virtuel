using Agenda_Virtuel.Manager;
using System;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to save informations about a school grade.
    /// </summary>
    [Serializable]
    public class Grade
    {
        //Variables
        
        /// <summary>
        /// Obtained grade
        /// </summary>
        private float grade = 0f;
        /// <summary>
        /// "The grade out of x" --> this variable is x.
        /// </summary>
        public float outOf = 20f;
        
        private float coeff = 1f;
        /// <summary>
        /// The date on which this school grade was added.
        /// </summary>
        public DateTime date;

        //Properties
        /// <summary>
        /// Return (grade / outOf) * coeff
        /// </summary>
        public float Point
        {
            get
            {
                return (grade / outOf) * coeff;
            }
        }

        /// <summary>
        /// The obtained school grade.
        /// </summary>
        public float TheGrade { get => grade; set => grade = value; }
        /// <summary>
        /// The coefficient of this school grade.
        /// </summary>
        public float Coeff { get => coeff; set => coeff = value; }

        //Constructors
        /// <summary>
        /// Instantiate a Grade
        /// </summary>
        public Grade()
        {
            date = HomeworkManager.todayDate;
        }

        /// <summary>
        /// Instantiate a Grade
        /// </summary>
        /// <param name="_grade">Grade obtained</param>
        /// <param name="_outOf">Out of what ?</param>
        /// <param name="_coeff">Coefficent of the grade</param>
        public Grade(float _grade, float _outOf, float _coeff)
        {
            grade = _grade;
            outOf = _outOf;
            coeff = _coeff;

            date = DateTime.Now;
        }

        /// <summary>
        /// Instantiate a Grade
        /// </summary>
        /// <param name="_grade">Grade obtained</param>
        /// <param name="_outOf">Out of what ?</param>
        /// <param name="_coeff">Coefficent of the grade</param>
        /// <param name="date">The date of receipt of the grade</param>
        public Grade(float _grade, float _outOf, float _coeff, DateTime date) : this(_grade, _outOf, _coeff)
        {
            this.date = date;
        }

        /// <summary>
        /// Get garde / outOf
        /// </summary>
        /// <returns>String value</returns>
        public override string ToString()
        {
            return grade + " / " + outOf;
        }
    }
}
