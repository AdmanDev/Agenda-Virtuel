using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Agenda_Virtuel.Controls.Plugin.Store;
using Agenda_Virtuel.Manager;
using Agenda_Virtuel.Plugin.Store;
using Agenda_Virtuel.Windows.Plugin.Store;
using MyFunctions;

namespace Agenda_Virtuel
{
    internal partial class PluginStoreWindow : Window
    {
        //Variables
        private bool connected = false;

        private Button SelectedButton;

        //Constructor
        public PluginStoreWindow()
        {
            InitializeComponent();
        }

        //Close this window
        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //Load
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HighlightButton(this.BT_DisplayAll);

            await PluginStoreManager.LoadAllStoreAsync();
            DisplayPlugins();

            await TryToConnectAsync();
        }

        private async System.Threading.Tasks.Task TryToConnectAsync()
        {
            if (SaveInServer.IsEnabled())
            {

                if (string.IsNullOrEmpty(PluginStoreManager.UserName))
                {
                    FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("mode", "getusername"),
                        new KeyValuePair<string, string>("userid", SaveInServer.UserID.ToString())
                    });

                    XElement json = await Functions.SendPostRequest(elements, PluginStoreManager.PHPpluginstoreURL);
                    PluginStoreManager.UserName = json.Element("Name")?.Value;

                    connected = true;
                    PluginStoreManager.UserID = SaveInServer.UserID;
                    ShowConnectedUserMenu();
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void HighlightButton(Button button)
        {
            if (button == null || button == SelectedButton)
                return;

            if (SelectedButton != null)
                SelectedButton.Background = Brushes.White;

            SelectedButton = button;
            button.Background = new SolidColorBrush(Color.FromRgb(58, 237, 255));

        }

        //Display all plugins of the store
        private void DisplayPlugins()
        {
            PluginStoreManager.installedPlugins = PluginStoreManager.GetInstalledPlugins();
            this.SP_PluginItems.Children.Clear();
            foreach (PluginStoreItem i in PluginStoreManager.Plugins)
            {
                this.SP_PluginItems.Children.Add(new PStoreItemViewer(i));
            }
        }

        //Connecton
        private void BT_Connection_Click(object sender, RoutedEventArgs e)
        {
            AccountConnectionWindow acw = new AccountConnectionWindow();

            if (acw.ShowDialog() == true)
            {
                connected = true;
                PluginStoreManager.UserName = acw.UserName;
                PluginStoreManager.UserID = acw.UserID;
                ShowConnectedUserMenu();
            }
        }

        private void ShowConnectedUserMenu()
        {
            this.SP_HeaderTools.Children.Clear();

            this.SP_HeaderTools.Children.Add(this.BT_DisplayAll);
            this.SP_HeaderTools.Children.Add(this.BT_InstalledPlugins);
            this.SP_HeaderTools.Children.Add((UIElement)this.FindResource("BT_DisplayUserPlugins"));
            this.SP_HeaderTools.Children.Add((UIElement)this.FindResource("BT_UploadPlugin"));
        }

        //Upload a new plugin
        private void BT_UploadPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (new UploadPluginWindow().ShowDialog() == true)
            {
                DisplayPlugins();
            }
        }

        //Display plugins of the user
        private void BT_DisplayUserPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (connected)
            {
                HighlightButton((Button)sender);

                this.SP_PluginItems.Children.Clear();
                PluginStoreManager.GetUserPlugins(PluginStoreManager.UserName).ForEach((x) =>
                {
                    this.SP_PluginItems.Children.Add(new PStoreItemViewer(x, PStoreItemViewer.PStoreItemMode.MyUploadedPlugins));
                });
            }
        }

        //Display all plugins of the store
        private async void BT_DisplayAll_Click(object sender, RoutedEventArgs e)
        {
            HighlightButton(this.BT_DisplayAll);

            await PluginStoreManager.LoadAllStoreAsync();
            DisplayPlugins();
        }

        //Display installed plugins
        private void BT_DisplayInstalledPlugins_Click(object sender, RoutedEventArgs e)
        {
            HighlightButton(this.BT_InstalledPlugins);

            this.SP_PluginItems.Children.Clear();
            PluginStoreManager.installedPlugins.ForEach((x) =>
            {
                this.SP_PluginItems.Children.Add(new PStoreItemViewer(x, PStoreItemViewer.PStoreItemMode.InstalledPlugins));
            });
        }

    }
}
