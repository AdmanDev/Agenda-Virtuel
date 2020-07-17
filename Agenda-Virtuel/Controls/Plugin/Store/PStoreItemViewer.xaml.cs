using Agenda_Virtuel.Manager;
using Agenda_Virtuel.Plugin;
using Agenda_Virtuel.Plugin.Store;
using Agenda_Virtuel.Windows.Plugin.Store;
using MyFunctions;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Agenda_Virtuel.Controls.Plugin.Store
{ 
    internal partial class PStoreItemViewer : System.Windows.Controls.UserControl
    {
        //Enums
        internal enum PStoreItemMode { Normal, MyUploadedPlugins, InstalledPlugins }

        //Variables
        private PluginStoreItem sItem;
        private readonly PStoreItemMode mode;

        //Properties
        internal PluginStoreItem ItemInfo
        {
            get => sItem;
            private set
            {
                sItem = value;
                DisplayItemInfo();
            }
        }

        //Constructors
        internal PStoreItemViewer()
        {
            InitializeComponent();
        }

        internal PStoreItemViewer(PluginStoreItem _item, PStoreItemMode _editMode = PStoreItemMode.Normal)
        {
            InitializeComponent();

            mode = _editMode;
            ItemInfo = _item;
        }

        //Display plugin informations
        private void DisplayItemInfo()
        {
            if (ItemInfo == null)
                return;

            if (ItemInfo.IsAnVirus)
            {
                this.IMG_Security.ToolTip = "Ce plugin est peut sûr !";
                this.IMG_Security.Source = WpfFunctions.BitmapToImageSource(Properties.Resources.Warning_Icon);
            }
            else
            {
                this.IMG_Security.ToolTip = "Ce plugin est sûr !";
                this.IMG_Security.Source = WpfFunctions.BitmapToImageSource(Properties.Resources.Ok_Icon);
            }

            switch (mode)
            {
                case PStoreItemMode.MyUploadedPlugins:
                    this.BT_DeletePlugin.Visibility = Visibility.Visible;
                    this.BT_SendUpdate.Visibility = Visibility.Visible;
                    break;
            }

            if (PluginStoreManager.installedPlugins.Find(x => x.Name == ItemInfo.Name) != null)
            {
                this.BT_DownloadPlugin.Visibility = Visibility.Collapsed;
                this.BT_UninstallPlugin.Visibility = Visibility.Visible;

                if (ItemInfo.ToUpdate)
                    this.BT_Update.Visibility = Visibility.Visible;
            }

            this.LB_PluginName.Content = ItemInfo.Name;
            this.LB_DeveloperName.Content = "Developpeur : " + ItemInfo.DeveloperName;
            this.LB_PluginDescription.Text = ItemInfo.Description;
        }

        //Download and install the plugin
        private void BT_DownloadPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (ItemInfo == null)
                return;

            if (!ItemInfo.Download())
                return;

            if (ItemInfo.Install())
            { //Plugin installed
                this.BT_DownloadPlugin.Visibility = Visibility.Collapsed;
                this.BT_UninstallPlugin.Visibility = Visibility.Visible;

                PluginStoreManager.installedPlugins.Add(ItemInfo);

                System.Windows.Forms.MessageBox.Show("Le plugin a été installé avec succès.", ItemInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Erreur : Impossible d'installer ce plugin.", ItemInfo.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //Delete this plugin from the store
        private void BT_DeletePlugin_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Êtes-vous sûr de vouloir supprimer ce plugin du store ?", "Agenda-Virtuel", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                PluginStoreManager.DeletePlugin(ItemInfo);
                ((StackPanel)this.Parent).Children.Remove(this);
            }
        }

        //Uninstall the plugin
        private void BT_UninstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Êtes-vous sûr de vouloir désinstaller ce plugin ?", ItemInfo.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            PluginLoader plugin = PluginManager.Plugins.First(x => x.name == ItemInfo.Name);

            if (plugin != null)
            {
                plugin.Unistall();

                if (mode == PStoreItemMode.InstalledPlugins)
                    ((StackPanel)this.Parent).Children.Remove(this);
                else
                {
                    this.BT_UninstallPlugin.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void BT_SendUpdate_Click(object sender, RoutedEventArgs e)
        {
            UploadPluginWindow upw = new UploadPluginWindow(UploadPluginWindow.UploadMode.Update);
            if(upw.ShowDialog() == true)
            {
                PluginStoreManager.UpdatePlugin(this.ItemInfo, upw.StoreItem, upw.LocalZipFile);
                this.ItemInfo = upw.StoreItem;
            }
        }

        private void BT_Update_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(PluginStoreManager.localUpdateFile, ItemInfo.DownloadUrl);

            PluginLoader plugin = PluginManager.Plugins.First(x => x.name == ItemInfo.Name);

            if (plugin != null)
            {
                plugin.Unistall(showMessage: false, removePluginParams: false);
            }
        }
    }
}
