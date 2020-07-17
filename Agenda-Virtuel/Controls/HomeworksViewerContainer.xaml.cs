using System;
using System.Windows;
using System.Windows.Controls;
using Agenda_Virtuel.Manager;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This is the default <c>IHomeworkViewerContainer</c>. It allow to display homeworks viewers (<c>IHomeworkViewer</c>).
    /// You can create your own viewers container if you create you <c>IHomeworkViewer</c> also.
    /// </summary>
    public partial class HomeworksViewerContainer : UserControl, IHomeworkViewerContainer
    {
        //Properties
        public Type ViewerItemType { get; set; }

        //Constructor
        public HomeworksViewerContainer()
        {
            InitializeComponent();

            ViewerItemType = typeof(HomeworkViewerItem);
            if (App.Mode == AppMode.NotificationMode)
                this.BT_CenterAddHomework.Visibility = Visibility.Collapsed;

            EventsManager.HomeworkViewerItemsListChanged += OnHomeworkViewerItemsListChanged;
        }

        private void OnHomeworkViewerItemsListChanged()
        {
            if (App.Mode == AppMode.NotificationMode)
                return;

            if (HomeworkManager.DisplayedCount > 0)
                this.BT_CenterAddHomework.Visibility = Visibility.Collapsed;
            else
                this.BT_CenterAddHomework.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Remove all homeworks viewers.
        /// </summary>
        public void Clear()
        {
            this.SP_Homeworks.Children.Clear();
        }

        /// <summary>
        /// Display a homework viewer.
        /// </summary>
        /// <param name="_homeworkViewerItem"><c>IHomeworkViewer</c> to display.</param>
        public void DisplayHomework(IHomeworkViewer _homeworkViewerItem)
        {
            this.SP_Homeworks.Children.Add(_homeworkViewerItem as UserControl);
        }

        /// <summary>
        /// Remove a homework viewer.
        /// </summary>
        /// <param name="viewer"><c>IHomeworkViewer</c> to remove.</param>
        public void RemoveViewer(IHomeworkViewer viewer)
        {
            this.SP_Homeworks.Children.Remove(viewer as UserControl);
        }

        private void BT_AddHomework_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.ShowHomeworkEditor();
        }
    }
}
