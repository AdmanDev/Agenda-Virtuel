using System;
using System.Collections.Generic;
using MyFunctions;
using System.IO;
using Agenda_Virtuel.Plugin.Store;
using Agenda_Virtuel.Plugin;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This static class manage the plugin store.
    /// </summary>
    internal static class PluginStoreManager
    {
        //Constants variables
        /// <summary>
        /// The plugins base url
        /// </summary>
        public const string pluginsstoreDir = "https://admandev.fr/api/agenda_virtuel/pluginstore/store/";
        /// <summary>
        /// Path of text file that contains infos of recent plugin update
        /// </summary>
        public static readonly string localUpdateFile = System.Windows.Forms.Application.StartupPath + @"\UpdatePlugin.txt";

        /// <summary>
        /// Url to do requests server
        /// </summary>
        public static readonly string PHPpluginstoreURL = "https://admandev.fr/api/agenda_virtuel/pluginstore/requests/global.php";

        //Properties
        /// <summary>
        /// User name of current user
        /// </summary>
        public static string UserName { get; set; }
        /// <summary>
        /// User Id of current user
        /// </summary>
        public static int UserID { get; set; }


        //Properties
        /// <summary>
        /// Plugins list available in the store
        /// </summary>
        public static List<PluginStoreItem> Plugins { get; private set; } = new List<PluginStoreItem>();
        /// <summary>
        /// List of installed plugins
        /// </summary>
        public static List<PluginStoreItem> installedPlugins = new List<PluginStoreItem>();


        /// <summary>
        /// Get available plugins from the store. This function is asynchronous
        /// </summary>
        /// <returns>Task object</returns>
        public static async Task LoadAllStoreAsync()
        {
            if (Plugins == null)
                Plugins = new List<PluginStoreItem>();
            else
                Plugins.Clear();

            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", "select")
            });

            XElement json = await Functions.SendPostRequest(elements, PHPpluginstoreURL);

            foreach (XElement xe in json.Elements())
            {
                string name = xe.Element("Name")?.Value;
                string description = xe.Element("Description")?.Value;
                string url = xe.Element("Url")?.Value;
                string developer = xe.Element("Developer")?.Value;
                bool isVirus = Convert.ToBoolean(Convert.ToInt32(xe.Element("IsVirus")?.Value));
                string version = xe.Element("Version")?.Value;

                PluginStoreItem currPlugin = new PluginStoreItem(name, description, url, developer, developer, isVirus, version);
                Plugins.Add(currPlugin);
            }

            Plugins.Reverse();
        }

        /// <summary>
        /// Get plugins list that Get the list of plugins that have been developed by a specific user
        /// </summary>
        /// <param name="_user">User name of plugins developer</param>
        /// <returns>List of PluginStoreItem object containing plugins list of a specific developer</returns>
        public static List<PluginStoreItem> GetUserPlugins(string _user)
        {
            return Plugins.FindAll(x => x.UserAccount == _user);
        }

        /// <summary>
        /// Get a list of installed plugin in current application instance.
        /// </summary>
        /// <returns>List of PluginStoreItem object containing installed plugins list</returns>
        public static List<PluginStoreItem> GetInstalledPlugins()
        {
            List<PluginStoreItem> result = new List<PluginStoreItem>();
            PluginStoreItem itemFound;

            foreach (PluginLoader pinstalled in PluginManager.Plugins)
            {
                itemFound = Plugins.Find(x => x.Name == pinstalled.name);

                if (itemFound != null)
                {
                    itemFound.ToUpdate = itemFound.Version != pinstalled.version;

                    result.Add(itemFound);
                }
            }

            return result;
        }

        /// <summary>
        /// Determine if there is no error or any problem to upload plugin in the store
        /// </summary>
        /// <param name="plugin">Plugin store item to test</param>
        /// <param name="localZipPath">Zip file of plugin to test</param>
        /// <returns>Return true if there is no problem to upload this plugin</returns>
        private static bool IsUploadable(PluginStoreItem plugin, string localZipPath)
        {
            if (plugin == null || string.IsNullOrEmpty(plugin.DownloadUrl) || !File.Exists(localZipPath))
                return false;

            if (Plugins.Find(x => x.Name == plugin.Name) != null)
            {
                System.Windows.Forms.MessageBox.Show("Un plugin du même nom éxiste déjà dans le store.", plugin.Name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }

            return true;
        }
        
        private static string StoreRequest(string mode, PluginStoreItem plugin, string localZipPath)
        {
            string zipName = localZipPath.SplitAndGetLastPart(@"\");
            string result = RequestHelper.PostMultipart
            (
                PHPpluginstoreURL,
                new Dictionary<string, object>() {
                    { "mode", mode },
                    { "user", UserID },
                    { "Name", plugin.Name },
                    { "Description", plugin.Description },
                    { "Version", plugin.Version },
                    { "zipFile", new FormFile() { Name = zipName, ContentType = "application/zip", FilePath = localZipPath } },
            });

            return result;
        }

        /// <summary>
        /// Upload a plugin in the store
        /// </summary>
        /// <param name="plugin">Plugin (store item) to upload</param>
        /// <param name="localZipPath">Zip file of plugin to upload</param>
        public static void InsertPlugin(PluginStoreItem plugin, string localZipPath)
        {
            if (!IsUploadable(plugin, localZipPath))
            {
                return;
            }

            StoreRequest("insert", plugin, localZipPath);
            Plugins.Add(plugin);
        }

        /// <summary>
        /// Update a plugin.
        /// </summary>
        /// <param name="oldItem">Old version of plugin</param>
        /// <param name="newItem">New version of plugin</param>
        /// <param name="_localZipFile">Zip file of new version of plugin</param>
        public static void UpdatePlugin(PluginStoreItem oldItem, PluginStoreItem newItem, string _localZipFile)
        {
            PluginStoreItem plugin = new PluginStoreItem(oldItem.Name, newItem.Description, oldItem.DownloadUrl, newItem.DeveloperName, newItem.UserAccount, true, newItem.Version);
            StoreRequest("update", plugin, _localZipFile);
        }

        /// <summary>
        /// Delete a plugin from the store.
        /// </summary>
        /// <param name="plugin">Plugin to delete</param>
        public static void DeletePlugin(PluginStoreItem plugin)
        {
            RequestHelper.PostMultipart
            (
                PHPpluginstoreURL,
                new Dictionary<string, object>() {
                    { "mode", "delete" },
                    { "Name", plugin.Name }
            });
        }

    }
}
