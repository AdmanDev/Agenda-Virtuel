namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This class allows to add, remove and manage the user schedule.
    /// </summary>
    public static class ScheduleManager
    {
        /*_________________________FUNCTIONS_________________________*/

        /// <summary>
        /// Get appointments of user.
        /// </summary>
        /// <returns>appointments array.</returns>
        public static Appointment[] GetUserSchedule()
        {
            return Global.userData.scheduleAppointments.ToArray();
        }

        /// <summary>
        /// Add appointment in the user schedule.
        /// </summary>
        /// <param name="appointment">The appointment to add.</param>
        /// <param name="save">If true, the appointment will be saved.</param>
        public static void AddAppointment(Appointment appointment, bool save = true)
        {
            Global.userData.scheduleAppointments.Add(appointment);

            EventsManager.Call_AppointmentAdded(appointment);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Remove appointment from the user schedule.
        /// </summary>
        /// <param name="appointment">Appointment to remove.</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void RemoveAppointment(Appointment appointment, bool save = true)
        {
            Global.userData.scheduleAppointments.Remove(appointment);

            EventsManager.Call_AppointmentRemoved(appointment);

            if (save)
            {
                Save.SaveData();

            }
        }

        /// <summary>
        /// Change the color of a subject.
        /// </summary>
        /// <param name="subject">The subject whose color will be set.</param>
        /// <param name="color">The new color of the subject</param>
        /// <param name="save">If true, this change will be saved.</param>
        public static void SetSubjectColor(string subject, System.Drawing.Color color, bool save = true)
        {
            Global.userData.settings.Subjects.SetColorOf(subject, color);

            EventsManager.Call_SubjectColorChanged(subject, color);

            if (save)
            {
                Save.SaveData();

            }
        }
    }
}
