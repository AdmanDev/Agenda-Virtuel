using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Agenda_Virtuel.Manager;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// Allows to create a plugin. The main class of you plugin must inherits from this class.
    /// </summary>
    [Serializable]
    public class Plugin
    {
        //Variables
        [NonSerialized] private bool isStarted = false;

        private Type hmEditorType;
        private Type hmViewerType;

        //Properties

        /// <summary>
        /// This is the loader of the plugin
        /// </summary>
        internal PluginLoader Loader { get; set; }

        /// <summary>
        /// Get the name of the plugin.
        /// </summary>
        public string Name
        {
            get
            {
                if (Loader == null)
                    return null;
                else
                    return Loader.name;
            }
        }

        /// <summary>
        /// Get the description of the plugin.
        /// </summary>
        public string Description
        {
            get
            {
                if (Loader == null)
                    return null;
                else
                    return Loader.description;
            }
        }

        /// <summary>
        /// Determine if the plugin is working.
        /// </summary>
        protected bool PluginIsStarted { get => isStarted; private set => isStarted = value; }

        /// <summary>
        /// Get or set the UserControl that will be displayed whenever user open the setting window of your plugin.
        /// </summary>
        public UserControl PluginSettingsWindow { get; set; } = null;

        /// <summary>
        /// Get or set the type of your own UserControl that allows to add a homework. This type must inherits from <c>IHomeworkEditor</c>
        /// </summary>
        public Type HomeworkEditorType
        {
            get => hmEditorType;

            set
            {
                if (value.GetInterface(nameof(IHomeworkEditor)) != null)
                    hmEditorType = value;
            }
        }

        /// <summary>
        /// Get or set the type of your own homeworks viewers container. This Type must inherits from <c>IHomeworkViewerContainer</c>
        /// </summary>
        public Type HomeworkViewerContainerType
        {
            get => hmViewerType;

            set
            {
                if (value.GetInterface(nameof(IHomeworkViewerContainer)) != null)
                {
                    hmViewerType = value;

                    if (Loader?.isHmsViewer == true)
                        HomeworkManager.SetViewerContainer((IHomeworkViewerContainer)value.GetConstructor(new Type[0]).Invoke(null));
                }
            }
        }

        /// <summary>
        /// Homework editor of the plugin
        /// </summary>
        internal IHomeworkEditor HomeworkEditor { get; set; }
        /// <summary>
        /// Homework viewes container of the plugin
        /// </summary>
        internal IHomeworkViewerContainer HomeworkViewer { get; set; }
        
        //________________________________EVENTS________________________________//

        /// <summary>
        /// Triggered when pugin is unloading
        /// </summary>
        protected event Action Unloading;


        //_______________________________FUNCTIONS_______________________________//

        /// <summary>
        /// Start the pugin
        /// </summary>
        internal void Start()
        {
            if (PluginIsStarted)
                return;

            PluginIsStarted = true;

            PluginManager.DisplayPluginMenus(Loader);

            EventsManager.Call_OnPluginStarted(Loader);
            OnStartup();

            LoadControls();
        }

        /// <summary>
        /// Load homework editor and homeworks viewer of this plugin
        /// </summary>
        private void LoadControls()
        {
            //Homeworks editor
            if (HomeworkEditorType != null)
            {
                HomeworkEditor = (IHomeworkEditor)HomeworkEditorType.GetConstructor(new Type[0]).Invoke(null);
                if (Loader.isHomeworksEditor)
                    HomeworkManager.HomeworkEditor = HomeworkEditor;
            }

            //Homeworks viewer
            if (HomeworkViewerContainerType != null)
            {
                HomeworkViewer = (IHomeworkViewerContainer)HomeworkViewerContainerType.GetConstructor(new Type[0]).Invoke(null);
                if (Loader.isHmsViewer)
                    HomeworkManager.ViewerContainer = HomeworkViewer;
            }
        }

        /// <summary>
        /// Stop your plugin and restart the application
        /// </summary>
        public void UnloadPlugin()
        {
            Unloading?.Invoke();
            Unloading = null;
            //System.Windows.Forms.MessageBox.Show(
            //    Unloading.GetInvocationList()[0].Method.DeclaringType.Assembly.FullName
            //    );

            if (Loader != null)
                Loader.Unload();
        }

        /// <summary>
        /// Executed on the plugin startup.
        /// </summary>
        protected virtual void OnStartup()
        {

        }

        ~Plugin()
        {

        }

        #region System functions

        /// <summary>
        /// Save all user data.
        /// </summary>
        public void SaveDatas()
        {
            Save.SaveData();
        }

        /// <summary>
        /// Save your own data. You can recover your data with the function "GetSetting".
        /// </summary>
        /// <param name="key">The key of your data.</param>
        /// <param name="value">Your data that will be saved.</param>
        public void SetSetting(string key, object value)
        {
            if (Global.userData.pluginsParams == null)
            {
                Global.userData.pluginsParams = new Dictionary<string, Dictionary<string, object>>();
            }

            if (!Global.userData.pluginsParams.ContainsKey(Name))
            {
                Global.userData.pluginsParams.Add(Name, new Dictionary<string, object>());
            }

            if (Global.userData.pluginsParams[Name].ContainsKey(key))
            {
                Global.userData.pluginsParams[Name][key] = value;
            }
            else
            {
                Global.userData.pluginsParams[Name].Add(key, value);
            }

            Save.SaveData();
        }

        /// <summary>
        /// Get a data saved with the function "SetSetting".
        /// </summary>
        /// <param name="key">The key of the value (data).</param>
        /// <returns>Your data or a default value if your data was not found.</returns>
        public TypeOfValue GetSetting<TypeOfValue>(string key)
        {
            if (Global.userData.pluginsParams == null)
            {
                Global.userData.pluginsParams = new Dictionary<string, Dictionary<string, object>>();
            }

            if (!Global.userData.pluginsParams.ContainsKey(Name))
            {
                if (typeof(TypeOfValue).IsValueType)
                {
                    return default(TypeOfValue);
                }
                else
                {
                    if (typeof(TypeOfValue).Name == "String")
                    {
                        return default(TypeOfValue);
                    }

                    return (TypeOfValue)typeof(TypeOfValue).GetConstructor(new Type[0]).Invoke(new object[0]);
                }
            }

            if (!Global.userData.pluginsParams[Name].ContainsKey(key))
            {
                if (typeof(TypeOfValue).IsValueType)
                    return default(TypeOfValue);
                else
                    return (TypeOfValue)typeof(TypeOfValue).GetConstructor(new Type[0]).Invoke(new object[0]);
            }

            return (TypeOfValue)Global.userData.pluginsParams[Name][key];
        }


        #endregion

        #region Plugins functions

        /// <summary>
        /// Set a new Control that will be displayed whenever user open the plugin settings window.
        /// </summary>
        /// <param name="settingsControl">Your UserControl</param>
        public void SetSettingWindow(UserControl settingsControl)
        {
            PluginSettingsWindow = settingsControl;
        }

        /// <summary>
        /// Display window with your plugin settings Control.
        /// </summary>
        public void DisplayPluginSettings()
        {
            if (PluginSettingsWindow == null)
                return;

            new PluginSettings_Displayer(this).ShowDialog();
        }

        #endregion


    }
}
