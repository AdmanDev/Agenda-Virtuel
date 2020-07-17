using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Agenda_Virtuel;
using Agenda_Virtuel.Manager;
using ViewersPlugin.Viewers.Schedule;

namespace ViewersPlugin.Controls
{
    public partial class ScheduleDay : UserControl
    {
        //Properties
        public DateTime Date { get; private set; }
        private List<IHomeworkViewer> Viewers { get; set; }

        //Constructors
        public ScheduleDay()
        {
            InitializeComponent();
        }

        public ScheduleDay(DateTime _date)
        {
            InitializeComponent();
            this.Width = Application.Current.MainWindow.Width / ScheduleViewer.DayToShow;

            Date = _date;
            Viewers = new List<IHomeworkViewer>();

            this.LB_Date.Text = _date.ToLongDateString();

            ShowHomeworks();

            EventsManager.HomeworkDeleted += OnHomeworkDeleted;
            EventsManager.HomewokIsReplaced += OnHomewokIsReplaced;
            EventsManager.HomeworkDisplayed += OnHomeworkDisplayed;
            EventsManager.DatasDownloaded += OnDatasDownloaded;

        }

        private void OnDatasDownloaded(Save obj)
        {
            ShowHomeworks();
        }

        private void OnHomeworkDisplayed(IHomeworkViewer homeworkViewerItem)
        {
            if(Date.CompareTo(HomeworkManager.SelectedDate) != 0)
                ShowHomeworks();
        }

        private void ShowHomeworks()
        {
            Clear();

            ScheduleItem si;
            foreach (Homework hm in HomeworkManager.GetHomeworksOf(Date))
            {
                si = new ScheduleItem();
                si.UpdateHomework(hm);
                HomeworkManager.NormalizeViewer(si);
                this.SP_Viewer.Children.Add(si);
                Viewers.Add(si);
            }
        }

        private void OnHomeworkDeleted(Homework homework)
        {
            if (Viewers.Find(x=>x.Homework.Equals(homework)) != null)
            {
                ShowHomeworks();
            }
        }

        private void OnHomewokIsReplaced(Homework oldHomework, Homework newHomework)
        {
            IHomeworkViewer v = Viewers.Find(x => x.Homework.Equals(oldHomework));
            if (v != null)
            {
                v.UpdateHomework(newHomework);
            }
        }

        //Show homework
        public void ShowHomework(IHomeworkViewer homework)
        {
            this.SP_Viewer.Children.Add(homework as UserControl);
            Viewers.Add(homework);
        }

        //Remove homework
        public void RemoveHomework(IHomeworkViewer homework)
        {
            this.SP_Viewer.Children.Remove(homework as UserControl);
            Viewers.Remove(homework);
        }

        //Clear all
        public void Clear()
        {
            this.SP_Viewer.Children.Clear();
            Viewers.Clear();
        }
    }
}
