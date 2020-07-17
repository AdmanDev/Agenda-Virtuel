using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ViewersPlugin.Controls;
using Agenda_Virtuel;
using Agenda_Virtuel.Manager;
using ViewersPlugin.Viewers.Schedule;

namespace ViewersPlugin
{
    public partial class ScheduleViewer : UserControl, IHomeworkViewerContainer
    {
        //Properties
        public Type ViewerItemType { get; set; }
        private List<ScheduleDay> Days { get; set; }
        public static int DayToShow { get; private set; }

        //Constructor
        public ScheduleViewer()
        {
            InitializeComponent();

            DayToShow = 5;
            Days = new List<ScheduleDay>();
            ViewerItemType = typeof(ScheduleItem);

            //Events
            EventsManager.DateChanged += OnDateChanged;
            EventsManager.NewHomeworkSaved += OnNewHomeworkSaved;
        }

        //Load
        private void OnLoad(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowDays(HomeworkManager.SelectedDate.Value, DayToShow);
        }

        private void OnDateChanged(DateTime? newDate)
        {
            ShowDays(newDate.Value, DayToShow);
        }

        private void OnNewHomeworkSaved(Homework homework)
        {
            ScheduleDay sd = Days.Find(x => x.Date.CompareTo(homework.Date.Value) == 0);

            if (sd != null)
            {
                ScheduleItem si = new ScheduleItem();
                si.UpdateHomework(homework);

                sd.ShowHomework(si);
            }
        }

        private void ShowDays(DateTime firstDate, int _days)
        {
            this.SP_Days.Children.Clear();
            Days.Clear();

            ScheduleDay sd;
            DateTime curDate = new DateTime(firstDate.Year, firstDate.Month, firstDate.Day);
            for (int i = 0; i < _days; i++)
            {
                sd = new ScheduleDay(curDate);
                Days.Add(sd);
                this.SP_Days.Children.Add(sd);

                curDate = curDate.AddDays(1);
            }
        }

        public void Clear()
        {
            ScheduleDay sd = Days.Find(x => x.Date.CompareTo(HomeworkManager.SelectedDate) == 0);

            if (sd != null)
                sd.Clear();
        }

        public void DisplayHomework(IHomeworkViewer _homeworkViewerItem)
        {
            
        }

        public void RemoveViewer(IHomeworkViewer viewer)
        {
            
        }

    }
}
