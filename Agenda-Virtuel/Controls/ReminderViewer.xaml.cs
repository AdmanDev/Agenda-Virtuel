using Agenda_Virtuel.Manager;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_Virtuel
{
    internal partial class ReminderViewer : UserControl
    {
        //Variables
        private readonly ReminderWindow reminderWindow;

        //Properties
        public string Reminder { get; private set; }

        //Constructor
        public ReminderViewer(string _reminder, ReminderWindow _rw)
        {
            InitializeComponent();

            Reminder = _reminder;
            reminderWindow = _rw;

            this.LB.Text = _reminder;
        }

        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            RemindersManager.RemoveReminder(this.Reminder);
        }

        private void BT_Edit_Click(object sender, RoutedEventArgs e)
        {
            reminderWindow.EditReminder(Reminder);
        }
    }
}
