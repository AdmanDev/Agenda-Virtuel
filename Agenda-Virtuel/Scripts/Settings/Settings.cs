using System;
using System.Collections.Generic;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This classs allows to save user settings.
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Get defaults settings.
        /// </summary>
        [NonSerialized] public readonly static Settings defaultSettings = new Settings();

        //Variables
        /// <summary>
        /// Subjects list
        /// </summary>
        private Subjects subjects;
        private string[] shortcutWords;
        /// <summary>
        /// Colors settings
        /// </summary>
        [NonSerialized] public ColorsSettings colors;
        /// <summary>
        /// Styles of application
        /// </summary>
        private Styles styles;
        internal List<string> serialisableColors;
        /// <summary>
        /// Fonts settings
        /// </summary>
        public FontsSettings fonts;
        /// <summary>
        /// Settings of installed plugins
        /// </summary>
        private PluginSettings pluginSettings;

        //Properties
        /// <summary>
        /// Get or set subjects list in the string array.
        /// </summary>
        public string[] SubjectsStrings { get=>subjects.TheSubjects; set => subjects.SetSubjects(value); }
        /// <summary>
        /// Get or set subjects list.
        /// </summary>
        public Subjects Subjects { get => subjects; internal set => subjects = value; }
        /// <summary>
        /// Get or set shortcuts words.
        /// </summary>
        public string[] ShortcutWords { get => shortcutWords; set => shortcutWords = value; }
        /// <summary>
        /// Get or set application <c>Styles</c>.
        /// </summary>
        /// <remarks>you have to use <c>Styles</c>.ApplyStyle to... apply the style</remarks>
        public Styles Styles { get => styles; set => styles = value; }
        /// <summary>
        /// Get or Set settings of installed plugins
        /// </summary>
        internal PluginSettings PluginSettings { get => pluginSettings; set => pluginSettings = value; }

        //Constructor
        /// <summary>
        /// Instantiate default Settings object
        /// </summary>
        public Settings()
        {
            subjects = new Subjects();
            shortcutWords = GetDefaultShortcutWords();
            colors = ColorsSettings.GetDefaultColors();
            styles = new Styles();
            serialisableColors = new List<string>();
            fonts = new FontsSettings();
            pluginSettings = new PluginSettings();
        }

        /// <summary>
        /// Get default shortcuts words
        /// </summary>
        /// <returns>String array</returns>
        public string[] GetDefaultShortcutWords()
        {
            return Properties.Resources.DefaultShortcutWords_FR.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
