using System;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to save fonts.
    /// </summary>
    [Serializable]
    public class FontsSettings
    {
        //Variables
        private FontGroup normalHomeworksFont;
        private FontGroup testsFont;
        private FontGroup subjectsFont;

        //Properties
        /// <summary>
        /// Get or set the <c>FontGroup</c> of "normal" homeworks.
        /// </summary>
        public FontGroup NormalHomeworksFont { get => normalHomeworksFont; set => normalHomeworksFont = value; }
        /// <summary>
        /// Get or set the <c>FontGroup</c> of "exams".
        /// </summary>
        public FontGroup TestsFont { get => testsFont; set => testsFont = value; }
        /// <summary>
        /// Get or set the <c>FontGroup</c> of subjects.
        /// </summary>
        public FontGroup SubjectsFont { get => subjectsFont; set => subjectsFont = value; }

        //Constructor
        /// <summary>
        /// Instantiate a new FontsSettings object
        /// </summary>
        public FontsSettings()
        {
            normalHomeworksFont = new FontGroup();
            testsFont = new FontGroup();
            subjectsFont = new FontGroup();
        }

        /// <summary>
        /// Instantiate a new FontsSettings object
        /// </summary>
        /// <param name="_homeworksFont">Font of normal homeworks text</param>
        /// <param name="_testsFont">Font of exam text</param>
        /// <param name="_subjectsFont">Font of subjects texst</param>
        public FontsSettings(FontGroup _homeworksFont, FontGroup _testsFont, FontGroup _subjectsFont)
        {
            normalHomeworksFont = _homeworksFont;
            testsFont = _testsFont;
            subjectsFont = _subjectsFont;
        }
    }
}
