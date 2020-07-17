using System;
using System.Collections.Generic;
using System.Drawing;

namespace Agenda_Virtuel
{
    /// <summary>
    /// Allows to save subjects and their owns colors to display in the schedule
    /// </summary>
    [Serializable]
    public class Subjects
    {
        //Variables
        private string[] subjects;
        private Dictionary<string, Color> subjectsColors;

        //Properties
        /// <summary>
        /// Get the list of subjects available in the schedule
        /// </summary>
        public string[] TheSubjects { get => subjects; }

        //constructor
        /// <summary>
        /// Instanciate a new Subjects object with default subjects list
        /// </summary>
        public Subjects()
        {
            SetSubjects(Properties.Resources.DefaultSubject_FR.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Add subjects and generate colors for each one of them
        /// </summary>
        /// <param name="_subjects">Subjects to add</param>
        public void SetSubjects(string[] _subjects)
        {
            subjects = _subjects;
            subjectsColors = new Dictionary<string, Color>();

            Color[] rcolor = MyFunctions.ColorManager.GetRandomColors(subjects.Length);
            int i = 0;

            foreach (string s in subjects)
            {
                subjectsColors.Add(s, rcolor[i]);
                i++;
            }
        }

        /// <summary>
        /// Get color of an subject
        /// </summary>
        /// <param name="subject">The subject whose color will be returned</param>
        /// <returns>The Color of the subject</returns>
        public Color GetColorOf(string subject)
        {
            if (!subjectsColors.ContainsKey(subject))
                return Color.White;

            return subjectsColors[subject];
        }

        /// <summary>
        /// Set color of an subject
        /// </summary>
        /// <param name="subject">The subject whose color will be set</param>
        /// <param name="newColor">The new color of the subject</param>
        public void SetColorOf(string subject, Color newColor)
        {
            if (!subjectsColors.ContainsKey(subject))
                return;

            subjectsColors[subject] = newColor;
        }

        /// <summary>
        /// Add subject
        /// </summary>
        /// <param name="subject">Subject to add</param>
        /// <param name="color">The subject's color</param>
        public void Add(string subject, Color color)
        {
            if (subjectsColors.ContainsKey(subject))
                return;

            subjectsColors.Add(subject, color);

            List<string> tmp = new List<string>(TheSubjects);
            tmp.Add(subject);
            subjects = tmp.ToArray();
        }

        /// <summary>
        /// Remove all subjects
        /// </summary>
        public void Clear()
        {
            subjectsColors.Clear();
            subjects = new string[0];
        }


    }
}
