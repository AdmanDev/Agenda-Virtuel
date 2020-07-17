namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This class allows to get, add, remove and manage user remiders.
    /// </summary>
    public static class RemindersManager
    {
        /*_________________________FUNCTIONS_________________________*/

        /// <summary>
        /// Get reminders of user.
        /// </summary>
        /// <returns>Array of reminders.</returns>
        public static string[] GetUserReminders()
        {
            return Global.userData.reminders.ToArray();
        }

        /// <summary>
        /// Add a reminder to the data.
        /// </summary>
        /// <param name="reminder">The reminder to add.</param>
        /// <param name="save">If true, the reminder will be saved.</param>
        public static void AddReminder(string reminder, bool save = true)
        {
            Global.userData.reminders.Add(reminder);

            EventsManager.Call_ReminderAdded(reminder);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Remove a reminder from the data.
        /// </summary>
        /// <param name="reminder">The reminder to remove.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void RemoveReminder(string reminder, bool save = true)
        {
            if (!Global.userData.reminders.Contains(reminder))
                return;

            Global.userData.reminders.Remove(reminder);

            EventsManager.Call_ReminderRemoved(reminder);

            if (save)
            {
                Save.SaveData();

            }
        }
    }
}
