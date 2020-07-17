using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Agenda_Virtuel.Manager;
using Agenda_Virtuel.Plugin;
using Agenda_Virtuel.Plugin.Store;
using MyFunctions;

namespace Agenda_Virtuel.Windows.Plugin.Store
{
    internal partial class UploadPluginWindow : Window
    {
        //Enums
        internal enum UploadMode { Normal, Update}

        //Variables
        private readonly UploadMode mode;
        private PluginInstaller installer;
        private readonly string userAccount;

        //Properties
        public PluginStoreItem StoreItem { get; private set; }
        public string LocalZipFile { get; private set; }

        //Constructor
        public UploadPluginWindow(UploadMode _mode = UploadMode.Normal)
        {
            InitializeComponent();

            userAccount = PluginStoreManager.UserName;
            mode = _mode;
        }

        //Choose the .zip file of the plugin
        private void BT_ChooseZipFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Fichier .zip" + "|*.zip"
            };

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string installerEntry="";
                foreach (System.IO.Compression.ZipArchiveEntry item in FileManager.GetZipEntries(ofd.FileName))
                {
                    if(item.Name == "Installer" + PluginInstaller.installerExtension)
                    {
                        installerEntry = item.FullName;
                    }
                }

                MemoryStream ins = FileManager.GetFileFromZip(ofd.FileName, installerEntry);

                if (ins != null)
                {
                    PluginInstaller pi = FileManager.Deserialize<PluginInstaller>(ins);
                    ins.Close();

                    if (pi != null)
                    {
                        installer = pi;

                        this.TB_ZipFile.Text = ofd.FileName;
                        this.TB_Description.Text = pi.ThePlugin.description;
                        this.TB_Version.Text = pi.ThePlugin.version;
                    }
                }
                else
                {
                    throw new Exception("The Installer" + PluginInstaller.installerExtension + " was not found !");
                }
            }
        }

        //Upload the plugin
        private void BT_Upload_Click(object sender, RoutedEventArgs e)
        {
            if (installer == null)
                return;

            LocalZipFile = this.TB_ZipFile.Text;

            if (!File.Exists(LocalZipFile))
                return;

            if (string.IsNullOrEmpty(this.TB_Version.Text))
            {
                System.Windows.Forms.MessageBox.Show("Entrer tout les champs !", "Agenda-Virtuel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string pluginUrl = LocalZipFile.SplitAndGetLastPart(@"\").Replace(" ", "");
            StoreItem = new PluginStoreItem(installer.ThePlugin.name, this.TB_Description.Text, pluginUrl, userAccount, userAccount, true, this.TB_Version.Text);

            switch (mode)
            {
                case UploadMode.Normal:
                    PluginStoreManager.InsertPlugin(StoreItem, LocalZipFile);
                    break;
            }                

            this.DialogResult = true;
            this.Close();
        }
    }
}
