using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Agenda_Virtuel;
using Agenda_Virtuel.Manager;

namespace ViewersPlugin.Viewers.Schedule
{
    public partial class SHDViewerItem : UserControl, IHomeworkViewer
    {
        //Variables
        private Brush initialHeaderBackground;
        private readonly SolidColorBrush testBackColor = new SolidColorBrush(Colors.Red);
        private bool isHighlighted;

        //Properties
        public Homework Homework { get; set; }
        public ContextMenu ViewerContextMenu { get; private set; }

        //Constructor
        public SHDViewerItem()
        {
            InitializeComponent();

            initialHeaderBackground = this.Grid_Header.Background;
            isHighlighted = false;
            ViewerContextMenu = (ContextMenu)this.FindResource("Menu");
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
                this.Background = null;
        }

        public void Recolor()
        {
            Highlight(isHighlighted);

            if (Homework.IsTest)
                this.Grid_Header.Background = testBackColor;
            else
                this.Grid_Header.Background = initialHeaderBackground;

            this.LB_Date.Foreground =
                this.LB_Job.Foreground =
                this.LB_Subject.Foreground =
                SettingsManager.GetSettings().colors.NormalHomeworksColor;
        }

        public void Refont()
        {
            SettingsManager.GetSettings().fonts.SubjectsFont.ApplyTo(this.LB_Subject);
            SettingsManager.GetSettings().fonts.SubjectsFont.ApplyTo(this.LB_Date);

            if (Homework.IsTest)
                SettingsManager.GetSettings().fonts.NormalHomeworksFont.ApplyTo(this.LB_Job);
            else
                SettingsManager.GetSettings().fonts.TestsFont.ApplyTo(this.LB_Job);
        }

        public void UpdateHomework(Homework _homework)
        {
            Homework = _homework;

            this.LB_Subject.Content = Homework.Subject;
            this.LB_Job.Text = Homework.Job;
            this.LB_Date.Content = Homework.Date.Value.ToShortDateString();


            Recolor();
            Refont();
        }

        private void BT_More_Click(object sender, RoutedEventArgs e)
        {
            ViewerContextMenu.IsOpen = true;
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
