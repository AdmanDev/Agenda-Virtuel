using System;
using System.Collections.Generic;
using MyFunctions;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to save all user data.
    /// </summary>
    [Serializable]
    public class Save
    {
        //const variables
        /// <summary>
        /// Path of save data file
        /// </summary>
        [NonSerialized] private static string saveFile = System.Windows.Forms.Application.StartupPath + @"\save.bin";

        //Variables
        /// <summary>
        /// This is the list of user homeworks.
        /// </summary>
        public List<Homework> homeworks;
        /// <summary>
        /// This is user settings.
        /// </summary>
        public Settings settings;
        /// <summary>
        /// This is user school grades.
        /// </summary>
        public SchoolGrades schoolGrades;
        /// <summary>
        /// This is the list of user reminders.
        /// </summary>
        public List<string> reminders;
        /// <summary>
        /// This is the list of user appointments.
        /// </summary>
        public List<Appointment> scheduleAppointments;

        /// <summary>
        /// Plugins settings
        /// </summary>
        internal Dictionary<string, Dictionary<string, object>> pluginsParams; //Plugins settings

        //Properties
        /// <summary>
        /// Get the path of user data.
        /// </summary>
        public static string SaveFilePath { get => saveFile; }

        //Constructors
        /// <summary>
        /// Instantiate new save object
        /// </summary>
        public Save()
        {
            homeworks = new List<Homework>();
            settings = new Settings();
            schoolGrades = new SchoolGrades();
            reminders = new List<string>();
            scheduleAppointments = new List<Appointment>();

            pluginsParams = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>
        /// Save user data.
        /// </summary>
        /// <param name="path">Output file.</param>
        public void SaveFile(string path)
        {
            if (settings?.colors != null)
                settings.colors.SerializeXaml();

            FileManager.Serialize(path, this);
        }

        /// <summary>
        /// Save user data.
        /// </summary>
        public static void SaveData()
        {
            Global.userData.SaveFile(saveFile);
        }

        /// <summary>
        /// Open user data.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns><c>Save</c> objet that contains user data.</returns>
        public static void LoadFile(string path)
        {
            Save s = FileManager.Deserialize<Save>(path);
            Global.userData = s;

            s.settings.colors = new ColorsSettings();
            s.settings.colors.DeserializeXaml(s.settings);
        }

        /// <summary>
        /// Open user data.
        /// </summary>
        public static void LoadFile()
        {
            LoadFile(saveFile);
        }

        /// <summary>
        /// Load user data from xml code
        /// </summary>
        /// <param name="xmlCode">Your xml code</param>
        public static void LoadFromXml(string xmlCode)
        {
            Global.userData = XmlSaveRead.Read(xmlCode);

            Global.userData.settings.colors = new ColorsSettings();
            Global.userData.settings.colors.DeserializeXaml(Global.userData.settings);
        }

        /// <summary>
        /// Load and Apply a save object
        /// </summary>
        /// <param name="save">Save to load.</param>
        public static void LoadSave(Save save)
        {
            Global.userData = save;

            save.settings.colors = new ColorsSettings();
            save.settings.colors.DeserializeXaml(save.settings);
        }

    }
}
