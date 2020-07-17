using System;
using System.Windows.Forms;
using Agenda_Virtuel.Plugin;

namespace Agenda_Virtuel.Controls.Plugin
{
    internal partial class PluginItemView : System.Windows.Controls.UserControl
    {
        //Variables
        private PluginLoader plugin;

        //Constructor
        internal PluginItemView(PluginLoader _plugin)
        {
            InitializeComponent();

            plugin = _plugin;

            this.CB_Enabled.Content = plugin.name;
            this.CB_Enabled.IsChecked = plugin.Enabled;

            this.BT_OpenSettings.IsEnabled = plugin.GetSettingsWindow() != null;
        }

        //Enable or disable the plugin
        private void CB_Enabled_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            plugin.Enabled = this.CB_Enabled.IsChecked == true;
            PluginManager.Save();

            if (plugin.Enabled)
                PluginManager.StartPlugin(plugin.AgPlugin);
        }

        //Uninstall the plugin
        private void BT_UnInstall_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes vous sûr de vouloir désinstaller ce plugin ?", plugin.name, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            this.CB_Enabled.IsChecked = false;

            plugin.Unistall();
        }

        //Open plugin settings
        private void BT_OpenSettings_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new PluginSettings_Displayer(plugin.AgPlugin).ShowDialog();
        }
    }
}
