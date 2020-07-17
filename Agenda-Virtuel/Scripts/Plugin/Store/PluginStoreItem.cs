using System;
using System.IO;
using System.Net;
using Agenda_Virtuel.Manager;
using MyFunctions;

namespace Agenda_Virtuel.Plugin.Store
{
    /// <summary>
    /// This class allows to get infos of a plugin in the plugin store and download and install.
    /// </summary>
    [Serializable]
    internal class PluginStoreItem
    {
        //Constants
        /// <summary>
        /// Local path of plugins download
        /// </summary>
        [NonSerialized] public static string DownloadLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";

        //Variables
        private string name;
        private string description;
        private string version;
        private string url;
        private string developerName;
        private string userAccount; // exemple : ADMAN
        private readonly bool isVirus = true;
        [NonSerialized] private string localpluginFile;

        //Properties
        /// <summary>
        /// The name of the plugin
        /// </summary>
        public string Name { get => name; private set => name = value; }
        /// <summary>
        /// The description of the plugin
        /// </summary>
        public string Description { get => description; private set => description = value; }
        /// <summary>
        /// The version of the plugin
        /// </summary>
        public string Version { get => version; private set => version = value; }
        /// <summary>
        /// The download url of the plugin
        /// </summary>
        public string DownloadUrl { get => url; private set => url = value; }
        /// <summary>
        /// The developer name of the plugin
        /// </summary>
        public string DeveloperName { get => developerName; private set => developerName = value; }
        /// <summary>
        /// The user account name of the plugin
        /// </summary>
        public string UserAccount { get => userAccount; private set => userAccount = value; }
        /// <summary>
        /// Determine if this plugin is clean
        /// </summary>
        public bool IsAnVirus => isVirus;
        /// <summary>
        /// Determine if an update is available
        /// </summary>
        public bool ToUpdate { get; internal set; } = false;

        //Constructors
        /// <summary>
        /// Instantiate a PluginStoreItem object
        /// </summary>
        public PluginStoreItem()
        {
            isVirus = true;
        }

        /// <summary>
        /// Instantiate a PluginStoreItem object
        /// </summary>
        /// <param name="_name">The name of the plugin</param>
        /// <param name="_description">The description of the plugin</param>
        /// <param name="_url">The download url of the plugin</param>
        /// <param name="_developer">The developer name of the plugin</param>
        /// <param name="_userAccount">The user account name of the plugin</param>
        /// <param name="_isVirus">True if the plugin is clean</param>
        /// <param name="_version">The version of the plugin</param>
        public PluginStoreItem(string _name, string _description, string _url, string _developer, string _userAccount, bool _isVirus, string _version)
        {
            name = _name;
            description = _description;
            url = _url;
            developerName = _developer;
            userAccount = _userAccount;
            isVirus = _isVirus;
            version = _version;
        }

        /// <summary>
        /// Download plugin
        /// </summary>
        /// <returns>True if plugin has been downloaded correctly</returns>
        public bool Download()
        {
            if (isVirus)
            {
                if (System.Windows.Forms.MessageBox.Show("Attention : Ce plugin est peu sûr."
                    + Environment.NewLine + "Êtes-vous sûr de vouloir continuer ?", name,
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                {
                    return false;
                }
            }

            bool success;
            try
            {
                localpluginFile = DownloadLocation + url.SplitAndGetLastPart("/");
                string fullUrl = PluginStoreManager.pluginsstoreDir + url;

                WebClient wc = new WebClient();
                wc.DownloadFile(fullUrl, localpluginFile);
                success = true;
            }
            catch (Exception e)
            {
                throw e;
            }

            return success;
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>True if the plugin has been installed correctly</returns>
        public bool Install()
        {
            if (!File.Exists(localpluginFile))
            {
                if (!Download())
                    return false;
            }

            PluginInstaller pi = PluginInstaller.InstallFromFile(localpluginFile, out bool r);

            if (r)
            {
                pi.ThePlugin.Enabled = true;
                PluginManager.StartPlugin(pi.ThePlugin.AgPlugin);
                PluginManager.Save();
            }

            File.Delete(localpluginFile);
            // Directory.Delete(localpluginFile.SplitAndGetPart(@"\", 0));

            return r;
        }

    }
}
