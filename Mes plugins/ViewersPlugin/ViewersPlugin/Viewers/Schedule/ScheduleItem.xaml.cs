using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Agenda_Virtuel;
using Agenda_Virtuel.Manager;

namespace ViewersPlugin.Viewers.Schedule
{
    public partial class ScheduleItem : UserControl, IHomeworkViewer
    {
        //Variables
        private readonly Brush initialBackground;
        private bool isHighlighted;

        //Properties
        public Homework Homework { get; set; }
        public ContextMenu ViewerContextMenu { get; private set; }

        //Constructor
        public ScheduleItem()
        {
            InitializeComponent();

            initialBackground = this.Background;
            ViewerContextMenu = (ContextMenu)this.FindResource("Menu");

            this.LB_Subject.Text = "Matière";
            this.LB_Job.Text = "A faire";
        }

        public void AddPluginMenus(MenuItem[] menus)
        {
            List<MenuItem> itemsToRemove = new List<MenuItem>();

            foreach (MenuItem item in ViewerContextMenu.Items)
            {
                foreach (MenuItem x in menus)
                {
                    if (x.Header == item.Header && x.Tag == item.Tag)
                        itemsToRemove.Add(item);
                }
            }

            itemsToRemove.ForEach((x) => ViewerContextMenu.Items.Remove(x));

            //menu.Items.Add(new Separator());
            foreach (MenuItem item in menus)
            {
                ViewerContextMenu.Items.Add(item);
            }
        }

        public void DeleteHomework()
        {
            HomeworkManager.DeleteHomework(Homework);
        }

        public void Highlight(bool enabled)
        {
            isHighlighted = enabled;

            if (enabled)
                this.Background = SettingsManager.GetSettings().colors.HighlightColor;
            else
                this.Background = initialBackground;
        }

        public void Recolor()
        {
            if (Homework.IsTest)
                this.Background = SettingsManager.GetSettings().colors.TestsColor;
            else
                this.Background = initialBackground;

            Highlight(isHighlighted);

            this.LB_Subject.Foreground = SettingsManager.GetSettings().colors.SubjectsColor;
        }

        public void Refont()
        {
            SettingsManager.GetSettings().fonts.SubjectsFont.ApplyTo(this.LB_Subject);

            if (Homework.IsTest)
                SettingsManager.GetSettings().fonts.NormalHomeworksFont.ApplyTo(this.LB_Job);
            else
                SettingsManager.GetSettings().fonts.TestsFont.ApplyTo(this.LB_Job);
        }

        public void UpdateHomework(Homework _homework)
        {
            Homework = _homework;
            this.LB_Subject.Text = Homework.Subject;
            this.LB_Job.Text = Homework.Job;

            Recolor();
            Refont();
        }

        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteHomework();
        }

        private void BT_Edit_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.BeginChangingHomework(this);
        }

        private void UserControl_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewerContextMenu.IsOpen = true;
        }
    }
}
