using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Windows.Media;
using Agenda_Virtuel.Manager;
using MyFunctions;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// This static class manage plugin system
    /// </summary>
    internal static class PluginManager
    {
        //Variables
        /// <summary>
        /// Plugins dlls directory
        /// </summary>
        private static string dllDirectory = Application.StartupPath + @"\";
        /// <summary>
        /// Installed plugins infos file
        /// </summary>
        private static string pluginsSavePath = Application.StartupPath + @"\Plugins.bin";
        /// <summary>
        /// Loaded plugins list
        /// </summary>
        private static List<PluginLoader> plugins = new List<PluginLoader>();

        //Properties
        /// <summary>
        /// Dlls directory folder
        /// </summary>
        public static string DllsDirectory { get => dllDirectory; }
        /// <summary>
        /// Loaded plugins list
        /// </summary>
        public static PluginLoader[] Plugins { get => plugins.ToArray(); }
        /// <summary>
        /// Get installed plugins count
        /// </summary>
        public static int InstalledPluginCount { get => plugins.Count; }


        /// <summary>
        /// Save installed plugins infos
        /// </summary>
        public static void Save()
        {
            FileManager.Serialize(pluginsSavePath, plugins);
        }

        /// <summary>
        /// Open installed plugins
        /// </summary>
        public static void Open()
        {
            plugins = FileManager.Deserialize<List<PluginLoader>>(pluginsSavePath);
            StartAll();

            InstallUpdate();
        }

        /// <summary>
        /// Install updates of plugins if exists
        /// </summary>
        private static void InstallUpdate()
        {
            if (File.Exists(PluginStoreManager.localUpdateFile))
            {
                string downloadUrl = File.ReadAllText(PluginStoreManager.localUpdateFile);
                File.Delete(PluginStoreManager.localUpdateFile);

                string localzip = Application.StartupPath + @"\" + downloadUrl.SplitAndGetLastPart("/");

                WebClient web = new WebClient();
                web.DownloadFile(downloadUrl, localzip);

                if (File.Exists(localzip))
                {
                    PluginInstaller.InstallFromFile(localzip, out bool r, false);
                    if (r)
                        MessageBox.Show("Le plugin a été mis à jour avec succès !", "Agenda-Virtuel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Erreur : Le plugin n'a pas été mis à jour !", "Agenda-Virtuel", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

                if (File.Exists(localzip))
                    File.Delete(localzip);
            }
        }

        /// <summary>
        /// Start plugins
        /// </summary>
        public static void StartAll()
        {
            if (!Global.userData.settings.PluginSettings.enabled)
                return;

            PluginLoader pl;
            for (int i = 0; i < plugins.Count; i++)
            {
                if (plugins[i].Enabled)
                {
                    if(!plugins[i].Isloaded)
                    {
                        pl = plugins[i];
                        plugins[i] = LoadPlugin(ref pl);
                    }
                }
            }

           
        }

        /// <summary>
        /// Load a plugin on another app domain
        /// </summary>
        /// <param name="loader">Plugin to load</param>
        /// <returns>Return created AppDomain</returns>
        public static AppDomain LoaderToAppDomain(ref PluginLoader loader)
        {
            AppDomain domain = AppDomain.CreateDomain(loader.name);
            Type t = typeof(PluginLoader);
            PluginLoader ndLoader = (PluginLoader)domain.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName);
            ndLoader.Initialize(loader);
            ndLoader.domain = domain;

            loader = ndLoader;
            return domain;
        }

        /// <summary>
        /// Load a plugin
        /// </summary>
        /// <param name="loader">Plugin to load</param>
        /// <param name="startPlugin">If true, the plugin will be started</param>
        /// <returns>Return loader on another app domain</returns>
        public static PluginLoader LoadPlugin(ref PluginLoader loader, bool startPlugin = true)
        {
            if (!Global.userData.settings.PluginSettings.enabled)
                return null;

            LoaderToAppDomain(ref loader);

            loader.Load();

            if (startPlugin)
                StartPlugin(loader.AgPlugin);

            return loader;
        }  

        /// <summary>
        /// Start plugin
        /// </summary>
        /// <param name="plugin">Plugin to start</param>
        public static void StartPlugin(Plugin plugin)
        {
            if (!Global.userData.settings.PluginSettings.enabled)
                return;

            plugin.Start();
        }

        /// <summary>
        /// Add plugin in the user plugins list
        /// </summary>
        /// <param name="plugin">Plugin to add</param>
        public static void AddPlugin(PluginLoader plugin)
        {
            if (!plugins.Contains(plugin))
                plugins.Add(plugin);
        }

        /// <summary>
        /// Remove plugin from the user plugins list
        /// </summary>
        /// <param name="plugin">Plugin to remove</param>
        public static void RemovePlugin(PluginLoader plugin)
        {
            if (plugins.Contains(plugin))
                plugins.Remove(plugin);
        }

        /// <summary>
        /// Check if a plugin is already installed
        /// </summary>
        /// <param name="_plugin">Plugin to check</param>
        /// <returns>Return true if plugin is installed</returns>
        public static bool AlreadyInstalled(PluginLoader _plugin)
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                if (_plugin.Equals(plugins[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Update installed plugin instance in the installed plugin list
        /// </summary>
        /// <param name="_plugin">Plugin to update</param>
        public static void UpdateInstance(PluginLoader _plugin)
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                if (_plugin.Equals(plugins[i]))
                {
                    plugins[i] = _plugin;
                    return;
                }
            }
        }

        /// <summary>
        /// Display plugin menus
        /// </summary>
        /// <param name="_plugin">Plugin whose menus will be displayed</param>
        internal static void DisplayPluginMenus(PluginLoader _plugin)
        {
            if (!Global.userData.settings.PluginSettings.enabled)
                return;

            //Create MenuItem
            System.Windows.Controls.MenuItem GenerateItem(string _header)
            {

                System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem()
                {
                    Header = _header,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontWeight = System.Windows.FontWeights.Normal
                };

                return item;
            }

            if (_plugin.Isloaded)
            {
                System.Windows.Controls.MenuItem rootMenu = GenerateItem(_plugin.name);

                List<System.Windows.Controls.MenuItem> headers = new List<System.Windows.Controls.MenuItem>();

                System.Windows.Controls.MenuItem headerMenu;
                System.Windows.Controls.MenuItem menuItem;

                foreach (PluginMenuItemAttribute pmi in _plugin.PluginMenus)
                {
                    menuItem = GenerateItem(pmi.ItemTitle);

                    if (string.IsNullOrEmpty(pmi.GroupName))
                    {
                        rootMenu.Items.Add(menuItem);
                    }
                    else
                    {
                        headerMenu = headers.Find(x => (string)x.Header == pmi.GroupName);
                        if (headerMenu == null)
                        {
                            headerMenu = GenerateItem(pmi.GroupName);

                            rootMenu.Items.Add(headerMenu);
                            headers.Add(headerMenu);
                        }

                        headerMenu.Items.Add(menuItem);
                    }

                    menuItem.Click += new System.Windows.RoutedEventHandler(
                       (sender, e) =>
                       {
                           pmi.method.Invoke(null, new object[0]);
                       });

                    headerMenu = null;
                }

                if (rootMenu.Items.Count > 0)
                {
                    Global.mainWindow.ShowPluginMainMenus(rootMenu);
                }
            }

        }

        /// <summary>
        /// Get homeworks menu items of a plugin
        /// </summary>
        /// <param name="_plugin">Plugin whose menu items will be returned</param>
        /// <param name="vi">Homework viewer whose this menu will be applied</param>
        /// <returns>Return a list of menu items</returns>
        private static List<System.Windows.Controls.MenuItem> GetHomewoksMenus(PluginLoader _plugin, IHomeworkViewer vi)
        {
            if(_plugin.Enabled)
            {
                List<System.Windows.Controls.MenuItem> menuItems = new List<System.Windows.Controls.MenuItem>();
                System.Windows.Controls.MenuItem menuItem;

                foreach (HomeworksMenuItemAttribute item in _plugin.HomeworksMenus)
                {
                    menuItem = new System.Windows.Controls.MenuItem() { Header = item.Title };
                    menuItems.Add(menuItem);

                    menuItem.Click += new System.Windows.RoutedEventHandler(
                       (sender, e) =>
                       {
                           item.method.Invoke(null, new object[] { vi });
                       });

                    menuItem.Tag = _plugin;
                }

                return menuItems;
            }

            return new List<System.Windows.Controls.MenuItem>();
        }

        /// <summary>
        /// Get homeworks menu items list of all installed plugins
        /// </summary>
        /// <param name="vi">Homework viewer whose these menus will be applied</param>
        /// <returns>Return menu items list</returns>
        public static List<System.Windows.Controls.MenuItem> GetHomewoksMenus(IHomeworkViewer vi)
        {
            List<System.Windows.Controls.MenuItem> menus = new List<System.Windows.Controls.MenuItem>();

            foreach (PluginLoader pl in plugins)
            {
                foreach (System.Windows.Controls.MenuItem item in GetHomewoksMenus(pl, vi))
                {
                    menus.Add(item);
                }
            }

            return menus;
        }

        internal static Plugin GetPluginByObject(object obj)
        {
            return plugins.FindAll(x => x.Isloaded).Find(x=>x.assembly?.FullName == obj.GetType().Assembly.FullName)?.AgPlugin;
        }

    }
}
