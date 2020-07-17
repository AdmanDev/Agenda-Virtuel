using Agenda_Virtuel.Manager;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agenda_Virtuel
{
    /// <summary>
    /// It is the default <c>IHomeworkViewer</c> that allows to display a homework.
    /// This viewer is displayed on the <c>HomeworksViewerContainer</c> control.
    /// You can create your own <c>IHomeworkViewer</c> if you create your own <c>IHomeworkViewerContainer</c>.
    /// </summary>
    public partial class HomeworkViewerItem : UserControl, IHomeworkViewer
    {
        //Variables
        private ColorsSettings colors;
        private FontsSettings fonts;
        private Homework homework;
        private ContextMenu menu;
        private bool highlighted;

        //Properties
        /// <summary>
        /// This is the <c>Homework</c> displayed by the viewer.
        /// </summary>
        public Homework Homework => homework;
        /// <summary>
        /// This is the menu displayed when user makes a right click.
        /// </summary>
        public ContextMenu ViewerContextMenu => menu;

        //Constructor
        public HomeworkViewerItem()
        {
            InitializeComponent();

            colors = Global.userData.settings.colors;
            fonts = Global.userData.settings.fonts;

            menu = this.FindResource("Menu") as ContextMenu;
            highlighted = false;

            EventsManager.ColorsSettingsChanged += OnColorsSettingsChanged;
            EventsManager.FontsSettingsChanged += OnFontsSettingsChanged;
        }       

        /// <summary>
        /// Update the homework to display.
        /// </summary>
        /// <param name="_homework"><c>Homework</c> to display.</param>
        public void UpdateHomework(Homework _homework)
        {
            this.homework = _homework;

            this.LB_Subject.Text = _homework.Subject;
            this.LB_Job.Text = _homework.Job;

            Refont();
            Recolor();
        }

        private void BT_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DeleteHomework();
        }

        /// <summary>
        /// Delete from user data, the <c>Homework</c> diplayed in the viewer.
        /// </summary>
        public void DeleteHomework()
        {
            HomeworkManager.DeleteHomework(this.Homework);
        }

        private void BT_Highlight_Click(object sender, System.Windows.RoutedEventArgs e)
        {
             Highlight(!highlighted);
        }

        /// <summary>
        /// Highlight this viewer / homework.
        /// </summary>
        /// <param name="enable">True if highlight else false.</param>
        public void Highlight(bool enable)
        {
            highlighted = enable;

            if (enable)
            {
                this.MainGrid.Background = colors.HighlightColor;
            }
            else
            {
                this.MainGrid.Background = null;
            }

            EventsManager.Call_HomeworkHighlighted_Event(this, enable);
        }

        private void LB_Job_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HomeworkManager.BeginChangingHomework(this);
        }

        private void B_Job_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            menu.IsOpen = true;
        }

        private void IMG_More_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            menu.IsOpen = true;
        }

        private void MI_Change_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HomeworkManager.BeginChangingHomework(this);
        }

        private void MI_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DeleteHomework();
        }

        /// <summary>
        /// Update colors of this viewer.
        /// </summary>
        public void Recolor()
        {
            //LB_Job color
            Brush jobColor;
            if (this.Homework.IsTest)
                jobColor = colors.TestsColor;
            else
                jobColor = colors.NormalHomeworksColor;

            this.LB_Job.Foreground = jobColor;
            
            //LB_Subject
            this.LB_Subject.Foreground =colors.SubjectsColor;

            //Higlight
            if (highlighted)
                this.MainGrid.Background = colors.HighlightColor;
        }

        private void OnColorsSettingsChanged(ColorsSettings newColors)
        {
            colors = newColors;
        }

        /// <summary>
        /// Update fonts of this viewer.
        /// </summary>
        public void Refont()
        {
            if(!this.Homework.IsTest)
                fonts.NormalHomeworksFont.ApplyTo(this.LB_Job);
            else
                fonts.TestsFont.ApplyTo(this.LB_Job);

            fonts.SubjectsFont.ApplyTo(this.LB_Subject);
        }

        private void OnFontsSettingsChanged(FontsSettings newFonts)
        {
            fonts = newFonts;
        }

        public void AddPluginMenus(MenuItem[] mis)
        {
            List<MenuItem> itemsToRemove = new List<MenuItem>();

            foreach (MenuItem item in menu.Items)
            {
                foreach (MenuItem x in mis)
                {
                    if (x.Header == item.Header && x.Tag == item.Tag)
                        itemsToRemove.Add(item);
                }
            }

            itemsToRemove.ForEach((x) => menu.Items.Remove(x));

            //menu.Items.Add(new Separator());
            foreach (MenuItem item in mis)
            {
                menu.Items.Add(item);
            }
        }
    }
}
