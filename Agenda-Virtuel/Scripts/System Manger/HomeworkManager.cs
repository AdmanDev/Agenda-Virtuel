using System;
using System.Collections.Generic;

namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This static class allows to manage homeworks data and displayed homeworks.
    /// Add / Remove homewoks -- Display / Hide homework viewers...
    /// </summary>
    public static class HomeworkManager
    {
        //Variables
        /// <summary>
        /// Viwer container to display homeworks viewers
        /// </summary>
        private static IHomeworkViewerContainer viewer;
        /// <summary>
        /// Homework viewer to display homework informations
        /// </summary>
        private static List<IHomeworkViewer> DisplayedHomeworks;
        /// <summary>
        /// Editor to add / edit a homework
        /// </summary>
        private static IHomeworkEditor editor;
        /// <summary>
        /// List of dates with homeworks
        /// </summary>
        private static List<DateTime?> hmsDates;
        /// <summary>
        /// Index of displayed page containing homeworks
        /// </summary>
        private static int indexPage;

        /// <summary>
        /// Date of today.
        /// </summary>
        public readonly static DateTime todayDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        /// <summary>
        /// Date of displayed homeworks
        /// </summary>
        private static DateTime? selectedDate;
        /// <summary>
        /// User's homeworks list
        /// </summary>
        private static List<Homework> homeworksList = new List<Homework>();

        //Properties
        /// <summary>
        /// Get the homework editor control.
        /// </summary>
        /// <remarks>see <see cref="IHomeworkEditor"/></remarks>
        public static IHomeworkEditor HomeworkEditor
        {
            get => editor;
            internal set
            {
                if (editor == null)
                    return;

                editor = value;
                EventsManager.Call_HomeworkEditorChanged(value);
            }
        }

        /// <summary>
        /// Get the homework viewers container control.
        /// </summary>
        /// <remarks>See <see cref="IHomeworkViewerContainer"/></remarks>
        public static IHomeworkViewerContainer ViewerContainer
        {
            get => viewer;
            internal set
            {
                if (viewer == null)
                    return;

                viewer = value;
                EventsManager.Call_HomeworkViewerChanged(value);
            }
        }

        /// <summary>
        /// Get the number of displayed homeworks in the <c>IHomeworkViewerContainer</c> control.
        /// </summary>
        public static int DisplayedCount { get => DisplayedHomeworks.Count; }

        /// <summary>
        /// Get or set the selected date. 
        /// You can change it to show homeworks of the date.
        /// </summary>
        public static DateTime? SelectedDate
        {
            get => selectedDate;
            set
            {
                if (!value.HasValue || DateTime.Compare(value.Value, todayDate) == -1)
                    return;

                selectedDate = new DateTime((int)value?.Year, (int)value?.Month, (int)value?.Day);
                EventsManager.Call_DateChanged(selectedDate);
            }
        }

        /// <summary>
        /// Get the user homeworks list.
        /// </summary>
        public static Homework[] HomeworksList
        {
            get => homeworksList.ToArray();
            private set => homeworksList = new List<Homework>(value);
        }

        /// <summary>
        /// Get the number of homeworks in the data.
        /// </summary>
        public static int Count { get => homeworksList.Count; }

        /// <summary>
        /// Index of displayed page containing homeworks
        /// </summary>
        private static int IndexPage
        {
            get => indexPage;
            set
            {
                if (value < 0)
                {
                    indexPage = hmsDates.Count - 1;
                    return;
                }

                if (value >= hmsDates.Count)
                {
                    indexPage = 0;
                    return;
                }

                indexPage = value;
            }
        }

        //________________________________FUNCTION________________________________//

        /// <summary>
        /// Initialise HomeworkManager
        /// </summary>
        /// <param name="_viewer">ViewerContainer to use to display homework viewers</param>
        /// <param name="_editor"></param>
        internal static void Initialize(IHomeworkViewerContainer _viewer, IHomeworkEditor _editor)
        {
            viewer = _viewer;
            editor = _editor;
            DisplayedHomeworks = new List<IHomeworkViewer>();

            EventsManager.DateChanged += OnDateChanged;
            EventsManager.DatasDownloaded += OnDatasDownloaded;
            EventsManager.HomeworkDisplayed += OnHomeworkDisplayed;
            EventsManager.OnPluginStarted += OnPluginStarted;
            EventsManager.HomeworkViewerContainerChanged += OnHomeworkViewerChanged;

            homeworksList = Global.userData.homeworks;
            DeleteOldHomeworks();
            InitializeHmsDates();

            SelectedDate = todayDate;
        }


        #region Homeworks pages

        /// <summary>
        /// Initialize hmsDates variable with all dates with homeworks
        /// </summary>
        private static void InitializeHmsDates()
        {
            hmsDates = new List<DateTime?>();
            IndexPage = 0;

            foreach (Homework h in HomeworksList)
            {
                AddDate(h.Date);
            }

        }

        /// <summary>
        /// Set the selected date and show homeworks. 
        /// </summary>
        /// <param name="date">Date to show.</param>
        public static void ChangeDate(DateTime? date)
        {
            SelectedDate = date;
        }

        /// <summary>
        /// Add date with homework
        /// </summary>
        /// <param name="date">Date on which there are homeworks</param>
        private static void AddDate(DateTime? date)
        {
            if (!hmsDates.Contains(date))
                hmsDates.Add(date);

            hmsDates.Sort();
        }

        /// <summary>
        /// Remove date when there is no more homework
        /// </summary>
        /// <param name="date">Date value</param>
        private static void RemoveDate(DateTime? date)
        {
            if (homeworksList.Find(x => x.Date.Value.CompareTo(date.Value) == 0) == null)
                hmsDates.Remove(date);
        }

        /// <summary>
        /// Show next page on which there is homework
        /// </summary>
        internal static void NextPage()
        {
            IndexPage++;

            if (hmsDates.Count > 0)
                SelectedDate = hmsDates[indexPage];
        }

        /// <summary>
        /// Show previous page on which there is homework
        /// </summary>
        internal static void PreviousPage()
        {
            IndexPage--;

            if (hmsDates.Count > 0)
                SelectedDate = hmsDates[indexPage];
        }

        #endregion

        #region Events

        /// <summary>
        /// Triggered whenever user data ise downloaded. Open downloaded homeworks
        /// </summary>
        /// <param name="save">Downloaded data</param>
        private static void OnDatasDownloaded(Save save)
        {
            Global.userData = save;
            homeworksList = save.homeworks;
            DeleteOldHomeworks();

            ShowHomeworksOfSelectedDate();
        }

        /// <summary>
        /// Triggered whenever date is changed. Display homeworks of selected date
        /// </summary>
        /// <param name="newDate">New selected date</param>
        private static void OnDateChanged(DateTime? newDate)
        {
            ShowHomeworksOfSelectedDate();
        }

        /// <summary>
        /// Triggered whenever homework is displayed
        /// </summary>
        /// <param name="vi">Displayed homework viewer</param>
        private static void OnHomeworkDisplayed(IHomeworkViewer vi)
        {
            NormalizeViewer(vi);
        }

        /// <summary>
        /// Triggered whenever plugin is started. Add menus of plugin
        /// </summary>
        /// <param name="plugin">Started plugin</param>
        private static void OnPluginStarted(Plugin.PluginLoader plugin)
        {
            foreach (IHomeworkViewer viewer in DisplayedHomeworks)
            {
                viewer.AddPluginMenus(Plugin.PluginManager.GetHomewoksMenus(viewer).ToArray());
            }
        }

        /// <summary>
        /// Triggered whenever homework viewers container change
        /// </summary>
        /// <param name="viewer">New viewers container</param>
        private static void OnHomeworkViewerChanged(IHomeworkViewerContainer viewer)
        {
            Clear();
            ShowHomeworksOfSelectedDate();
        }

        #endregion

        #region Homework

        /// <summary>
        /// Delete old homeworks (before today)
        /// </summary>
        private static void DeleteOldHomeworks()
        {
            List<Homework> oldHomeworks = new List<Homework>();
            homeworksList.ForEach(
                x =>
                {
                    if (x.Date.Value.CompareTo(todayDate) < 0)
                        oldHomeworks.Add(x);
                });

            oldHomeworks.ForEach(
                x =>
                {
                    homeworksList.Remove(x);
                });

            if (oldHomeworks.Count > 0)
                Save.SaveData();
        }

        /// <summary>
        /// Get homeworks by date.
        /// </summary>
        /// <param name="_date">Date of homeworks</param>
        /// <returns><c>Homework</c>s list.</returns>
        public static Homework[] GetHomeworksOf(DateTime _date)
        {
            return homeworksList.FindAll(x =>
            {
                return DateTime.Compare(x.Date.Value, _date) == 0;
            }).ToArray();
        }

        /// <summary>
        /// Show homework editor.
        /// </summary>
        public static void ShowHomeworkEditor()
        {
            Global.mainWindow.ShowEditor();
        }

        #endregion

        #region Show / Hide homework        

        /// <summary>
        /// Save in the data and show a <c>Homework</c>.
        /// </summary>
        /// <param name="homework"><c>Homework</c> to save and show.</param>
        public static void SaveAndShow(Homework homework)
        {
            ShowHomework(homework);
            SaveHomework(homework);
        }

        /// <summary>
        /// Save in the data and show a list of homeworks.
        /// </summary>
        /// <param name="homeworks">List of <c>Homework</c> to save and show.</param>
        public static void SaveAndShow(IEnumerable<Homework> homeworks)
        {
            foreach (Homework h in homeworks)
            {
                SaveAndShow(h);
            }
        }

        /// <summary>
        /// Show homeworks by the selected date.
        /// </summary>
        public static void ShowHomeworksOfSelectedDate()
        {
            Clear();

            List<Homework> _SHomeworks = new List<Homework>(GetHomeworksOf(SelectedDate.Value));
            ShowHomework(_SHomeworks);
        }

        /// <summary>
        /// Show a homework in the viewer (<c>IHomeworkViewerContainer</c>).
        /// </summary>
        /// <param name="homework"><c>Homework</c> to show.</param>
        public static void ShowHomework(Homework homework)
        {
            IHomeworkViewer item = viewer.ViewerItemType.GetConstructor(new Type[0]).Invoke(new object[0]) as IHomeworkViewer;
            item.UpdateHomework(homework);

            ShowHomework(item);
        }

        /// <summary>
        /// Show a list of homeworks.
        /// </summary>
        /// <param name="homeworksList">The homewoks list to show.</param>
        public static void ShowHomework(IEnumerable<Homework> homeworksList)
        {
            foreach (Homework h in homeworksList)
            {
                ShowHomework(h);
            }
        }

        /// <summary>
        /// Show a homework viewer.
        /// </summary>
        /// <param name="viewerItem"><c>IHomeworkViewer</c> to show.</param>
        public static void ShowHomework(IHomeworkViewer viewerItem)
        {
            viewer.DisplayHomework(viewerItem);
            DisplayedHomeworks.Add(viewerItem);

            EventsManager.Call_HomeworkDisplayed(viewerItem);
            EventsManager.Call_HomeworkViewerItemsListChanged();
        }

        /// <summary>
        /// Remove all displayed homework viewers.
        /// </summary>
        public static void Clear()
        {
            viewer.Clear();
            DisplayedHomeworks.Clear();

            EventsManager.Call_HomeworkViewerCleared();
            EventsManager.Call_HomeworkViewerItemsListChanged();
        }

        /// <summary>
        /// Remove a displayed homework viewer.
        /// </summary>
        /// <param name="viewerItem"><c>IHomeworkViewer</c> to remove.</param>
        public static void RemoveHomeworkViewerItem(IHomeworkViewer viewerItem)
        {
            viewer.RemoveViewer(viewerItem);
            DisplayedHomeworks.Remove(viewerItem);

            EventsManager.Call_HomeworkHidden(viewerItem);
            EventsManager.Call_HomeworkViewerItemsListChanged();
        }

        /// <summary>
        /// Remove a displayed viewer of a homework.
        /// </summary>
        /// <param name="homework"><c>Homework</c> whose viewer will be removed.</param>
        public static void RemoveHomeworkViewerOf(Homework homework)
        {
            IHomeworkViewer viewerItem = GetViewerItemOf(homework);
            if (viewerItem != null)
                RemoveHomeworkViewerItem(viewerItem);
        }

        /// <summary>
        /// Add a <c>Homework</c> to the data and save it.
        /// </summary>
        /// <param name="homework"><c>Homework</c> to save.</param>
        public static void SaveHomework(Homework homework)
        {
            homeworksList.Add(homework);
            AddDate(homework.Date);

            Save.SaveData();

            EventsManager.Call_HomeworkSaved(homework);
        }

        /// <summary>
        /// Add a list of <c>Homework</c> to the data and save it.
        /// </summary>
        /// <param name="homeworks">The <c>Homework</c> list to save.</param>
        public static void SaveHomeworks(IEnumerable<Homework> homeworks)
        {
            foreach (Homework h in homeworks)
            {
                SaveHomework(h);
            }
        }

        /// <summary>
        /// Remove a <c>Homework</c> from the data .
        /// </summary>
        /// <param name="homework"><c>Homework</c> to remove.</param>
        public static void DeleteHomework(Homework homework)
        {
            homeworksList.Remove(homework);
            RemoveDate(homework.Date);

            Save.SaveData();

            EventsManager.Call_HomeworkDeleted(homework);

            RemoveHomeworkViewerOf(homework);
        }

        #endregion

        #region Change homework

        /// <summary>
        /// Show homework editor to change a <c>Homework</c>.
        /// </summary>
        /// <param name="viewerOfHomework"><c>IHomeworkViewer</c> whose <c>Homework</c> will be changed.</param>
        public static void BeginChangingHomework(IHomeworkViewer viewerOfHomework)
        {
            editor.BeginChangeHomework(viewerOfHomework.Homework);
            editor.Mode = HomeworkEditorMode.Change;

            EventsManager.Call_BeginModifyingHomework(viewerOfHomework);
        }

        /// <summary>
        /// Replace a <c>Homework</c> by another
        /// </summary>
        /// <param name="oldHomework"><c>Homework</c> to replace.</param>
        /// <param name="newHomework">The new <c>Homework</c>.</param>
        public static void ReplaceHomework(Homework oldHomework, Homework newHomework)
        {
            int i = homeworksList.FindIndex(x => x.Equals(oldHomework));

            if (i < 0)
                return;

            homeworksList[i] = newHomework;

            IHomeworkViewer vi = DisplayedHomeworks.Find(x => x.Homework.Equals(oldHomework));
            if (vi != null)
            {
                vi.UpdateHomework(newHomework);
                editor.Mode = HomeworkEditorMode.Add;
            }

            Save.SaveData();

            EventsManager.Call_HomewokIsReplaced(oldHomework, newHomework);
        }

        #endregion

        #region Viewers

        /// <summary>
        /// Update colors of displayed <c>IHomeworkViewer</c>.
        /// </summary>
        public static void RecolorViewers()
        {
            DisplayedHomeworks.ForEach(
                x =>
                {
                    x.Recolor();
                }
                );
        }

        /// <summary>
        /// Update fonts of displayed <c>IHomeworkViewer</c>.
        /// </summary>
        public static void RefontViewers()
        {
            DisplayedHomeworks.ForEach(
                x =>
                {
                    x.Refont();
                }
                );
        }

        /// <summary>
        /// Get a <c>IHomeworkViewer</c> by a homework.
        /// </summary>
        /// <param name="homework"><c>Homework</c> whose viewer will be returned.</param>
        /// <returns><c>IHomeworkViewer</c> of the homework.</returns>
        public static IHomeworkViewer GetViewerItemOf(Homework homework)
        {
            return DisplayedHomeworks.Find(x => x.Homework.Equals(homework));
        }

        /// <summary>
        /// Highlight a viewer / homework.
        /// </summary>
        /// <param name="viewerItem"><c>IHomeworkViewer</c> to highlight.</param>
        /// <param name="highlight">True to highlight otherwise False.</param>
        public static void HighlightHomework(IHomeworkViewer viewerItem, bool highlight)
        {
            viewerItem.Highlight(highlight);
        }

        /// <summary>
        /// Highlight a viewer / homework.
        /// </summary>
        /// <param name="homework"><c>Homework</c> to highlight.</param>
        /// <param name="highlight">True to highlight otherwise False.</param>
        public static void HighlightHomework(Homework homework, bool highlight)
        {
            IHomeworkViewer viewerItem = GetViewerItemOf(homework);
            if (viewerItem != null)
                viewerItem.Highlight(highlight);
        }

        /// <summary>
        /// Add plugins menus to your <c>IHomeworkViewer</c>.
        /// </summary>
        /// <param name="vi"><c>IHomeworkViewer</c> to normalize.</param>
        public static void NormalizeViewer(IHomeworkViewer vi)
        {
            vi.AddPluginMenus(Plugin.PluginManager.GetHomewoksMenus(vi).ToArray());
        }

        /// <summary>
        /// Set homework viewers container
        /// </summary>
        /// <param name="container">New container</param>
        internal static void SetViewerContainer(IHomeworkViewerContainer container)
        {
            Plugin.Plugin p = Plugin.PluginManager.GetPluginByObject(container);
            if (p != null)
            {
                if (p.Loader.isHmsViewer)
                    ViewerContainer = container;
            }
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sort homeworks.
        /// </summary>
        /// <param name="sortingMode">The sorting mode. see <see cref="HomeworkSortingMode"/>.</param>
        /// <param name="args">arguments used when the sorting mode is set to "Other". Exemple : "mathematics".</param>
        public static void SortBy(HomeworkSortingMode sortingMode, string args)
        {
            Clear();

            switch (sortingMode)
            {
                case HomeworkSortingMode.ByDay:
                    SelectedDate = selectedDate;
                    break;

                case HomeworkSortingMode.ByTests:
                    List<Homework> tests = homeworksList.FindAll(x => x.IsTest);
                    ShowHomework(tests);
                    break;

                case HomeworkSortingMode.DisplayAll:
                    ShowHomework(homeworksList);
                    break;

                case HomeworkSortingMode.Other:
                    args = args.ToLower();
                    List<Homework> homSub = homeworksList.FindAll(x => x.Subject.ToLower().StartsWith(args));
                    ShowHomework(homSub);
                    break;
            }

            EventsManager.Call_SortingModeChanged(sortingMode, args);

        }

        #endregion
    }
}
