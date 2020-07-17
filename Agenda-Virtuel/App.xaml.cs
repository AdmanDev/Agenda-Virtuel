using Agenda_Virtuel.Manager;
using System;
using System.IO;
using System.Reflection; 
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Agenda_Virtuel
{
    public partial class App : Application
    {
        //Constant variables
        internal const string NOTIFAPPARG = "notifyMode"; 

        //Properties
        public static App Instence { get; private set; }
        /// <summary>
        /// This is the mode of application. <see cref="AppMode"/>
        /// </summary>
        public static AppMode Mode { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve += LoadEmbedAssembly;
            DeleteFiles();

            Instence = this;
            EventsManager.DatasDownloaded += OnDatasDownloaded;
            SaveInServer.Intitialize();
            LoadDatas();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length <= 1)
            {//Normal mode
                StartNormalMode();
            }
            else
            {
                switch (args[1])
                {
                    case NOTIFAPPARG:
                        Mode = AppMode.NotificationMode;
                        NotificationsManager.Start_HomeworkNotification();
                        if(Agenda_Virtuel.Properties.Settings.Default.RunPluginNotification)
                            Plugin.PluginManager.Open();
                        break;

                    default:
                        StartNormalMode();
                        break;
                }
            }
            
        }

        private void StartNormalMode()
        {
            MyFunctions.UpdateManager.CheckUpdate();

            Mode = AppMode.Normal;
            ShowMainWindow();
            Plugin.PluginManager.Open();
            SaveInServer.Download();
            SaveInServer.SartUpdateTimer();
        }

        private void ShowMainWindow()
        {
            Application.Current.MainWindow = new MainWindow();
            Application.Current.MainWindow.Show();
        }

        private void DeleteFiles()
        {
            //Delete files
            string[] files = new string[Agenda_Virtuel.Properties.Settings.Default.FilesToDelete.Count];
            Agenda_Virtuel.Properties.Settings.Default.FilesToDelete.CopyTo(files, 0);

            foreach (string f in files)
            {
                if (File.Exists(f))
                    File.Delete(f);

                Agenda_Virtuel.Properties.Settings.Default.FilesToDelete.Remove(f);
            }

            Agenda_Virtuel.Properties.Settings.Default.Save();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            
        }

        public static Assembly LoadEmbedAssembly(object sender, ResolveEventArgs args)
        {
            try
            {
                //gets the main Assembly
                Assembly mainAssembly = Assembly.GetExecutingAssembly();
                string finalname = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

                //here we search the resources for our dll and get the first match
                string[] ResourcesList = mainAssembly.GetManifestResourceNames();
                string OurResourceName = null;

                for (int i = 0; i <= ResourcesList.Length - 1; i++)
                {
                    string name = ResourcesList[i];
                    if (name.EndsWith(finalname))
                    {
                        //Get the name then close the loop to get the first occuring value
                        OurResourceName = name;
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(OurResourceName))
                {
                    //get a stream representing our resource then load it as bytes
                    using (Stream stream = mainAssembly.GetManifestResourceStream(OurResourceName))
                    {
                        byte[] block = new byte[stream.Length];
                        stream.Read(block, 0, block.Length);
                        return Assembly.Load(block);
                    }
                }
                else
                {
                    return null;
                }
            }
            catch 
            {
                return null;
            }
        }

        private void LoadDatas(Save save = null)
        {
            Styles.Initialize();

            if (save == null)
                Save.LoadFile();
            else
            {
                Save.LoadSave(save);
            }

            Brush background;
            if (Global.userData.settings.colors.BackgroundColor != null)
            {
                background = Global.userData.settings.colors.BackgroundColor;
            }
            else
            {
                background = Styles.DefaultWinBackgroundColor;
            }

            SaveInServer.DisableQuery();
            SettingsManager.SetWindowsBackground(background, false);
        }

        private void OnDatasDownloaded(Save save)
        {
            LoadDatas(save);   
        }

        
    }
}