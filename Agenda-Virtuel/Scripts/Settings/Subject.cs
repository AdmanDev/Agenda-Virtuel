﻿using System;
using System.Collections.Generic;
using System.Drawing;
using MyFunctions;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to save informations about a subject as the name or coefficient.
    /// </summary>
    [Serializable]
    public class Subject
    {
        //Variables
        /// <summary>
        /// The name of the subject
        /// </summary>
        private string name = "Undefined";
        /// <summary>
        /// Coefficient of the subject
        /// </summary>
        private float subjectCoeff = 1;
        /// <summary>
        /// Color of subject. (used in schedule)
        /// </summary>
        private Color color;
        /// <summary>
        /// The grades list of the subject.
        /// </summary>
        public List<Grade> grades = new List<Grade>();
        /// <summary>
        /// The History of the subject.
        /// </summary>
        public History history = new History();

        //Properties
        /// <summary>
        /// The name of subject.
        /// </summary>
        /// <remarks>The default name is "undefined".</remarks>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// The coefficient of the subject.
        /// </summary>
        public float Coeff { get => subjectCoeff; set => subjectCoeff = value; }
        /// <summary>
        /// Color of subject. (used in schedule)
        /// </summary>
        public Color Color { get => color; set => color = value; }
        /// <summary>
        /// Get the school grades average of the subject.
        /// </summary>
        public float Average
        {
            get
            {
                float sum = 0f;
                float sumCoeff = 0f;

                //Sum of all grades anf their coeffs
                foreach (Grade currentGrade in grades)
                {
                    sum += currentGrade.Point;
                    sumCoeff += currentGrade.Coeff;
                }

                //Return average out of 20
                if (sum > 0 && sumCoeff > 0)
                    return (float)Math.Round((sum / sumCoeff) * 20, 2);

                return 0;
            }
        }

        /// <summary>
        /// Get the best school grade of the subject.
        /// </summary>
        public float BestGrade
        {
            get
            {
                float result = 0f;

                //For each all grades...
                foreach (Grade currentGrade in grades)
                {
                    if (result == 0)
                    {
                        result = (currentGrade.TheGrade / currentGrade.outOf) * 20;
                    }
                    else
                    {
                        float currentNote = (currentGrade.TheGrade / currentGrade.outOf) * 20;

                        if (currentNote > result)
                            result = currentNote;
                    }
                }

                return (float)Math.Round(result, 2);
            }
        }

        /// <summary>
        /// Get the worst school grade of this subject.
        /// </summary>
        public float WorstGrade
        {
            get
            {
                float result = 0f;

                //For each all grades...
                foreach (Grade currentGrade in grades)
                {
                    if (result == 0)
                    {
                        result = (currentGrade.TheGrade / currentGrade.outOf) * 20;
                    }
                    else
                    {
                        float currentNote = (currentGrade.TheGrade / currentGrade.outOf) * 20;

                        if (currentNote < result)
                            result = currentNote;
                    }
                }

                return (float)Math.Round(result, 2);
            }
        }

        /// <summary>
        /// Average of user in this subject
        /// </summary>
        public bool Moy { get => Average >= 10; }

        //Constructors
        /// <summary>
        /// Instantiate a new Subject object
        /// </summary>
        public Subject()
        {
        }

        /// <summary>
        /// Instantiate a new Subject object
        /// </summary>
        /// <param name="_subjectName">Name of subject</param>
        /// <param name="_subjectCoeff">Coefficient of subject</param>
        /// <param name="_color">Subject color</param>
        /// <param name="_grades">User school grades list in this subject</param>
        public Subject(string _subjectName, float _subjectCoeff, Color _color, List<Grade> _grades = null)
        {
            if (_color == null)
                _color = ColorManager.GenerateColor();

            if (_grades == null)
                _grades = new List<Grade>();

            name = _subjectName;
            subjectCoeff = _subjectCoeff;
            color = _color;
            grades = _grades;
        }

        /// <summary>
        /// Return name of the subject
        /// </summary>
        /// <returns>Sring value</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Get default subjects list
        /// </summary>
        /// <returns>List of defaults subjects</returns>
        public static List<Subject> GetDefaultSubjectsList()
        {
            List<Subject> subjects = new List<Subject>()
            {
                new Subject("Mathématiques", 1, ColorManager.FromHexa("#555558")),
                new Subject("Français", 1, ColorManager.FromHexa("#00CCFF")),
                new Subject("Anglais", 1, ColorManager.FromHexa("#C36D96")),
                new Subject("Espagnol", 1, ColorManager.FromHexa("#4692AD")),
                new Subject("Histoire-Geo", 1, ColorManager.FromHexa("#89006A")),
                new Subject("Art-Plastique", 1, ColorManager.FromHexa("#64547B")),
                new Subject("Technologie", 1, ColorManager.FromHexa("#7A4900")),
                new Subject("Physiques-Chimie", 1, ColorManager.FromHexa("#9131AF")),
                new Subject("S.V.T", 1, ColorManager.FromHexa("#7FA670")),
            };

            return subjects;
        }

    }
}
