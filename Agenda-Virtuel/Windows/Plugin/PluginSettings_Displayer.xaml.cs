using System.Windows;

namespace Agenda_Virtuel.Plugin
{
    internal partial class PluginSettings_Displayer : Window
    {
        public PluginSettings_Displayer(Plugin plugin)
        {
            InitializeComponent();

            if (plugin.PluginSettingsWindow != null)
                this.Group.Content = plugin.PluginSettingsWindow;

            this.Group.Header = plugin.Name + " - Paramètres";
        }
    }
}
