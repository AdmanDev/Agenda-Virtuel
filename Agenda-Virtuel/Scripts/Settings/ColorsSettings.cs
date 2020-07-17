using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows.Media;

namespace Agenda_Virtuel
{ 
    /// <summary>
    /// This class allows to save colors.
    /// </summary>
    [Serializable]
    public class ColorsSettings
    {
        //Static variable
        /// <summary>
        /// Default colors settings
        /// </summary>
        public static ColorsSettings DefaultsColors { get; } = GetDefaultColors();

        //Variables
        private string backgroundColor;   

        private string normalHomeworksColor;
        private string testsColor;
        private string subjectsColor;
        private string highlightColor;

        //Properties
        /// <summary>
        /// Get or set windows background.
        /// </summary>
        /// <remarks>Use <c>SettingsManager</c>.SetWindowsBackground method to apply this background.</remarks>
        public Brush BackgroundColor { get; set; }
        /// <summary>
        /// Get or set the color of "normal" homeworks.
        /// </summary>
        public Brush NormalHomeworksColor { get; set; }
        /// <summary>
        /// Get or set the color of "exams".
        /// </summary>
        public Brush TestsColor { get; set; }
        /// <summary>
        /// Get or set the color of subjects.
        /// </summary>
        public Brush SubjectsColor { get; set; }
        /// <summary>
        /// Get or set the color used to highlight homeworks.
        /// </summary>
        public Brush HighlightColor { get; set; }

        //Constructors
        /// <summary>
        /// Instantiate a new ColorsSettings object
        /// </summary>
        /// <param name="_backgroundColor">Background color</param>
        /// <param name="_normalHomeworksColor">Color of normal homework text</param>
        /// <param name="_testsColor">Color of exam text</param>
        /// <param name="_subjectsColor">Color of subject text</param>
        /// <param name="_highlightColor">Highlight background color</param>
        public ColorsSettings(Color _backgroundColor, Color _normalHomeworksColor, Color _testsColor, Color _subjectsColor, Color _highlightColor)
        {
            BackgroundColor = new SolidColorBrush(_backgroundColor);
            NormalHomeworksColor = new SolidColorBrush(_normalHomeworksColor);
            TestsColor = new SolidColorBrush(_testsColor);
            SubjectsColor = new SolidColorBrush(_subjectsColor);
            HighlightColor = new SolidColorBrush(_highlightColor);
        }

        /// <summary>
        /// Instantiate a new ColorsSettings object
        /// </summary>
        public ColorsSettings()
        {

        }

        /// <summary>
        /// Get defaults colors.
        /// </summary>
        /// <returns>ColorsSettings object.</returns>
        public static ColorsSettings GetDefaultColors()
        {
            return new ColorsSettings
                (
                    Color.FromRgb(135, 10, 10), // Background of windows
                    Color.FromRgb(255, 255, 255), // Normal homeworks
                    Color.FromRgb(0, 255, 255), // Tests
                    Color.FromRgb(255, 255, 255), // Subjects 
                    Color.FromRgb(0, 255, 0) // Highlight
                );
        }

        /// <summary>
        /// Serialize colors to xaml values.
        /// </summary>
        /// <remarks>You have to use <c>Save</c>.SaveData to really save colors.</remarks>
        public void SerializeXaml()
        {
            if (BackgroundColor != null)
                backgroundColor = XamlWriter.Save(BackgroundColor);

            if (NormalHomeworksColor != null)
                normalHomeworksColor = XamlWriter.Save(NormalHomeworksColor);

            if (TestsColor != null)
                testsColor = XamlWriter.Save(TestsColor);

            if (SubjectsColor != null)
                subjectsColor = XamlWriter.Save(SubjectsColor);

            if (HighlightColor != null)
                highlightColor = XamlWriter.Save(HighlightColor);

            if(Global.userData != null)
                Global.userData.settings.serialisableColors = new List<string>()
                {
                    backgroundColor,
                    normalHomeworksColor,
                    testsColor,
                    subjectsColor,
                    highlightColor
                };

        }

        /// <summary>
        /// Deserialize colors.
        /// </summary>
        /// <param name="s"><c>Settings</c> object that contains serialized colors.</param>
        public void DeserializeXaml(Settings s)
        {
            if (s == null)
                return;

            List<string> cs = s.serialisableColors;
            if (cs == null || cs.Count <= 0)
            {
                s.colors = GetDefaultColors();
                return;
            }

            if (cs?[0] != null)
                BackgroundColor = (Brush)XamlReader.Parse(cs[0]);
            else
                BackgroundColor = DefaultsColors.BackgroundColor;

            if (cs?[1] != null)
                NormalHomeworksColor = (Brush)XamlReader.Parse(cs[1]);
            else
                NormalHomeworksColor = DefaultsColors.NormalHomeworksColor;

            if (cs?[2] != null)
                TestsColor = (Brush)XamlReader.Parse(cs[2]);
            else
                TestsColor = DefaultsColors.TestsColor;

            if (cs?[3] != null)
                SubjectsColor = (Brush)XamlReader.Parse(cs[3]);
            else
                SubjectsColor = DefaultsColors.SubjectsColor;

            if (cs?[4] != null)
                HighlightColor = (Brush)XamlReader.Parse(cs[4]);
            else
                HighlightColor = DefaultsColors.HighlightColor;

        }

    }
}
