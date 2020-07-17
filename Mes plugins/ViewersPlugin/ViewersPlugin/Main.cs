using System;
using Agenda_Virtuel.Plugin;

namespace ViewersPlugin
{
    public class Main : Plugin
    {
        //Variables
        public static Main instance;

        //on start
        protected override void OnStartup()
        {
            instance = this;
            SetSettingWindow(new SettingsUC());

            string viewerType = GetSetting<string>(SettingsUC.VIEWER_KEY);
            if (string.IsNullOrEmpty(viewerType))
                HomeworkViewerContainerType = typeof(ScheduleViewer);
            else
                HomeworkViewerContainerType = this.GetType().Assembly.GetType("ViewersPlugin." + viewerType);
        }
    }
}
