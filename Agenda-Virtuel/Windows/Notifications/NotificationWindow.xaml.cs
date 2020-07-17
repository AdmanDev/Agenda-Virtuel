using System;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_Virtuel.Windows.Notifications
{
    internal partial class NotificationWindow : Window
    {
        public NotificationWindow(string _title, UserControl _controlToShow)
        {
            InitializeComponent();

            //Location
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - this.Width;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Height - 40;

            this.LB_Title.Content = _title;

            this.Grid_Viewer.Children.Clear();
            this.Grid_Viewer.Children.Add(_controlToShow);
        }

        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
