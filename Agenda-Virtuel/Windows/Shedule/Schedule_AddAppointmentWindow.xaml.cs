using Agenda_Virtuel.Manager;
using System;
using System.Windows;
using System.Windows.Forms;

namespace Agenda_Virtuel.Windows.Shedule
{
    internal partial class Schedule_AddAppointmentWindow : Window
    {
        //Variables
        private MaskedTextBox TB_StartTime;
        private MaskedTextBox TB_EndTime;

        private DateTime start, end;

        //Properties
        public new string Title { get; private set; }
        public int DayIndex { get; private set; }
        public DateTime StartTime
        {
            get => start;
            set => start = new DateTime(2018, 10, value.Day, value.Hour, value.Minute, 0);
        }
        public DateTime EndTime
        {
            get => end;
            set => end = new DateTime(2018, 10, value.Day, value.Hour, value.Minute, 0);
        }

        //Constructors
        public Schedule_AddAppointmentWindow()
        {
            InitializeComponent();

            this.TB_StartTime = new MaskedTextBox()
            {
                Font = new System.Drawing.Font("Microsoft Sans Serif", 14),
                TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                Text = "0830",
                Mask = "00:00"
            };

            this.TB_EndTime = new MaskedTextBox()
            {
                Font = new System.Drawing.Font("Microsoft Sans Serif", 14),
                TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                Text = "0930",
                Mask = "00:00"
            };

            this.CBB_Subjects.ItemsSource = Global.userData.settings.SubjectsStrings;
            this.WFH_StartTime.Child = this.TB_StartTime;
            this.WFH_EndTime.Child = this.TB_EndTime;

            EventsManager.SubjectsListChanged += OnSubjectsListChanged;
        }

        public Schedule_AddAppointmentWindow(string _title, int _dayIndex, string _startTime, string _endTime) : this()
        {
            this.CBB_Subjects.Text = _title;
            this.CBB_Days.SelectedIndex = _dayIndex;
            this.TB_StartTime.Text = _startTime;
            this.TB_EndTime.Text = _endTime;
        }

        private void OnSubjectsListChanged(string[] subjects)
        {
            this.CBB_Subjects.ItemsSource = subjects;
        }

        //Cancel --> close this window
        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //Validate
        private void BT_Validate_Click(object sender, RoutedEventArgs e)
        {
            Title = this.CBB_Subjects.Text;
            DayIndex = this.CBB_Days.SelectedIndex + 1;

            try
            {
                StartTime = DateTime.Parse(this.TB_StartTime.Text);
                EndTime = DateTime.Parse(this.TB_EndTime.Text);
            }
            catch
            {
                throw new Exception("Incorrect time");
            }

            if (StartTime == null || EndTime == null)
                return;

            if (StartTime.CompareTo(EndTime) > 0 || string.IsNullOrEmpty(Title))
                return;

            StartTime = new DateTime(2018, 10, DayIndex, StartTime.Hour, StartTime.Minute, 0);
            EndTime = new DateTime(2018, 10, DayIndex, EndTime.Hour, EndTime.Minute, 0);

            this.DialogResult = true;
            this.Close();
        }

        public Appointment GetAppointment()
        {
            return new Appointment(Title, StartTime, EndTime);
        }
    }
}
