using System;
using System.Windows;
using System.Windows.Forms;
using Agenda_Virtuel.Windows.Notifications;

namespace Agenda_Virtuel.Manager
{
    /// <summary>
    /// This class allows to show notifications for user.
    /// </summary>
    public static class NotificationsManager
    {
        //_______________________________VARIABLES_______________________________
        /// <summary>
        /// Timer execute action each x milliseconds
        /// </summary>
        private static Timer timer;
        /// <summary>
        /// Action to execute
        /// </summary>
        private static Action timerAction;
        /// <summary>
        /// Window whose notification will be displayed
        /// </summary>
        private static Window notifWindow;

        //_______________________________FUNCTIONS_______________________________

        /// <summary>
        /// Initialize a notification that display user homeworks
        /// </summary>
        internal static void Start_HomeworkNotification()
        {
            bool enabled = Properties.Settings.Default.Notify;
            string timeToNotify = Properties.Settings.Default.NotifyTime;

            if (enabled && TimeSpan.TryParse(timeToNotify, out TimeSpan time))
            {
                TimeSpan now = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                TimeSpan timeBeforeNotify = time.Add(- now);

                if (timeBeforeNotify.Ticks >= 0)
                {
                    timerAction = () =>
                    {
                        NotifyHomework();
                    };

                    StartTimer((int)timeBeforeNotify.TotalMilliseconds);
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }

             //   NotifyHomework();
            }
        }

        /// <summary>
        /// Initialize and start the timer
        /// </summary>
        /// <param name="interval">Interval in millisecond of the timer</param>
        private static void StartTimer(int interval)
        {
            timer = new Timer()
            {
                Interval = interval
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            timerAction.Invoke();
            timer.Stop();
        }

        /// <summary>
        /// Show homework notification
        /// </summary>
        private static void NotifyHomework()
        {
            SaveInServer.Download();

            HomeworksViewerContainer viewer = new HomeworksViewerContainer();
            HomeworkManager.Initialize(viewer, new HomeworkEditorControl());

            Homework[] homeworks = null;
            DateTime date = HomeworkManager.todayDate.AddDays(1);

            for (int i = 0; i < 15; i++)
            {
                homeworks = HomeworkManager.GetHomeworksOf(date);

                if (homeworks == null || homeworks.Length <= 0)
                    date = date.AddDays(1);
                else
                    break;
            }

            new HomeworksNotificationWindow(homeworks, viewer, date).Show();
        }

        /// <summary>
        /// Show notification.
        /// </summary>
        /// <param name="title">The title of notification.</param>
        /// <param name="controlToShow">The WPF UserControl to show.</param>
        /// <param name="duration">The duration of the notification, in milliseconds</param>
        public static void Notify(string title, System.Windows.Controls.UserControl controlToShow, int duration = 5000)
        {
            notifWindow = new NotificationWindow(title, controlToShow);

            timerAction = () =>
            {
                if (notifWindow != null)
                {
                    notifWindow.Close();
                    notifWindow = null;
                }
            };

            StartTimer(duration);
            notifWindow.Show();
        }

    }
}
