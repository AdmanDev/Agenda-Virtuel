namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This class allows to manage the user school grades.
    /// </summary>
    public static class SchoolGradesManager
    {
        /*___________________________________FUNCTIONS___________________________________*/

        /// <summary>
        /// Get school grades of user. Don't judge him :)
        /// </summary>
        /// <returns><c>SchoolGrades</c> of user.</returns>
        public static SchoolGrades GetUserSchoolGrades()
        {
            return Global.userData.schoolGrades;
        }

        /// <summary>
        /// Add a school grade.
        /// </summary>
        /// <param name="subject">The <c>Subject</c> of the school grade.</param>
        /// <param name="newGrade">The new school <c>Grade</c> to add.</param>
        /// <param name="save">If true, the school grade will be saved.</param>
        public static void AddGrade(Subject subject, Grade newGrade, bool save = true)
        {
            subject.grades.Add(newGrade);
            EventsManager.Call_SchoolGradeAdded(subject, newGrade);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Remove a school grade.
        /// </summary>
        /// <param name="subject">The <c>Subject</c> of the school grade.</param>
        /// <param name="gradeToDelete">The new school <c>Grade</c> to remove.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void RemoveGrade(Subject subject, Grade gradeToDelete, bool save = true)
        {
            subject.grades.Remove(gradeToDelete);
            EventsManager.Call_SchoolGradeRemoved(subject, gradeToDelete);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Add a school subject.
        /// </summary>
        /// <param name="subject">The <c>Subject</c> to add.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void AddSubject(Subject subject, bool save = true)
        {
            Global.userData.schoolGrades.Subjects.Add(subject);
            EventsManager.Call_SubjectAdded(subject);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Add a school subject.
        /// </summary>
        /// <param name="subjectName">The name of subject to add.</param>
        /// <param name="coeff">The coefficient of the subject.</param>
        /// <param name="save">If true, this change will be saved.</param>
        /// <returns><c>Subject</c> object.</returns>
        public static Subject AddSubject(string subjectName, float coeff, bool save = true)
        {
            Subject s = new Subject(subjectName, coeff);
            AddSubject(s, save);

            return s;
        }

        /// <summary>
        /// Remove a subject.
        /// </summary>
        /// <param name="subject">The <c>Subject</c> to remove.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void RemoveSubject(Subject subject, bool save = true)
        {
            Global.userData.schoolGrades.Subjects.Remove(subject);
            EventsManager.Call_SubjectRemoved(subject);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Start a new trimester and remove all school grades of all subjects.
        /// </summary>
        /// <param name="oldGrades"><c>SchoolGrades</c> of the last trimester</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void NewTrimester(SchoolGrades oldGrades, bool save = true)
        {
            Global.userData.schoolGrades.NewTrimester();
            EventsManager.Call_NewTrimesterEvent(oldGrades);

            if (save)
            {
                Save.SaveData();

            }
        }

    }
}
