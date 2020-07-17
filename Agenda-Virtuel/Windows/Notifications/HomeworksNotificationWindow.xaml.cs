using System;
using System.Windows;
using Agenda_Virtuel.Manager;

namespace Agenda_Virtuel.Windows.Notifications
{
    internal partial class HomeworksNotificationWindow : Window
    {
        //Constructor
        public HomeworksNotificationWindow(Homework[] _homeworks, IHomeworkViewerContainer _viewer, DateTime _date)
        {
            InitializeComponent();

            //Location
            this.Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - this.Width;
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Height - 40;

            _viewer.Clear();

            if(_homeworks != null && _homeworks.Length > 0)
            {
                //Show homeworks
                this.Grid_Viewer.Children.Clear();
                this.Grid_Viewer.Children.Add(_viewer as UIElement);

                this.LB_Title.Content = "Devoir pour le " + _date.ToLongDateString();

                foreach (Homework h in _homeworks)
                {
                    HomeworkManager.ShowHomework(h);
                }
            }
        }

        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
