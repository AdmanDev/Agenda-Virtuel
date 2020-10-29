using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Agenda_Virtuel.Manager;
using MyFunctions;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This is the main window of this application
    /// </summary>
    internal partial class MainWindow : Window
    {
        //Variables
        /// <summary>
        /// The menu that is displayed whenever user click on "others" button
        /// </summary>
        private readonly ContextMenu mainMenu;
        private int mi_PluginIndex;
        /// <summary>
        /// List of sorting modes
        /// </summary>
        private string[] sortingOptions;

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
            Global.mainWindow = this;

            mainMenu = (ContextMenu)this.FindResource("Menu");
            mi_PluginIndex = -1;
            sortingOptions = new string[] { "- Normal", "- Par contrôles", "- Tout afficher" };

            EventsManager.DateChanged += OnDateChanged;
            EventsManager.HomeworkDisplayed += OnHomeworkDisplayed;
            EventsManager.NewHomeworkSaved += OnNewHomeworkSaved;
            EventsManager.HomeworkDeleted += OnHomeworkDeleted;
            EventsManager.BeginChangingHomework += OnBeginModifyingHomework;
            EventsManager.HomewokIsReplaced += OnHomewokIsReplaced;
            EventsManager.DatasDownloaded += OnDatasDownloaded;
            EventsManager.SubjectsListChanged += OnSubjectsListChanged;
            EventsManager.HomeworkEditorChanged += OnHomeworkEditorChanged;
            EventsManager.HomeworkViewerContainerChanged += OnHomeworkViewerChanged;

            this.Calendar.DisplayDateStart = DateTime.Now;
        }

        #region Load

        // On load
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            HomeworkManager.Initialize(this.HmsViewer, this.HEC);
            this.LB_HomeworksCount.Text = HomeworkManager.Count.ToString();

            LoadSortingCBB();

            ShowAndroidAppAd();
        }

        private void ShowAndroidAppAd()
        {
            if (Properties.Settings.Default.ShowAndroidAd)
            {
                Random rnd = new Random();
                int r = rnd.Next(0, 101);
                if (r <= 40)
                {
                    new AndroidAppWindow().Show();
                }
            }
        }

        /// <summary>
        /// Fill sorting modes combobox
        /// </summary>
        private void LoadSortingCBB()
        {
            this.CBB_SortHomeworks.Items.Clear();

            foreach (string op in sortingOptions)
            {
                this.CBB_SortHomeworks.Items.Add(op);
            }

            foreach (Subject subject in Global.userData.settings.Subjects)
            {
                this.CBB_SortHomeworks.Items.Add(subject.Name);
            }
        }

        #endregion

        #region Calendar

        //Show or hide the calenda
        private void BT_Calendar_Click(object sender, RoutedEventArgs e)
        {
            if (this.Calendar.Visibility != Visibility.Visible)
                this.Calendar.Visibility = Visibility.Visible;
            else
                this.Calendar.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Fill subject field with user schedule
        /// </summary>
        private void SetCurrentSubject()
        {
            DateTime now = DateTime.Now;

            Appointment appointment = Global.userData.scheduleAppointments.Find
                (x =>

                   x.StartTime.DayOfWeek == now.DayOfWeek &&
                   x.StartTime.TimeOfDay.CompareTo(now.TimeOfDay) <= 0 &&
                   x.EndTime.TimeOfDay.CompareTo(now.TimeOfDay) > 0

                );

            if (appointment != null)
            {
                HomeworkManager.HomeworkEditor.SetSubject(appointment.Title);
            }
        }

        //User select a new date in the calendar
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Calendar.Visibility = Visibility.Collapsed;

            if (this.Calendar.SelectedDate.HasValue)
                HomeworkManager.SelectedDate = this.Calendar.SelectedDate;
        }

        //Previous date
        private void BT_PreviousDate_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.SelectedDate = HomeworkManager.SelectedDate?.AddDays(-1d);
        }

        //Next date
        private void BT_NextDate_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.SelectedDate = HomeworkManager.SelectedDate?.AddDays(1d);
        }

        #endregion

        #region Homework editor

        /// <summary>
        /// Show homework editor
        /// </summary>
        internal void ShowEditor()
        {
            this.Grid_AddHomework.Visibility = Visibility.Visible;
            SetCurrentSubject();

            EventsManager.Call_OnAddHomeworks_ButtonClick(true);
        }

        private void BT_AddHomework_Click(object sender, RoutedEventArgs e)
        {
            if (this.Grid_AddHomework.Visibility != Visibility.Visible)
            {
                ShowEditor();
            }
            else
            {
                ResetHomeworkEditor();

                EventsManager.Call_OnAddHomeworks_ButtonClick(false);
            }
        }

        /// <summary>
        /// Hide homework editor
        /// </summary>
        private void ResetHomeworkEditor()
        {
            this.Grid_AddHomework.Visibility = Visibility.Collapsed;
            HomeworkManager.HomeworkEditor.Mode = HomeworkEditorMode.Add;
        }

        #endregion

        #region EventsManager

        /// <summary>
        /// Whenever selected date change, reset homework editor and update calandar with new selected date
        /// </summary>
        /// <param name="newDate">New selected date</param>
        private void OnDateChanged(DateTime? newDate)
        {
            ResetHomeworkEditor();

            if (newDate == null || !newDate.HasValue)
                return;

            this.LB_Date.Text = newDate.Value.ToLongDateString().FirstLetterInUppercase();

            if (!this.Calendar.SelectedDate.HasValue || DateTime.Compare(this.Calendar.SelectedDate.Value, newDate.Value) != 0)
            {
                this.Calendar.SelectedDate = newDate;
            }
        }

        /// <summary>
        /// Show new editor if the selected editor has been changed
        /// </summary>
        /// <param name="editor">New editor</param>
        private void OnHomeworkEditorChanged(IHomeworkEditor editor)
        {
            if (editor == null)
                return;

            this.Grid_AddHomework.Children.Clear();
            this.Grid_AddHomework.Children.Add(editor as UserControl);
        }

        /// <summary>
        /// Replace current homework viewer container by the new selected
        /// </summary>
        /// <param name="viewer">New selected viewer container</param>
        private void OnHomeworkViewerChanged(IHomeworkViewerContainer viewer)
        {
            if (viewer == null)
                return;

            this.Grid_HomeworksTable.Children.Clear();
            this.Grid_HomeworksTable.Children.Add(viewer as UserControl);
        }

        /// <summary>
        /// Display homeworks count whenever data is downloaded
        /// </summary>
        /// <param name="save">Data downloaded</param>
        private void OnDatasDownloaded(Save save)
        {
            this.LB_HomeworksCount.Text = save.homeworks.Count.ToString();
        }

        /// <summary>
        /// Whenever homework is displayed, reset homework editor
        /// </summary>
        /// <param name="viewerItem">Displayed homework (viewer)</param>
        private void OnHomeworkDisplayed(IHomeworkViewer viewerItem)
        {
            ResetHomeworkEditor();
        }

        /// <summary>
        /// Whenever a homework is deleted, update homeworks count text
        /// </summary>
        /// <param name="homework">Deleted homework</param>
        private void OnHomeworkDeleted(Homework homework)
        {
            this.LB_HomeworksCount.Text = HomeworkManager.Count.ToString();
        }

        /// <summary>
        /// Whenever a homework is added, update homeworks count text
        /// </summary>
        /// <param name="homework">Added homework</param>
        private void OnNewHomeworkSaved(Homework homework)
        {
            this.LB_HomeworksCount.Text = HomeworkManager.Count.ToString();
        }

        /// <summary>
        /// Whenever a homework is editing, display homework editor
        /// </summary>
        /// <param name="homeworkViewerItem">Editing homework (viewer)</param>
        private void OnBeginModifyingHomework(IHomeworkViewer homeworkViewerItem)
        {
            this.Grid_AddHomework.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Whenever homework is replaced, reset homework editor
        /// </summary>
        /// <param name="oldHomework"></param>
        /// <param name="newHomework"></param>
        private void OnHomewokIsReplaced(Homework oldHomework, Homework newHomework)
        {
            ResetHomeworkEditor();
        }

        /// <summary>
        /// Whenever the subjects list is changed, update sorting combobox
        /// </summary>
        /// <param name="subjects">New subbjects list</param>
        private void OnSubjectsListChanged(List<Subject> subjects)
        {
            LoadSortingCBB();
        }

        #endregion

        #region Menu of "Others" button

        private void BT_Others_Click(object sender, RoutedEventArgs e)
        {
            this.BT_Others.ContextMenu.IsOpen = true;
        }

        private void MI_OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void MI_SchoolGrades_Click(object sender, RoutedEventArgs e)
        {
            new SchoolGradesWindow().Show();
        }

        private void MI_OpenReminders(object sender, RoutedEventArgs e)
        {
            new ReminderWindow().Show();
        }

        private void MI_OpenSchedule(object sender, RoutedEventArgs e)
        {
            new ScheduleWindow().Show();
        }

        private void MN_Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(this.Grid_HomeworksTable, "My homeworks - " + HomeworkManager.SelectedDate.Value.ToShortDateString());
            }
        }

        private void MN_Plugins_Click(object sender, RoutedEventArgs e)
        {
            new PluginManagerWindow().ShowDialog();
        }

        #endregion

        #region Footer

        private void BT_NextHomeworks_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.NextPage();
        }

        private void BT_PreviousHomeworks_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.PreviousPage();
        }

        private void CBB_SortHomeworks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                this.LB_Date.Text = "Afficher : " + e.AddedItems[0];

            string option = "";
            HomeworkSortingMode sortingMode;
            int i = this.CBB_SortHomeworks.SelectedIndex;

            if (i < 0)
                return;

            if (i == 0)
                sortingMode = HomeworkSortingMode.ByDay;
            else if (i == 1)
                sortingMode = HomeworkSortingMode.ByTests;
            else if (i == 2)
                sortingMode = HomeworkSortingMode.DisplayAll;
            else
            {
                sortingMode = HomeworkSortingMode.Other;

                if (e.AddedItems.Count > 0)
                    option = e.AddedItems[0] as string;
            }

            HomeworkManager.SortBy(sortingMode, option);
        }

        private void CBB_SortHomeworks_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.LB_Date.Text = "Afficher : " + this.CBB_SortHomeworks.Text;

            HomeworkManager.SortBy(HomeworkSortingMode.Other, this.CBB_SortHomeworks.Text);
        }


        #endregion

        internal void ShowPluginMainMenus(MenuItem menu)
        {
            if (mi_PluginIndex < 0)
            {
                foreach (MenuItem mi in mainMenu.Items)
                {
                    if (mi.Name == "MI_Plugin")
                    {
                        mi_PluginIndex = mainMenu.Items.IndexOf(mi) + 1;
                        break;
                    }
                }
            }

            mainMenu.Items.Insert(mi_PluginIndex, menu);
        }

    }
}
