using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This class allows to manage the user settings.
    /// </summary>
    public static class SettingsManager
    {
        /*______________________________________VARIABLES______________________________________*/


        /*______________________________________FUNCTIONS______________________________________*/

        /// <summary>
        /// Get the user settings.
        /// </summary>
        /// <returns><c>Settings</c> object.</returns>
        public static Settings GetSettings()
        {
            return Global.userData.settings;
        }

        /// <summary>
        /// Set applications windows background.
        /// </summary>
        /// <param name="brush">The new background.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetWindowsBackground(Brush brush, bool save = true)
        {
            Global.userData.settings.Styles.WinBackgroundColor = brush;
            Global.userData.settings.Styles.ApplyStyle(false);
            Global.userData.settings.colors.BackgroundColor = brush;

            EventsManager.Call_WinBackgroundChanged(brush);
           

            if (save)
            {
                Save.SaveData();
            }
        }

        #region Colors

        /// <summary>
        /// Set color of homeworks text.
        /// </summary>
        /// <param name="target">The target of this change. <see cref="ColorTarget"/>.</param>
        /// <param name="brush">The new value (color or brush).</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetColor(ColorTarget target, Brush brush, bool save = true)
        {
            switch (target)
            {
                case ColorTarget.NormalHomeworks:
                    Global.userData.settings.colors.NormalHomeworksColor = brush;
                    break;

                case ColorTarget.Tests:
                    Global.userData.settings.colors.TestsColor = brush;
                    break;

                case ColorTarget.subjects:
                    Global.userData.settings.colors.SubjectsColor = brush;
                    break;

                case ColorTarget.HighlightedHomeworks:
                    Global.userData.settings.colors.HighlightColor = brush;
                    break;

                default:
                    throw new Exception("ColorTarget value \"" + target + "\" isn't known (SettingsManager.SetColor)");

            }

            HomeworkManager.RecolorViewers();

            EventsManager.Call_ColorChanged(target, brush);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Set colors settings.
        /// </summary>
        /// <param name="colorsSettings">The new colors settings.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetColors(ColorsSettings colorsSettings, bool save = true)
        {
            Global.userData.settings.colors = colorsSettings;

            EventsManager.Call_ColorsSettingsChanged(colorsSettings);
            EventsManager.Call_ColorChanged(ColorTarget.NormalHomeworks, colorsSettings.NormalHomeworksColor);
            EventsManager.Call_ColorChanged(ColorTarget.Tests, colorsSettings.TestsColor);
            EventsManager.Call_ColorChanged(ColorTarget.subjects, colorsSettings.SubjectsColor);
            EventsManager.Call_ColorChanged(ColorTarget.HighlightedHomeworks, colorsSettings.HighlightColor);
            EventsManager.Call_WinBackgroundChanged(colorsSettings.BackgroundColor);

            HomeworkManager.RecolorViewers();

            if (save)
            {
                Save.SaveData();

            }
        }

        #endregion

        #region Fonts

        /// <summary>
        /// Set font of homework text.
        /// </summary>
        /// <param name="target">The target of this change. <see cref="FontTarget"/>.</param>
        /// <param name="font">The new font. <see cref="FontGroup"/>.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetFont(FontTarget target, FontGroup font, bool save = true)
        {
            if (font == null)
                return;

            switch (target)
            {
                case FontTarget.NormalHomeworks:
                    Global.userData.settings.fonts.NormalHomeworksFont = font;
                    break;

                case FontTarget.Tests:
                    Global.userData.settings.fonts.TestsFont = font;
                    break;

                case FontTarget.subjects:
                    Global.userData.settings.fonts.SubjectsFont = font;
                    break;

                default:
                    throw new Exception("FontTarget value \"" + target + "\" isn't known (SettingsManager.SetFont)");
            }

            HomeworkManager.RefontViewers();
            EventsManager.Call_FontChanged(target, font);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Set fonts settings.
        /// </summary>
        /// <param name="fontsSettings">The new <c>FontsSettings</c>.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetFonts(FontsSettings fontsSettings, bool save = true)
        {
            Global.userData.settings.fonts = fontsSettings;

            EventsManager.Call_FontsSettingsChanged(fontsSettings);
            EventsManager.Call_FontChanged(FontTarget.NormalHomeworks, fontsSettings.NormalHomeworksFont);
            EventsManager.Call_FontChanged(FontTarget.Tests, fontsSettings.TestsFont);
            EventsManager.Call_FontChanged(FontTarget.subjects, fontsSettings.SubjectsFont);

            HomeworkManager.RefontViewers();

            if (save)
            {
                Save.SaveData();

            }
        }

        #endregion

        #region Subjects and shortcut words

        /// <summary>
        /// Add a school subject.
        /// </summary>
        /// <param name="subject">The <c>Subject</c> to add.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void AddSubject(Subject subject, bool save = true)
        {
            Global.userData.settings.Subjects.Add(subject);
            EventsManager.Call_SubjectAdded(subject);
            EventsManager.Call_SubjectsListChanged(Global.userData.settings.Subjects);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Remove a subject.
        /// </summary>
        /// <param name="subject">The <c>Subject</c> to remove.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void DeleteSubject(Subject subject, bool save = true)
        {
            Global.userData.settings.Subjects.Remove(subject);
            EventsManager.Call_SubjectDeleted(subject);
            EventsManager.Call_SubjectsListChanged(Global.userData.settings.Subjects);

            if (save)
            {
                Save.SaveData();
            }
        }

        /// <summary>
        /// Set new list of subjects.
        /// </summary>
        /// <param name="newSubjectsList">New subjects list of user.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetSubjectList(List<Subject> newSubjectsList, bool save = true)
        {
            Global.userData.settings.Subjects = newSubjectsList;
            EventsManager.Call_SubjectsListChanged(newSubjectsList);

            if (save)
            {
                Save.SaveData();
            }
        }

        /// <summary>
        /// Set the shortcut words list.
        /// </summary>
        /// <param name="words">The new shortcut words list.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetShortcutWords(string[] words, bool save = true)
        {
            Global.userData.settings.ShortcutWords = words;
            EventsManager.Call_ShortcutWordsChanged(words);

            if (save)
            {
                Save.SaveData();

            }
        }

        #endregion
    }
}
