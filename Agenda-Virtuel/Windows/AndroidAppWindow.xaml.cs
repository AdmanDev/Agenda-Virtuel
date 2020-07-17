using System.Windows;

namespace Agenda_Virtuel
{
    public partial class AndroidAppWindow : Window
    {
        public AndroidAppWindow()
        {
            InitializeComponent();
        }

        private void CB_DontDisplay_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowAndroidAd = this.CB_DontDisplay.IsChecked == false;
            Properties.Settings.Default.Save();
        }
    }
}
