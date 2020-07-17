using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// This class allows to load a plugin
    /// </summary>
    [Serializable]
    internal class PluginLoader
    {
        //Variables
        private string dllPath, dllName, pluginClass;
        public string version;
        public string[] secDlls;
        public string name = "My super plugin";
        public string description;
        private bool enabled = false;

        internal bool isHomeworksEditor;
        internal bool isHmsViewer;

        [NonSerialized] public Assembly assembly;
        [NonSerialized] private bool isloaded = false;
        [NonSerialized] private string error;
        [NonSerialized] private Plugin agendaPlugin;
        [NonSerialized] public AppDomain domain;

        //Properties
        /// <summary>
        /// The plugin to load
        /// </summary>
        public Plugin AgPlugin { get => agendaPlugin; private set => agendaPlugin = value; }
        /// <summary>
        /// Determine if the plugin is loaded
        /// </summary>
        public bool Isloaded { get => isloaded; private set => isloaded = value; }
        /// <summary>
        /// Determine if the plugin is enabled
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;

                if (value)
                {
                    Load();
                }
                else
                {
                    Unload(true);
                }

            }
        }

        /// <summary>
        /// The path of the main plugin dll
        /// </summary>
        public string DllPath { get => dllPath; set => dllPath = value; }
        /// <summary>
        /// The plugin name
        /// </summary>
        public string DllName { get => dllName; private set => dllName = value; }
        /// <summary>
        /// The name of the main plugin class that inherits from Plugin class
        /// </summary>
        public string PluginClass { get => pluginClass; set => pluginClass = value; }
        /// <summary>
        /// List of plugin main menu items (displayed when you click on "others" button)
        /// </summary>
        public List<PluginMenuItemAttribute> PluginMenus { get; private set; }
        /// <summary>
        /// List of homework viewer menu items (displayed when right click on a hoework
        /// </summary>
        public List<HomeworksMenuItemAttribute> HomeworksMenus { get; private set; }

        //Constructors
        /// <summary>
        /// Instantiate a new PluginLoader
        /// </summary>
        public PluginLoader()
        {

        }

        /// <summary>
        /// Instantiate a new PluginLoader
        /// </summary>
        /// <param name="_dllName">The path of the main plugin dll</param>
        /// <param name="_class">The name of the main plugin class that inherits from Plugin class</param>
        /// <param name="_pluginName">The plugin name</param>
        /// <param name="_version">The plugin version</param>
        public PluginLoader(string _dllName, string _class, string _pluginName, string _version)
        {
            dllName = _dllName;
            pluginClass = _class;
            name = _pluginName;
            version = _version;
        }

        /// <summary>
        /// Intilize all field with another loader
        /// </summary>
        /// <param name="loader">Loader do copy</param>
        public void Initialize(PluginLoader loader)
        {
            dllName = loader.dllName;
            pluginClass = loader.pluginClass;
            version = loader.version;
            name = loader.name;
            description = loader.description;
            secDlls = loader.secDlls;
            enabled = loader.enabled;

            isHomeworksEditor = loader.isHomeworksEditor;
            isHmsViewer = loader.isHmsViewer;

            domain = loader.domain;
        }

        /// <summary>
        /// Load plugin
        /// </summary>
        /// <returns>Plugin Loaded</returns>
        public Plugin Load()
        {
            if (HasError())
            {
                ShowError();
                return null;
            }

            AgPlugin = (Plugin)assembly.GetType(pluginClass).GetConstructor(new Type[0]).Invoke(null);
            AgPlugin.Loader = this;

            PluginMenus = FindMenus<PluginMenuAttribute, PluginMenuItemAttribute>();
            HomeworksMenus = FindMenus<HomeworksMenuAttribute, HomeworksMenuItemAttribute>();


            Isloaded = true;
            return AgPlugin;
        }

        /// <summary>
        /// Unload plugin
        /// </summary>
        /// <param name="restart">Restart application if true. (recommended)</param>
        public void Unload(bool restart = true)
        {
            Isloaded = false;

            if (domain != null)
            {
                AppDomain.Unload(domain);
                domain = null;
            }

            if (restart)
            {
                System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <param name="showMessage">If true, ask user if he want to remove plugin settings</param>
        /// <param name="removePluginParams">If true we delete plugin settings wihtout ask user</param>
        public void Unistall(bool showMessage = true, bool removePluginParams = true)
        {
            Unload(false);

            if (!PluginManager.Plugins.Contains(this))
                return;

            PluginManager.RemovePlugin(this);
            PluginManager.Save();

            if (Global.userData.pluginsParams.ContainsKey(name))
            {
                bool remove = removePluginParams;

                if (showMessage)
                    remove = MessageBox.Show("Voulez-vous supprimer les paramètres du plugin ?", name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                if (remove)
                {
                    Global.userData.pluginsParams.Remove(name);
                    Save.SaveData();
                }
            }

            Properties.Settings.Default.FilesToDelete.Add(DllPath);
            Properties.Settings.Default.FilesToDelete.AddRange(secDlls);
            Properties.Settings.Default.Save();

            System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
            System.Windows.Application.Current.Shutdown();
        }


        /// <summary>
        /// Get settings window
        /// </summary>
        /// <returns>UserControl object</returns>
        public System.Windows.Controls.UserControl GetSettingsWindow()
        {
            if (AgPlugin == null)
                return null;

            return AgPlugin.PluginSettingsWindow;
        }

        /// <summary>
        /// Check if there is any error in the plugin
        /// </summary>
        /// <param name="_dllPath"></param>
        /// <returns>True an error has been detected</returns>
        public bool HasError(string _dllPath = "")
        {
            if (_dllPath == "")
                dllPath = PluginManager.DllsDirectory + dllName;
            else
                dllPath = _dllPath;

            if (!File.Exists(dllPath))
            {
                error = "The following file was not found :" + Environment.NewLine + dllPath;
                return true;
            }

            if (!dllPath.Contains(".dll"))
            {
                error = "The following file is not a dll file :" + Environment.NewLine + dllPath;
            }

            if (assembly == null)
                assembly = Assembly.LoadFile(dllPath);

            List<Type> types = new List<Type>(assembly.GetTypes());
            Type agType = types.Find((x) => x.IsSubclassOf(typeof(Plugin)));

            if (agType == null)
            {
                error = "The following type was not found :" + Environment.NewLine + pluginClass;
                return true;
            }


            if (!agType.IsSubclassOf(typeof(Plugin)))
            {
                error = pluginClass + " is not sub class of " + nameof(Plugin);
                return true;
            }

            error = "";
            return false;
        }

        /// <summary>
        /// Show the error found by the "HasError" function
        /// </summary>
        public void ShowError()
        {
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Find menus of...
        /// </summary>
        /// <typeparam name="Menu">Type of menu to find</typeparam>
        /// <typeparam name="Item">Type of item of menu to find</typeparam>
        /// <returns></returns>
        private List<Item> FindMenus<Menu, Item>()
            where Menu : Attribute, new() where Item : Attribute, new()
        {
            if (assembly == null)
                return null;

            List<Item> itemsAttributes = new List<Item>();

            Menu menuAttribute;
            Item itemAttribute;

            foreach (Type t in assembly.GetTypes())
            {
                //Stop if t isn't static
                if (!t.IsAbstract && !t.IsSealed)
                    continue;

                menuAttribute = (Menu)t.GetCustomAttribute(typeof(Menu));
                if (menuAttribute != null)
                {
                    foreach (MethodInfo mi in t.GetMethods())
                    {
                        itemAttribute = (Item)mi.GetCustomAttribute(typeof(Item));

                        if (itemAttribute != null)
                        {
                            Type itype = typeof(Item);
                            itype.GetField("method").SetValue(itemAttribute, mi);

                            itemsAttributes.Add(itemAttribute);
                        }
                    }
                }
            }

            return itemsAttributes;
        }

        /// <summary>
        /// Get new instance of a T object
        /// </summary>
        /// <typeparam name="T">Type of object whose you want an instance</typeparam>
        /// <returns>T object</returns>
        private T GetInstanceOf<T>()
        {
            if (assembly == null)
                return default(T);

            foreach (Type t in assembly.GetTypes())
            {
                if (t.GetInterface(nameof(IHomeworkEditor)) != null)
                {
                    return (T)t.GetConstructor(new Type[0]).Invoke(null);
                }
            }

            return default(T);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PluginLoader))
                return false;

            PluginLoader other = (PluginLoader)obj;

            if (other == null)
                return false;

            if (Isloaded && other.Isloaded)
            {
                return DllPath == other.DllPath && name == other.name && PluginClass == other.PluginClass;
            }

            return name == other.name && PluginClass == other.PluginClass;
        }

    }
}
