using Agenda_Virtuel.Controls.Plugin;
using Agenda_Virtuel.Manager;
using Agenda_Virtuel.Plugin;
using System.Windows;
using System.Windows.Forms;

namespace Agenda_Virtuel
{
    internal partial class PluginManagerWindow : Window
    {
        //Variables
        private OpenFileDialog OFD;
        private FolderBrowserDialog FBD;

        //Constructor
        public PluginManagerWindow()
        {
            InitializeComponent();

            OFD = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = PluginInstaller.installerExtension,
                Filter = "Fichier " + PluginInstaller.installerExtension +
                         "|*" + PluginInstaller.installerExtension + "|Fichier .zip" + "|*.zip"
            };

            FBD = new FolderBrowserDialog()
            {
                Description = "Destination"
            };

            EventsManager.OnPluginInstalled += OnOnPluginInstalled;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ShowPlugins();
        }

        //Show all plugin
        private void ShowPlugins()
        {
            this.SP_Plugins.Children.Clear();

            foreach (PluginLoader p in PluginManager.Plugins)
            {
                this.SP_Plugins.Children.Add(new PluginItemView(p));
            }
        }

        private void BT_NewPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (Panel_AddPlugin.Visibility != Visibility.Visible)
                this.Panel_AddPlugin.Visibility = Visibility.Visible;
            else
                this.Panel_AddPlugin.Visibility = Visibility.Collapsed;
        }

        private void BT_Install_Click(object sender, RoutedEventArgs e)
        {
            if (this.OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PluginInstaller.InstallFromFile(this.OFD.FileName, out bool r);
            }

            this.Panel_AddPlugin.Visibility = Visibility.Collapsed;
        }

        private void OnOnPluginInstalled(PluginLoader obj)
        {
            ShowPlugins();
        }

        private void BT_CreateInstaller_Click(object sender, RoutedEventArgs e)
        {
            PluginInstallerWindow piw = new PluginInstallerWindow();

            if (piw.ShowDialog() == true)
            {
                if (this.FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string dir = this.FBD.SelectedPath + @"\" + piw.plugin.name + @"_Installer";
                    PluginInstaller installer = PluginInstaller.CreateInstaller(dir, piw.plugin, piw.secDlls);

                    if (System.Windows.Forms.MessageBox.Show("Voulez-vous installer ce plugin ?", "Agenda - Virtuel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        installer.Install();
                    }
                    
                }

                this.Panel_AddPlugin.Visibility = Visibility.Collapsed;
            }
        }

        private void BT_PluginStore_Click(object sender, RoutedEventArgs e)
        {
            new PluginStoreWindow().ShowDialog();
        }
    }
}
