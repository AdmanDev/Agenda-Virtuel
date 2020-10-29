using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This static class manages all events of the application.
    /// </summary>
    public static class EventsManager
    {
        /*______________________________________DELEGATES______________________________________*/
        public delegate void DateChangedDelegate(DateTime? newDate);
        public delegate void HomeworkDelegate(Homework homework);
        public delegate void HomeworksListDelegate(List<Homework> homeworksList);
        public delegate void DoubleHomeworkDelegate(Homework oldHomework, Homework newHomework);
        public delegate void ViewerItemDelegate(IHomeworkViewer homeworkViewerItem);
        public delegate void HomeworkHighlightedDelegate(IHomeworkViewer viewer, bool enabled);
        public delegate void SortingModeChanged(HomeworkSortingMode sortingMode, string args);

        public delegate void ImageSourceDelegate(ImageSource image);
        public delegate void BrushDelegate(Brush brush);
        public delegate void BrushChangedDelegate(ColorTarget target, Brush brush);
        public delegate void ColorsSettingsDelegate(ColorsSettings newColors);
        public delegate void FontChangedDelegate(FontTarget target, FontGroup font);
        public delegate void FontsSettingsDelegate(FontsSettings newFonts);
        public delegate void SubjectsListDelegate(List<Subject> subjectsList);
        public delegate void SubjectDelegate(Subject subject);
        public delegate void ShortcutWordsChangedDelegate(string[] words);

        public delegate void SubjectGradeDelegate(Subject subject, Grade grade);
        public delegate void SchoolGradeSubjectDelegate(Subject subject);
        public delegate void NewTrimesterDelegate(SchoolGrades oldGrades);

        public delegate void ReminderDelegate(string reminder);

        public delegate void AppointmentDelegate(Appointment appointment);
        public delegate void SubjectColorChangedDelegate(Subject subject, System.Drawing.Color color);


        /*______________________________________EVENTS______________________________________*/

        //System
        /// <summary>
        /// If user saves his data in the server, this event is triggered whenever his data is downloaded.
        /// </summary>
        public static event Action<Save> DatasDownloaded;

        //Homeworks manager events
        /// <summary>
        /// This event is triggered whenever the selected date is changed.
        /// </summary>
        public static event DateChangedDelegate DateChanged;
        /// <summary>
        /// This event is triggered whenever a homework viewer is displayed.
        /// </summary>
        public static event ViewerItemDelegate HomeworkDisplayed;
        /// <summary>
        /// This event is triggered whenever a homework viewer is hidden.
        /// </summary>
        public static event ViewerItemDelegate HomeworkHidden;
        /// <summary>
        /// This event is triggered whenever user edits a homework.
        /// </summary>
        public static event ViewerItemDelegate BeginChangingHomework;
        /// <summary>
        /// This event is triggered whenever a homework is highlighted.
        /// </summary>
        public static event HomeworkHighlightedDelegate HomeworkHighlighted;
        /// <summary>
        /// This event is triggered whenever the viewer container is cleared.
        /// </summary>
        public static event Action HomeworkViewerContainerCleared;
        /// <summary>
        /// This event is triggered whenever a <c>IHomeworkViewer</c> is displayed, hidden or cleared.
        /// </summary>
        public static event Action HomeworkViewerItemsListChanged;
        /// <summary>
        /// This event is triggered whenever new homework is added.
        /// </summary>
        public static event HomeworkDelegate NewHomeworkSaved;
        /// <summary>
        /// This event is triggered whenever a <c>Homework</c> is removed.
        /// </summary>
        public static event HomeworkDelegate HomeworkDeleted;
        /// <summary>
        /// This event is triggered whenever a <c>Homework</c> is replaced by another.
        /// </summary>
        public static event DoubleHomeworkDelegate HomewokIsReplaced;
        /// <summary>
        /// This event is triggered whenever a <c>Homework</c> is added, removed or replaced from the homeworks list.
        /// </summary>
        public static event HomeworksListDelegate HomeworksListChanged;
        /// <summary>
        /// This event is triggered whenever a <c>Homework</c> is adding, removing or replacing from the homeworks list.
        /// </summary>
        public static event HomeworksListDelegate HomeworksListChanging;
        /// <summary>
        /// This event is triggered whenever the <c>IHomeworkEditor</c> is changed.
        /// </summary>
        public static event Action<IHomeworkEditor> HomeworkEditorChanged;
        /// <summary>
        /// This event is triggered whenever the <c>IHomeworkViewerContainer</c> is changed.
        /// </summary>
        public static event Action<IHomeworkViewerContainer> HomeworkViewerContainerChanged;

        //Settings manager events
        /// <summary>
        /// This event is triggered whenever the application windows background is changed.
        /// </summary>
        public static event BrushDelegate WinBackgroundChanged;
        /// <summary>
        /// This event is triggered whenever the application styles is changed.
        /// </summary>
        public static event Action<Styles> StylesChanged;
        /// <summary>
        /// This event is triggered whenever a homework text color is changed.
        /// </summary>
        public static event BrushChangedDelegate ColorChanged;
        /// <summary>
        /// This event is triggered whenever colors settings is changed.
        /// </summary>
        public static event ColorsSettingsDelegate ColorsSettingsChanged;
        /// <summary>
        /// This event is triggered whenever font of homework text is changed.
        /// </summary>
        public static event FontChangedDelegate FontChanged;
        /// <summary>
        /// This event is triggered whenever fonts settings is changed.
        /// </summary>
        public static event FontsSettingsDelegate FontsSettingsChanged;
        /// <summary>
        /// This event is triggered whenever a subject is added.
        /// </summary>
        public static event SubjectDelegate SubjectAdded;
        /// <summary>
        /// This event is triggered whenever a subject is deleted.
        /// </summary>
        public static event SubjectDelegate SubjectDeleted;
        /// <summary>
        /// This event is triggered whenever subjects list is changed.
        /// </summary>
        public static event SubjectsListDelegate SubjectsListChanged;
        /// <summary>
        /// This event is triggered whenever the list of shortcut words is changed.
        /// </summary>
        public static event ShortcutWordsChangedDelegate ShortcutWordsChanged;

        //School grade manager events
        /// <summary>
        /// This event is triggered whenever a school grade is added.
        /// </summary>
        public static event SubjectGradeDelegate SchoolGradeAdded;
        /// <summary>
        /// This event is triggered whenever a school grade is removed.
        /// </summary>
        public static event SubjectGradeDelegate SchoolGradeRemoved;
        /// <summary>
        /// This event is triggered whenever user (or other) clicks on the "new trimester" button.
        /// </summary>
        public static event NewTrimesterDelegate NewTrimesterEvent;

        //Reminder manager events
        /// <summary>
        /// This event is triggered whenever a reminder is added to the data.
        /// </summary>
        public static event ReminderDelegate ReminderAdded;
        /// <summary>
        /// This event is triggered whenever a reminder is removed from the data.
        /// </summary>
        public static event ReminderDelegate ReminderRemoved;

        //Schedule manager events
        /// <summary>
        /// This event is triggered whenever an appointment is added to the user schedule.
        /// </summary>
        public static event AppointmentDelegate AppointmentAdded;
        /// <summary>
        /// This event is triggered whenever an appointment is removed from the user schedule.
        /// </summary>
        public static event AppointmentDelegate AppointmentRemoved;
        /// <summary>
        /// This event is triggered whenever a subject color is changed.
        /// </summary>
        public static event SubjectColorChangedDelegate SubjectColorChanged;

        //MainWindow events
        /// <summary>
        /// This event is triggered whenever user clicks on the "Add hommework" button in the main window.
        /// </summary>
        public static event Action<bool> OnAddHomeworks_ButtonClick;
        /// <summary>
        /// This event is triggered whenever user is writing a homework, in the <c>IHomeworkEditor</c>.
        /// </summary>
        public static event HomeworkDelegate OnHomeworkWriting;
        /// <summary>
        /// This event is triggered whenever clicks on a "shortcut word" button.
        /// </summary>
        public static event Action<Button, string> OnWordShorcut_ButtonClick;
        /// <summary>
        /// This event is triggered whenever the homeworks sorting mode changed
        /// <see cref="HomeworkSortingMode"/>
        /// </summary>
        public static event SortingModeChanged OnSortingModeChanged;

        //Plugin manager
        /// <summary>
        /// This event is triggered whenever a plugin start
        /// </summary>
        internal static event Action<Plugin.PluginLoader> OnPluginStarted;
        /// <summary>
        /// This event is triggered whenever a plugin is installed
        /// </summary>
        internal static event Action<Plugin.PluginLoader> OnPluginInstalled;

        /*______________________________________Functions______________________________________*/


        /*______________________________________CALL EVENTS______________________________________*/

        #region CALL EVENTS

        #region System events

        /// <summary>
        /// Invoke DatasDownloaded event
        /// </summary>
        /// <param name="save">Save downloaded</param>
        internal static void Call_DatasDownloaded(Save save)
        {
            DatasDownloaded?.Invoke(save);
        }

        #endregion

        #region Homework manager
        /// <summary>
        /// Invoke DateChanged event
        /// </summary>
        /// <param name="newDate">New selected date </param>
        internal static void Call_DateChanged(DateTime? newDate)
        {
            DateChanged?.Invoke(newDate);
        }

        /// <summary>
        /// Invoke HomeworkDisplayed event
        /// </summary>
        /// <param name="homeworkViewerItem">Displayed homework viwer</param>
        internal static void Call_HomeworkDisplayed(IHomeworkViewer homeworkViewerItem)
        {
            HomeworkDisplayed?.Invoke(homeworkViewerItem);
        }

        /// <summary>
        /// Invoke HomeworkHidden event
        /// </summary>
        /// <param name="homeworkViewerItem">Hidden homework viwer</param>
        internal static void Call_HomeworkHidden(IHomeworkViewer homeworkViewerItem)
        {
            HomeworkHidden?.Invoke(homeworkViewerItem);
        }

        /// <summary>
        /// Invoke BeginChangingHomework event
        /// </summary>
        /// <param name="homeworkViewerItem">HomeworkViewer whose homework will be edited</param>
        internal static void Call_BeginModifyingHomework(IHomeworkViewer homeworkViewerItem)
        {
            BeginChangingHomework?.Invoke(homeworkViewerItem);
        }

        /// <summary>
        /// Execute this method if your <c>IHomeworkViewer</c> is highlighted.
        /// </summary>
        /// <param name="viewer">The homework viewer that is highlighted.</param>
        /// <param name="enabled">True if is highlighted else False</param>
        public static void Call_HomeworkHighlighted_Event(IHomeworkViewer viewer, bool enabled)
        {
            HomeworkHighlighted?.Invoke(viewer, enabled);
        }

        /// <summary>
        /// Invoke HomeworkViewerContainerCleared event
        /// </summary>
        internal static void Call_HomeworkViewerCleared()
        {
            HomeworkViewerContainerCleared?.Invoke();
        }

        /// <summary>
        /// Invoke HomeworkViewerItemsListChanged event
        /// </summary>
        internal static void Call_HomeworkViewerItemsListChanged()
        {
            HomeworkViewerItemsListChanged?.Invoke();
        }

        /// <summary>
        /// Invoke HomeworksListChanging, NewHomeworkSaved and HomeworksListChanged events
        /// </summary>
        /// <param name="homework">Added homework</param>
        internal static void Call_HomeworkSaved(Homework homework)
        {
            HomeworksListChanging?.Invoke(Global.userData.homeworks);
            NewHomeworkSaved?.Invoke(homework);
            HomeworksListChanged?.Invoke(Global.userData.homeworks);
        }

        /// <summary>
        /// Invoke HomeworksListChanging, HomeworkDeleted and HomeworksListChanged events
        /// </summary>
        /// <param name="homework">Deleted homework</param>
        internal static void Call_HomeworkDeleted(Homework homework)
        {
            HomeworksListChanging?.Invoke(Global.userData.homeworks);
            HomeworkDeleted?.Invoke(homework);
            HomeworksListChanged?.Invoke(Global.userData.homeworks);
        }

        /// <summary>
        /// Invoke HomeworksListChanging, HomewokIsReplaced and HomeworksListChanged events
        /// </summary>
        /// <param name="oldH">Old homework</param>
        /// <param name="newH">New homework</param>
        internal static void Call_HomewokIsReplaced(Homework oldH, Homework newH)
        {
            HomeworksListChanging?.Invoke(Global.userData.homeworks);
            HomewokIsReplaced?.Invoke(oldH, newH);
            HomeworksListChanged?.Invoke(Global.userData.homeworks);
        }

        /// <summary>
        /// Invoke OnSortingModeChanged event
        /// </summary>
        /// <param name="sortingMode">The type of homeworks sorting.</param>
        /// <param name="args">Arguments to sort homeworks</param>
        internal static void Call_SortingModeChanged(HomeworkSortingMode sortingMode, string args)
        {
            OnSortingModeChanged?.Invoke(sortingMode, args);
        }

        /// <summary>
        /// Invoke HomeworkEditorChanged event
        /// </summary>
        /// <param name="editor">New homework editor</param>
        internal static void Call_HomeworkEditorChanged(IHomeworkEditor editor)
        {
            HomeworkEditorChanged?.Invoke(editor);
        }

        /// <summary>
        /// Invoke HomeworkViewerContainerChanged event
        /// </summary>
        /// <param name="viewer">New viewers container</param>
        internal static void Call_HomeworkViewerChanged(IHomeworkViewerContainer viewer)
        {
            HomeworkViewerContainerChanged?.Invoke(viewer);
        }

        #endregion

        #region Settings manager

        /// <summary>
        /// Invoke WinBackgroundChanged event
        /// </summary>
        /// <param name="brush">New windows background</param>
        internal static void Call_WinBackgroundChanged(Brush brush)
        {
            WinBackgroundChanged?.Invoke(brush);
        }

        /// <summary>
        /// Invoke StylesChanged event
        /// </summary>
        /// <param name="styles">New app styles</param>
        internal static void Call_StylesChanged(Styles styles)
        {
            StylesChanged?.Invoke(styles);
        }

        /// <summary>
        /// Invoke ColorChanged event
        /// </summary>
        /// <param name="target">Type of element of this color (homework, test, subject)</param>
        /// <param name="brush">New color</param>
        internal static void Call_ColorChanged(ColorTarget target, Brush brush)
        {
            ColorChanged?.Invoke(target, brush);
        }

        /// <summary>
        /// Invoke ColorsSettingsChanged event 
        /// </summary>
        /// <param name="newColors">New colors settings</param>
        internal static void Call_ColorsSettingsChanged(ColorsSettings newColors)
        {
            ColorsSettingsChanged?.Invoke(newColors);
        }

        /// <summary>
        /// Invoke FontChanged event
        /// </summary>
        /// <param name="target">Type of element of this font (homework, test, subject)</param>
        /// <param name="font">New font</param>
        internal static void Call_FontChanged(FontTarget target, FontGroup font)
        {
            FontChanged?.Invoke(target, font);
        }

        /// <summary>
        /// Invoke FontsSettingsChanged event
        /// </summary>
        /// <param name="newFonts">New fonts settings</param>
        internal static void Call_FontsSettingsChanged(FontsSettings newFonts)
        {
            FontsSettingsChanged?.Invoke(newFonts);
        }

        /// <summary>
        /// Invoke SubjectAdded event
        /// </summary>
        /// <param name="subject">New subject</param>
        internal static void Call_SubjectAdded(Subject subject)
        {
            SubjectAdded?.Invoke(subject);
        }

        /// <summary>
        /// Invoke SubjectAdded event
        /// </summary>
        /// <param name="subject">New subject</param>
        internal static void Call_SubjectDeleted(Subject subject)
        {
            SubjectDeleted?.Invoke(subject);
        }

        /// <summary>
        /// Invoke SubjectsListChanged event
        /// </summary>
        /// <param name="subjects">New subjects list</param>
        internal static void Call_SubjectsListChanged(List<Subject> subjects)
        {
            SubjectsListChanged?.Invoke(subjects);
        }

        internal static void Call_ShortcutWordsChanged(string[] words)
        {
            ShortcutWordsChanged?.Invoke(words);
        }

        #endregion

        #region SchoolGrade manager

        /// <summary>
        /// Invoke SchoolGradeAdded event
        /// </summary>
        /// <param name="subject">Subject of new school grade</param>
        /// <param name="grade">School grade</param>
        internal static void Call_SchoolGradeAdded(Subject subject, Grade grade)
        {
            SchoolGradeAdded?.Invoke(subject, grade);
        }

        /// <summary>
        /// Invoke SchoolGradeRemoved event
        /// </summary>
        /// <param name="subject">Subject of removed school grade</param>
        /// <param name="grade"></param>
        internal static void Call_SchoolGradeRemoved(Subject subject, Grade grade)
        {
            SchoolGradeRemoved?.Invoke(subject, grade);
        }

        /// <summary>
        /// Invoke NewTrimesterEvent event
        /// </summary>
        /// <param name="oldGrades">Old school grades of trimester</param>
        internal static void Call_NewTrimesterEvent(SchoolGrades oldGrades)
        {
            NewTrimesterEvent?.Invoke(oldGrades);
        }

        #endregion

        #region Reminders manager

        /// <summary>
        /// Invoke ReminderAdded event
        /// </summary>
        /// <param name="_reminder">Added reminder</param>
        internal static void Call_ReminderAdded(string _reminder)
        {
            ReminderAdded?.Invoke(_reminder);
        }

        /// <summary>
        /// Invoke ReminderRemoved event
        /// </summary>
        /// <param name="_reminder">Removed homework</param>
        internal static void Call_ReminderRemoved(string _reminder)
        {
            ReminderRemoved?.Invoke(_reminder);
        }

        #endregion

        #region Schedule manager

        /// <summary>
        /// Invoke AppointmentAdded event
        /// </summary>
        /// <param name="appointment">Added appointment</param>
        internal static void Call_AppointmentAdded(Appointment appointment)
        {
            AppointmentAdded?.Invoke(appointment);
        }

        /// <summary>
        /// Invoke AppointmentRemoved event
        /// </summary>
        /// <param name="appointment">Removed appointment</param>
        internal static void Call_AppointmentRemoved(Appointment appointment)
        {
            AppointmentRemoved?.Invoke(appointment);
        }

        /// <summary>
        /// Invoke SubjectColorChanged event 
        /// </summary>
        /// <param name="subject">Subject whose color has been changed</param>
        /// <param name="color">New color of subject</param>
        internal static void Call_SubjectColorChanged(Subject subject, System.Drawing.Color color)
        {
            SubjectColorChanged?.Invoke(subject, color);
        }

        #endregion

        #region MainWindow

        /// <summary>
        /// Invoke OnAddHomeworks_ButtonClick event
        /// </summary>
        /// <param name="enabled">True if the editor has been activated</param>
        internal static void Call_OnAddHomeworks_ButtonClick(bool enabled)
        {
            OnAddHomeworks_ButtonClick?.Invoke(enabled);
        }

        /// <summary>
        /// Invoke OnHomeworkWriting event
        /// </summary>
        /// <param name="homework">Homework that is writing</param>
        internal static void Call_OnHomeworkWriting(Homework homework)
        {
            OnHomeworkWriting?.Invoke(homework);
        }

        /// <summary>
        /// Invoke OnWordShorcut_ButtonClick event
        /// </summary>
        /// <param name="button">Clicked button</param>
        /// <param name="word">Shortcut word</param>
        internal static void Call_OnWordShorcut_ButtonClick(Button button, string word)
        {
            OnWordShorcut_ButtonClick?.Invoke(button, word);
        }

        #endregion

        #region PluginManager

        /// <summary>
        /// Invoke OnPluginStarted event
        /// </summary>
        /// <param name="loader">Started plugin</param>
        internal static void Call_OnPluginStarted(Plugin.PluginLoader loader)
        {
            OnPluginStarted?.Invoke(loader);
        }

        /// <summary>
        /// Invoke OnPluginInstalled event
        /// </summary>
        /// <param name="loader">Installed plugin</param>
        internal static void Call_OnPluginInstalled(Plugin.PluginLoader loader)
        {
            OnPluginInstalled?.Invoke(loader);
        }

        #endregion

        #endregion

    }
}
