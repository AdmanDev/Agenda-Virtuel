using System;
using System.IO;
using Agenda_Virtuel.Manager;
using MyFunctions;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// This class allows to create a plugin installer and install a plugin.
    /// </summary>
    [Serializable]
    internal class PluginInstaller
    {
        //Variables
        private PluginLoader plugin;
        private string dllName;
        private string[] dlls;

        //NonSierialized Variables
        [NonSerialized] public const string installerExtension = ".Avplugin";
        [NonSerialized] private string installerFolderPath;

        //Properties
        /// <summary>
        /// It's the plugin loader
        /// </summary>
        internal PluginLoader ThePlugin { get => plugin; set => plugin = value; }
        /// <summary>
        /// It's the main dll name of the plugin (with extension)
        /// </summary>
        public string MainDllName { get=>dllName; set=>dllName=value; }
        /// <summary>
        /// It's the path of the main plugin's dll withouth dll name
        /// </summary>
        public string MainDllPath { get; set; }
        /// <summary>
        /// Array of additionals dlls paths
        /// </summary>
        public string[] OtherDlls { get => dlls; private set => dlls = value; }

        /// <summary>
        /// Set the plugin directory or get full plugin dll path
        /// </summary>
        public string InstalerFolderPath
        {
            get => installerExtension;
            set
            {
                if (Directory.Exists(value))
                {
                    installerFolderPath = value;
                    MainDllPath = value + @"\" + MainDllName;
                }
                else
                {
                    throw new IOException("The following directory was not fond : " +
                        Environment.NewLine + value);
                }
            }
        }

        //Constructors
        /// <summary>
        /// Instantiate a new PluginInstaller
        /// </summary>
        public PluginInstaller()
        {

        }

        /// <summary>
        /// Instantiate a new PluginInstaller
        /// </summary>
        /// <param name="_plugin">The plugin loader of the plugin to install</param>
        /// <param name="_mainDllPath">The path of the main plugin dll</param>
        /// <param name="_otherDlls">Additional dlls</param>
        public PluginInstaller(PluginLoader _plugin, string _mainDllPath, string[] _otherDlls)
        {
            ThePlugin = _plugin;
            MainDllPath = _mainDllPath;
            MainDllName = new FileInfo(_mainDllPath).Name;
            OtherDlls = _otherDlls;

            _plugin.secDlls = new string[_otherDlls.Length];
            for (int i = 0; i < _otherDlls.Length; i++)
            {
                _plugin.secDlls[i] = PluginManager.DllsDirectory + @"\" + new FileInfo(_otherDlls[i]).Name;
            }
        }

        //Install this plugin
        /// <summary>
        /// Install plugin
        /// </summary>
        /// <param name="showMessage">Show error if true</param>
        /// <returns>True if plugin has been installed correctly</returns>
        public bool Install(bool showMessage = true)
        {
            //Create the dlls directory
            if (!Directory.Exists(PluginManager.DllsDirectory))
                Directory.CreateDirectory(PluginManager.DllsDirectory);

            if (PluginManager.InstalledPluginCount >= 1 && showMessage)
            {
                if (System.Windows.Forms.MessageBox.Show("Vous avez déjà des plugins installés, si vous en installer plusieurs, il ce peut qu'il y est des erreurs de compatibilité.", ThePlugin.name, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    return false;
            }

            if (PluginManager.AlreadyInstalled(ThePlugin))
            {
                if (showMessage)
                    System.Windows.Forms.MessageBox.Show("Ce plugin est déjà installé.", ThePlugin.name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }

            File.Copy(MainDllPath, PluginManager.DllsDirectory + MainDllName, true);
            ThePlugin.DllPath = PluginManager.DllsDirectory + MainDllName;

            //copy socondary dlls
            foreach (string d in OtherDlls)
            {
                File.Copy(d, PluginManager.DllsDirectory + new FileInfo(d).Name, false);
            }

            PluginManager.AddPlugin(ThePlugin);

            plugin.Enabled = true;
            PluginManager.StartPlugin(plugin.AgPlugin);

            EnableControls();

            PluginManager.Save();

            EventsManager.Call_OnPluginInstalled(ThePlugin);
            return true;
        }

        /// <summary>
        /// Disable current homework editor and homework viewer and activate those of this plugin
        /// </summary>
        private void EnableControls()
        {
            if (plugin.AgPlugin.HomeworkEditor != null)
            {
                foreach (PluginLoader pl in PluginManager.Plugins)
                {
                    pl.isHomeworksEditor = false;
                }

                plugin.isHomeworksEditor = true;
                HomeworkManager.HomeworkEditor = plugin.AgPlugin.HomeworkEditor;
            }

            if (plugin.AgPlugin.HomeworkViewer != null)
            {
                foreach (PluginLoader pl in PluginManager.Plugins)
                {
                    pl.isHmsViewer = false;
                }

                plugin.isHmsViewer = true;
                HomeworkManager.ViewerContainer = plugin.AgPlugin.HomeworkViewer;
            }
        }

        /// <summary>
        /// Create an plugin installer file
        /// </summary>
        /// <param name="output">Output file</param>
        /// <param name="plugin">Plugin whose you want to crate an installer</param>
        /// <param name="secDlls">Additional dlls</param>
        /// <returns>PluginInstaller object</returns>
        public static PluginInstaller CreateInstaller(string output, PluginLoader plugin, string[] secDlls)
        {
            string dirName = output.SplitAndGetLastPart(@"\").Replace(" ", "");
            output = output.Replace(output.SplitAndGetLastPart(@"\"), dirName);
            Directory.CreateDirectory(output);

            if (!Directory.Exists(output))
                return null;

            FileInfo dllInfo = new FileInfo(plugin.DllPath);

            PluginInstaller installer = new PluginInstaller(plugin, output + @"\" + dllInfo.Name, secDlls);

            MyFunctions.FileManager.Serialize(output + @"\Installer" + installerExtension, installer);
            File.Copy(dllInfo.FullName, output + @"\" + dllInfo.Name, true);

            //Copy secondary dlls
            for (int i = 0; i < secDlls.Length; i++)
            {
                File.Copy(secDlls[i], output + @"\" + new FileInfo(secDlls[i]).Name, true);
            }

            //Zip
            FileManager.ZipDirectory(output, output + ".zip");

            return installer;
        }

        /// <summary>
        /// Open and Install a plugin from .zip or .Avplugin file
        /// </summary>
        /// <param name="filePath">The installer file (.zip or .Avplugin)</param>
        /// <param name="result">The result of the process</param>
        /// <param name="showMessage"></param>
        /// <returns>PluginInstaller object</returns>
        public static PluginInstaller InstallFromFile(string filePath, out bool result, bool showMessage = true)
        {
            result = false;
            if (!File.Exists(filePath))
                return null;

            FileInfo file = new FileInfo(filePath);

            if (file.Extension != ".zip" && file.Extension != installerExtension)
                return null;

            string unzipedDirectory = "";
            if (file.Extension == ".zip")
            {
                string localFile = filePath.Replace(".zip", "");
                if (Directory.Exists(localFile))
                    Directory.Delete(localFile, true);

                FileManager.UnzipFile(filePath, localFile);
                unzipedDirectory = localFile;
                filePath = unzipedDirectory + @"\" + new DirectoryInfo(localFile).Name + @"\Installer" + installerExtension;
            }

            PluginInstaller installer = FileManager.Deserialize<PluginInstaller>(filePath);

            if (file.Extension == ".zip")
                installer.InstalerFolderPath = filePath.Replace(@"\Installer" + installerExtension, "");
            else
                installer.InstalerFolderPath = file.Directory.FullName;


            result = installer.Install(showMessage);

            if (Directory.Exists(unzipedDirectory))
                Directory.Delete(unzipedDirectory, true);

            return installer;
        }

    }
}
