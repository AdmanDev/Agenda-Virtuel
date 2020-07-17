using Agenda_Virtuel.Manager;
using System.Collections.Generic;
using System.Windows;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This is reminder window 
    /// </summary>
    internal partial class ReminderWindow : Window
    {
        //Variables
        private string reminderToReplace;

        //Properties
        private List<ReminderViewer> Viewers { get; set; }

        //Constructor
        public ReminderWindow()
        {
            InitializeComponent();

            ShowReminders();

            EventsManager.ReminderAdded += OnReminderAdded;
            EventsManager.ReminderRemoved += OnReminderRemoved;
            EventsManager.DatasDownloaded += OnDatasDownloaded;
        }

        private void ShowReminders()
        {
            Viewers = new List<ReminderViewer>();
            foreach (string r in Global.userData.reminders)
            {
                OnReminderAdded(r);
            }
        }

        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            if (Panel_AddReminder.Visibility != Visibility.Visible)
                Panel_AddReminder.Visibility = Visibility.Visible;
            else
                Panel_AddReminder.Visibility = Visibility.Collapsed;
        }

        private void BT_SaveReminder_Click(object sender, RoutedEventArgs e)
        {
            string newReminder = TB.Text;

            if (!string.IsNullOrEmpty(reminderToReplace))
                RemindersManager.RemoveReminder(reminderToReplace);

            if (!string.IsNullOrEmpty(newReminder))
            {
                RemindersManager.AddReminder(newReminder);
                TB.Text = "";
                Panel_AddReminder.Visibility = Visibility.Collapsed;
            }
        }

        private void OnReminderAdded(string reminder)
        {
            ReminderViewer viewer = new ReminderViewer(reminder, this);
            SP_Reminders.Children.Add(viewer);
            Viewers.Add(viewer);
        }

        private void OnReminderRemoved(string reminder)
        {
            ReminderViewer rvToDelete = Viewers.Find(x => x.Reminder == reminder);
            if (rvToDelete != null)
            {
                SP_Reminders.Children.Remove(rvToDelete);
            }
        }

        private void OnDatasDownloaded(Save s)
        {
            SP_Reminders.Children.Clear();
            ShowReminders();
        }

        internal void EditReminder(string reminder)
        {
            Panel_AddReminder.Visibility = Visibility.Visible;
            TB.Text = reminder;
            reminderToReplace = reminder;
        }

    }
}
