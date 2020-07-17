using System;
using System.Windows;
using System.Windows.Controls;
using Agenda_Virtuel.Manager;

namespace ViewersPlugin
{
    public partial class SettingsUC : UserControl
    {
        //SETTINGS KEYS
        public const string VIEWER_KEY = "VIEWER";

        //Constructor
        public SettingsUC()
        {
            InitializeComponent();
        }

        //On Load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string curView = Main.instance.HomeworkViewerContainerType.Name;
            switch (curView)
            {
                case nameof(SHDViewer):
                    this.RB_SHD.IsChecked = true;
                    break;

                case nameof(ScheduleViewer):
                    this.RB_Schedule.IsChecked = true;
                    break;
            }
        }

        private void RB_SHD_Click(object sender, RoutedEventArgs e)
        {
            Main.instance.SetSetting(VIEWER_KEY, nameof(SHDViewer));
            Main.instance.HomeworkViewerContainerType = typeof(SHDViewer);
        }

        private void RB_Schedule_Click(object sender, RoutedEventArgs e)
        {
            Main.instance.SetSetting(VIEWER_KEY, nameof(ScheduleViewer));
            Main.instance.HomeworkViewerContainerType = typeof(ScheduleViewer);
        }

    }
}
