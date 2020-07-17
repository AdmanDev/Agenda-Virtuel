using System;
using System.Collections.Generic;
using System.Windows;
using Calendar;
using Agenda_Virtuel.Windows.Shedule;
using System.Drawing;
using Agenda_Virtuel.Manager;

namespace Agenda_Virtuel
{
    internal partial class ScheduleWindow : Window
    {
        //Constantes
        private Color subjectsColor = Color.White; //Subjects texts color

        //Variables
        private DayView dayView;
        private List<Calendar.Appointment> caAppointments;
        private string startTime, endTime;
        private int dayIndex;

        //Constructor
        public ScheduleWindow()
        {
            InitializeComponent();

            dayView = new DayView()
            {
                AllowInplaceEditing = false,
                AllowNew = false,
                StartDate = new DateTime(2018, 10, 1),
                HalfHourHeight = 30,
                StartHour = 8,
                WorkingMinuteStart = 0,
                SelectionStart = new DateTime(2018, 10, 1),
                SelectionEnd = new DateTime(2018, 10, 8),
                DaysToShow = 7,
                Font = new Font("Segoe UI", 24)
            };
            dayView.ResolveAppointments += DayView_ResolveAppointments;
            dayView.SelectionChanged += DayView_SelectionChanged;
            this.WFH.Child = dayView;

            startTime = "0830";
            endTime = "0930";
            dayIndex = 0;

            caAppointments = new List<Calendar.Appointment>();

            EventsManager.AppointmentAdded += OnAppointmentAdded;
            EventsManager.AppointmentRemoved += OnAppointmentRemoved;
            EventsManager.DatasDownloaded += OnDatasDownloaded;
            InitializeSchedule();
        }

        //Get appointments and display them
        private void InitializeSchedule()
        {
            caAppointments.Clear();
            Calendar.Appointment app;
            Color sColor;
            for (int i = 0; i < Global.userData.scheduleAppointments.Count; i++)
            {
                sColor = Global.userData.settings.Subjects.GetColorOf(Global.userData.scheduleAppointments[i].Title);

                app = new Calendar.Appointment
                {
                    StartDate = Global.userData.scheduleAppointments[i].StartTime,
                    EndDate = Global.userData.scheduleAppointments[i].EndTime,
                    Title = Global.userData.scheduleAppointments[i].Title,
                    TextColor = subjectsColor,
                    Color = sColor,
                    //Locked = true
                };

                caAppointments.Add(app);
            }

            this.dayView.Invalidate();
        }

        //Show appontments
        private void DayView_ResolveAppointments(object sender, ResolveAppointmentsEventArgs args)
        {
            args.Appointments = caAppointments;
        }

        private void BT_AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            Schedule_AddAppointmentWindow sapw = new Schedule_AddAppointmentWindow("", dayIndex, startTime, endTime);

            if (sapw.ShowDialog() == true)
            {
                ScheduleManager.AddAppointment(sapw.GetAppointment());
            }
        }

        private void OnAppointmentAdded(Appointment appointment)
        {
            Color sColor = Global.userData.settings.Subjects.GetColorOf(appointment.Title);

            Calendar.Appointment capp = new Calendar.Appointment()
            {
                Title = appointment.Title,
                StartDate = appointment.StartTime,
                EndDate = appointment.EndTime,
                TextColor = subjectsColor,
                Color = sColor,
                //Locked = true
            };

            caAppointments.Add(capp);
            dayView.Invalidate();
        }

        private void BT_RemoveAppointment_Click(object sender, RoutedEventArgs e)
        {
            Calendar.Appointment selectedApp = this.dayView.SelectedAppointment;

            if (selectedApp == null)
                return;

            Agenda_Virtuel.Appointment appointment = Global.userData.scheduleAppointments.Find
                (
                x => x.StartTime.CompareTo(selectedApp.StartDate) == 0 &&
                x.EndTime.CompareTo(selectedApp.EndDate) == 0 &&
                x.Title == selectedApp.Title
                );

            if (appointment != null)
            {
                ScheduleManager.RemoveAppointment(appointment);
            }
        }

        private void OnAppointmentRemoved(Appointment appointment)
        {
            Calendar.Appointment appToRemove = caAppointments.Find
                (
                x => x.StartDate.CompareTo(appointment.StartTime) == 0 &&
                x.EndDate.CompareTo(appointment.EndTime) == 0 &&
                x.Title == appointment.Title
                );

            if (appToRemove != null)
            {
                caAppointments.Remove(appToRemove);
                this.dayView.Invalidate();
            }
        }

        private void DayView_SelectionChanged(object sender, EventArgs e)
        {
            dayIndex = this.dayView.SelectionStart.Day - 1;
            startTime = this.dayView.SelectionStart.ToShortTimeString();
            endTime = this.dayView.SelectionEnd.ToShortTimeString();
        }

        private void BT_CustomizeSubjectsColors_Click(object sender, RoutedEventArgs e)
        {
            new CustomizeSubjectsColorsWindow().ShowDialog();
            InitializeSchedule();
        }

        private void OnDatasDownloaded(Save obj)
        {
            InitializeSchedule();
        }
    }
}
