using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MyFunctions;
using System.IO;
using Agenda_Virtuel.Manager;
using Agenda_Virtuel.Plugin;
using System.Windows.Controls;
using System.Reflection;
using Button = System.Windows.Controls.Button;
using Agenda_Virtuel.Windows.Settings;
using System.Collections.Generic;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This is settings winow
    /// </summary>
    internal partial class SettingsWindow : Window
    {
        //Variables
        private OpenFileDialog ofd;
        private FontDialog fontD;
        private MaskedTextBox TB_HomeworkNotifTime;

        private Settings settings;
        private bool save;  //If true, save data on closing

        private readonly string startupAppPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Agenda - Virtuel.lnk";
        private bool startNotif = false;

        private string selectedStyle = "PrimaryButtonStyle";

        //Events
        private new event Action OnClosing;

        //Constructor
        public SettingsWindow()
        {
            InitializeComponent();

            save = false;

            ofd = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true
            };

            fontD = new FontDialog()
            {
                ShowColor = false,
                ShowEffects = false,
                AllowScriptChange = false,
                FontMustExist = true
            };

            this.TB_HomeworkNotifTime = new MaskedTextBox()
            {
                Font = new System.Drawing.Font("Microsoft Sans Serif", 14),
                TextAlign = System.Windows.Forms.HorizontalAlignment.Center,
                Text = "1730",
                Mask = "00:00"
            };
            this.TB_HomeworkNotifTime.TextChanged += TB_HomeworkNotifTime_TextChanged;

            this.WFH_HomeworksNotifTime.Child = this.TB_HomeworkNotifTime;

            InitializeColorSetteurs();
            InitializeSS_Style();

            ShowCurrentSettings();

            EventsManager.ColorsSettingsChanged += OnColorsSettingsChanged;
            EventsManager.DatasDownloaded += OnDatasDownloaded;
            EventsManager.SubjectsListChanged += OnSubjectsListChanged;
        }

        private void OnDatasDownloaded(Save save)
        {
            settings = Global.userData.settings;
            ShowCurrentSettings();
        }

        #region Initializing

        private void InitializeColorSetteurs()
        {
            this.CS_NormalHomework.Title = "Devoirs normaux";
            this.CS_NormalHomework.SelectedColorChanged += NormalHomework_ColorChanged;

            this.CS_Tests.Title = "Contrôles";
            this.CS_Tests.SelectedColorChanged += Tests_ColorChanged;

            this.CS_Subjects.Title = "Matières";
            this.CS_Subjects.SelectedColorChanged += Subjects_ColorChanged;

            this.CS_Higlight.Title = "Devoirs surlignés";
            this.CS_Higlight.SelectedColorChanged += Highlight_ColorChanged;
        }

        private void InitializeSS_Style()
        {
            this.SS_Style.StylesChanged += SS_Style_StylesChanged;
            this.SS_Style.OwnerType = typeof(Button);
            this.SS_Style.SStyle = Styles.GetSelectedStyles().PrimaryButtonStyle;
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnClosing?.Invoke();

            if (save)
                Save.SaveData();

            if (startNotif && File.Exists(startupAppPath))
                ProcessManager.RunPowerShellCommand("Start \"" + startupAppPath + "\"");
        }

        #region Show current settings

        private void ShowCurrentSettings()
        {
            settings = Global.userData.settings;

            //Background image
            ShowCurrentTheme();

            //Background color
            if (!settings.colors.BackgroundColor.Equals(ColorsSettings.DefaultsColors.BackgroundColor))
            {
                this.BT_RemoveBackColor.Visibility = Visibility.Visible;
            }

            //Colors
            ShowCurrentColors();

            //Fonts
            ShowCurrentFonts();

            //Subjects list
            ShowCurrentSubjectsList();

            //Shortcut words
            ShowCurrentShortcutWords();

            //Save datas in the server ?
            ShowIfSaveOnline();

            //School grades
            ShowSchoolGradesOptions();

            //Notifications
            this.CB_Notif_Homeworks.IsChecked = Properties.Settings.Default.Notify;
            this.TB_HomeworkNotifTime.Text = Properties.Settings.Default.NotifyTime;
            this.CB_RunPluginsOnNotificationMode.IsChecked = Properties.Settings.Default.RunPluginNotification;

            ShowPluginsSettings();
        }

        private void ShowCurrentTheme()
        {
            this.Grid_BackgroundPreview.Background = settings.colors.BackgroundColor;
        }

        private void ShowCurrentColors()
        {
            this.CS_NormalHomework.SetColor(settings.colors.NormalHomeworksColor);
            this.CS_Tests.SetColor(settings.colors.TestsColor);
            this.CS_Subjects.SetColor(settings.colors.SubjectsColor);
            this.CS_Higlight.SetColor(settings.colors.HighlightColor);
        }

        private void ShowCurrentFonts()
        {
            settings.fonts.NormalHomeworksFont.ApplyTo(this.BT_NormalHWFont);
            settings.fonts.TestsFont.ApplyTo(this.BT_TestsFont);
            settings.fonts.SubjectsFont.ApplyTo(this.BT_SubjectsFont);
        }

        private void ShowCurrentSubjectsList()
        {
            this.SP_subjects.Children.Clear();
            foreach (Subject s in settings.Subjects)
            {
                SubjectView view = new SubjectView(s);
                this.SP_subjects.Children.Add(view);
            }
        }

        private void ShowCurrentShortcutWords()
        {
            string words = "";
            foreach (string s in settings.ShortcutWords)
            {
                if (string.IsNullOrEmpty(words))
                    words = s;
                else
                    words += Environment.NewLine + s;
            }
            this.TB_ShortcutWords.Text = words;
        }

        private void ShowIfSaveOnline()
        {
            if (Properties.Settings.Default.UserID < 0)
            {//Not connected
                this.BT_AccountLogin.Visibility = Visibility.Visible;
                this.BT_AccountLogout.Visibility = Visibility.Collapsed;
            }
            else
            {//Is connected
                this.BT_AccountLogin.Visibility = Visibility.Collapsed;
                this.BT_AccountLogout.Visibility = Visibility.Visible;
            }
        }

        private void ShowSchoolGradesOptions()
        {
            this.CB_DiplayCommentsSchoolGrades.IsChecked = Global.userData.schoolGrades.ShowComments;
            this.CB_HighlightSchoolGradesResult.IsChecked = Global.userData.schoolGrades.HighlighResult;
        }

        private void ShowPluginsSettings()
        {
            //CheckBox enabled
            this.CB_PluginEnabled.IsChecked = Global.userData.settings.PluginSettings.enabled;

            //Homeworks editor
            ShowPlugin_HomeworkEditor();

            //Homeworks viewer
            ShowPlugin_HomeworkViewer();
        }

        private void ShowPlugin_HomeworkEditor()
        {
            //Homeworks editors
            System.Windows.Controls.RadioButton CreateBTR(string text, PluginLoader plugin, bool isChecked = false)
            {
                System.Windows.Controls.RadioButton rb = new System.Windows.Controls.RadioButton()
                {
                    Content = text,
                    Foreground = new SolidColorBrush(Colors.White),
                    IsChecked = isChecked,
                    Tag = plugin
                };

                rb.Click += RB_HmEditor_Click;

                return rb;
            }

            this.SP_HmsEditors.Children.Clear();

            System.Windows.Controls.RadioButton rb_default = CreateBTR("Défaut", null, true);
            this.SP_HmsEditors.Children.Add(rb_default);

            foreach (PluginLoader p in PluginManager.Plugins)
            {
                if (p.AgPlugin == null)
                    continue;

                if (p.AgPlugin.HomeworkEditor != null)
                {
                    if (p.isHomeworksEditor)
                        rb_default.IsChecked = false;

                    this.SP_HmsEditors.Children.Add(CreateBTR(p.name, p, p.isHomeworksEditor));
                }
            }
        }

        private void ShowPlugin_HomeworkViewer()
        {
            //Homeworks viewer
            System.Windows.Controls.RadioButton CreateBTR(string text, PluginLoader plugin, bool isChecked = false)
            {
                System.Windows.Controls.RadioButton rb = new System.Windows.Controls.RadioButton()
                {
                    Content = text,
                    Foreground = new SolidColorBrush(Colors.White),
                    IsChecked = isChecked,
                    Tag = plugin
                };

                rb.Click += RB_HmViewer_Click;

                return rb;
            }

            this.SP_HmsViewers.Children.Clear();

            System.Windows.Controls.RadioButton rb_default = CreateBTR("Défaut", null, true);
            this.SP_HmsViewers.Children.Add(rb_default);

            foreach (PluginLoader p in PluginManager.Plugins)
            {
                if (p.AgPlugin == null)
                    continue;

                if (p.AgPlugin.HomeworkViewer != null)
                {
                    if (p.isHmsViewer)
                        rb_default.IsChecked = false;

                    this.SP_HmsViewers.Children.Add(CreateBTR(p.name, p, p.isHmsViewer));
                }
            }
        }

        #endregion

        #region Theme (Background image)

        private void BT_ChooseTheme_Click(object sender, RoutedEventArgs e)
        {
            ofd.Filter = "PNG|*.png|JPEG|*.jpg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BitmapImage img = new BitmapImage(new Uri(ofd.FileName));
                SettingsManager.SetWindowsBackground(new ImageBrush(img));

                ShowCurrentTheme();
            }
        }

        #endregion

        #region Colors

        private void OnColorsSettingsChanged(ColorsSettings newColors)
        {
            ShowCurrentColors();
        }

        private void BT_ChooseBackColor_Click(object sender, RoutedEventArgs e)
        {
            Windows.ColorDialog colorDialog = new Windows.ColorDialog();
            if (colorDialog.ShowDialog() == true)
            {
                this.BT_RemoveBackColor.Visibility = Visibility.Visible;

                SettingsManager.SetWindowsBackground(colorDialog.SelectedBrush);
                ShowCurrentTheme();
            }
        }

        private void BT_RemoveBackColor_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetWindowsBackground(Styles.DefaultWinBackgroundColor);
            ShowCurrentTheme();
        }

        private void BT_DefaultColor_Click(object sender, RoutedEventArgs e)
        {
            ColorsSettings defaultColors = ColorsSettings.GetDefaultColors();
            defaultColors.BackgroundColor = settings.colors.BackgroundColor;

            SettingsManager.SetColors(defaultColors);
        }

        private void NormalHomework_ColorChanged(Color color)
        {
            SettingsManager.SetColor(ColorTarget.NormalHomeworks, new SolidColorBrush(color));
        }

        private void Tests_ColorChanged(Color color)
        {
            SettingsManager.SetColor(ColorTarget.Tests, new SolidColorBrush(color));
        }

        private void Subjects_ColorChanged(Color color)
        {
            SettingsManager.SetColor(ColorTarget.subjects, new SolidColorBrush(color));
        }

        private void Highlight_ColorChanged(Color color)
        {
            SettingsManager.SetColor(ColorTarget.HighlightedHomeworks, new SolidColorBrush(color));
        }


        #endregion

        #region Fonts

        private void BT_DefaultFonts_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetFonts(new FontsSettings());
            ShowCurrentFonts();
        }

        private void BT_EditFont_Click(object sender, RoutedEventArgs e)
        {
            if (fontD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
                string target = button.Uid;

                FontGroup nfont = new FontGroup(fontD.Font);
                nfont.ApplyTo(button);

                switch (target)
                {
                    case "NormalHomework":
                        SettingsManager.SetFont(FontTarget.NormalHomeworks, nfont);
                        break;

                    case "Tests":
                        SettingsManager.SetFont(FontTarget.Tests, nfont);
                        break;

                    case "Subjects":
                        SettingsManager.SetFont(FontTarget.subjects, nfont);
                        break;

                    default:
                        throw new Exception(button.Name + " button Uid is not available"
                            + Environment.NewLine + "UID = " + button.Uid);

                }
            }
        }




        #endregion

        #region Subjects list

        private void OnSubjectsListChanged(List<Subject> subjectsList)
        {
            ShowCurrentSubjectsList();
        }

        private void BT_AddSubject_Click(object sender, RoutedEventArgs e)
        {
            new AddSubjectWindow().ShowDialog();
            this.SV_Subjects.ScrollToBottom();
        }

        private void BT_DefaultSubjects_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetSubjectList(Settings.defaultSettings.Subjects);
        }

        #endregion

        #region Shortcut words

        bool tb_shortcutword_FirstChange = true;
        bool tb_shortcutword_Changed = false;

        public object Brush { get; private set; }

        private void TB_ShortcutWords_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (tb_shortcutword_FirstChange || tb_shortcutword_Changed)
            {
                tb_shortcutword_FirstChange = false;
                return;
            }

            OnClosing += () =>
            {
                string[] shWords = this.TB_ShortcutWords.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                SettingsManager.SetShortcutWords(shWords, false);
            };

            tb_shortcutword_Changed = true;
            save = true;
        }

        private void BT_DefaultSCWords_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetShortcutWords(Settings.defaultSettings.ShortcutWords);
            ShowCurrentShortcutWords();
        }


        #endregion

        #region Online account

        private void BT_AccountLogin_Click(object sender, RoutedEventArgs e)
        {
            AccountConnectionWindow acw = new AccountConnectionWindow();
            acw.ShowDialog();

            if (acw.Success)
            {
                Properties.Settings.Default.UserID = acw.UserID;
                Properties.Settings.Default.Save();

                SaveInServer.Download();
                SaveInServer.Connection();

                ShowIfSaveOnline();
            }


        }

        private void BT_AccountLogout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UserID = -1;
            Properties.Settings.Default.Save();

            ShowIfSaveOnline();
        }

        #endregion

        #region School grades

        private bool saveSchoolOptions = false;
        private void BT_DefaultSchoolGradesOptions_Click(object sender, RoutedEventArgs e)
        {
            Global.userData.schoolGrades.ShowComments = true;
            Global.userData.schoolGrades.HighlighResult = true;

            ShowSchoolGradesOptions();
            SaveSchoolGradesOptions();

        }

        private void SaveSchoolGradesOptions()
        {
            save = true;

            if (!saveSchoolOptions)
            {
                saveSchoolOptions = true;
                OnClosing += () =>
                {
                    bool showComments = Global.userData.schoolGrades.ShowComments;
                    bool highlightResult = Global.userData.schoolGrades.HighlighResult;
                    SaveInServer.UpdateSchoolOptions(showComments, highlightResult);
                };
            }
        }

        private void CB_DiplayCommentsSchoolGrades_Click(object sender, RoutedEventArgs e)
        {
            Global.userData.schoolGrades.ShowComments = this.CB_DiplayCommentsSchoolGrades.IsChecked == true;
            SaveSchoolGradesOptions();
        }

        private void CB_HighlightSchoolGradesResult_Click(object sender, RoutedEventArgs e)
        {
            Global.userData.schoolGrades.HighlighResult = this.CB_HighlightSchoolGradesResult.IsChecked == true;
            SaveSchoolGradesOptions();
        }


        #endregion

        #region Notification

        private void CB_Notif_Homeworks_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = this.CB_Notif_Homeworks.IsChecked == true;
            if (enabled)
            {
                DriveInfo drive = new DriveInfo(System.Windows.Forms.Application.ExecutablePath);
                if (drive.DriveType != DriveType.Fixed)
                {// USB disk
                    string msg = "Pour activer les notifications, le logiciel doit être installé sur le disque dûr interne de votre ordinateur."
                      + Environment.NewLine + "Installez l'application sur un disque dûr interne puis réessayez.";

                    System.Windows.Forms.MessageBox.Show(msg, "Agenda - Virtuel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.CB_Notif_Homeworks.IsChecked = enabled = false;
                    return;
                }
                else
                {
                    string target = System.Windows.Forms.Application.ExecutablePath;

                    FileManager.CreateShortcut(target, startupAppPath, App.NOTIFAPPARG);
                }
            }
            else
            {
                if (File.Exists(startupAppPath))
                    File.Delete(startupAppPath);
            }

            Properties.Settings.Default.Notify = enabled;
            Properties.Settings.Default.Save();

            startNotif = enabled;

        }

        private bool prim_TB_HomeworkNotifTime_TextChanged = true;
        private void TB_HomeworkNotifTime_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.NotifyTime = this.TB_HomeworkNotifTime.Text;
            Properties.Settings.Default.Save();

            if (!prim_TB_HomeworkNotifTime_TextChanged)
                startNotif = true;

            prim_TB_HomeworkNotifTime_TextChanged = false;
        }

        private void CB_RunPluginsOnNotificationMode_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RunPluginNotification = this.CB_RunPluginsOnNotificationMode.IsChecked == true;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Plugins

        private void CB_PluginEnabled_Click(object sender, RoutedEventArgs e)
        {
            Global.userData.settings.PluginSettings.enabled = this.CB_PluginEnabled.IsChecked == true;
            Save.SaveData();

            SaveInServer.UpdatePluginIsEnabled(this.CB_PluginEnabled.IsChecked == true);

            if (this.CB_PluginEnabled.IsChecked == true)
            {
                PluginManager.StartAll();
                ShowPluginsSettings();
            }
            else
            {
                System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }

        }

        private void RB_HmEditor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton rb = (System.Windows.Controls.RadioButton)sender;
            PluginLoader pl = (PluginLoader)rb.Tag;

            foreach (PluginLoader p in PluginManager.Plugins)
            {
                p.isHomeworksEditor = false;
            }

            if (pl != null)
            {
                pl.isHomeworksEditor = true;
                HomeworkManager.HomeworkEditor = pl.AgPlugin.HomeworkEditor;
            }
            else
            {
                HomeworkManager.HomeworkEditor = new HomeworkEditorControl();
            }

            PluginManager.Save();
        }

        private void RB_HmViewer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton rb = (System.Windows.Controls.RadioButton)sender;
            PluginLoader pl = (PluginLoader)rb.Tag;

            foreach (PluginLoader p in PluginManager.Plugins)
            {
                p.isHmsViewer = false;
            }

            if (pl != null)
            {
                pl.isHmsViewer = true;
                HomeworkManager.ViewerContainer = pl.AgPlugin.HomeworkViewer;
            }
            else
            {
                HomeworkManager.ViewerContainer = new HomeworksViewerContainer();
            }

            PluginManager.Save();

        }


        #endregion

        #region Styles

        private void SS_Style_StylesChanged(Style _style)
        {
            if (_style.TargetType == typeof(Button))
                this.BT_StylePreview.Style = _style;

            SetStyleProperty(selectedStyle, _style);
        }

        private void CB_StyleObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            saveStyle = false;
            selectedStyle = (string)((ComboBoxItem)e.AddedItems[0]).Tag;

            PropertyInfo pi = GetStyleProperty(selectedStyle);
            if (pi != null)
            {
                if (this.SS_Style != null)
                    this.SS_Style.SStyle = (Style)pi.GetValue(Global.userData.settings.Styles);
            }


        }

        private PropertyInfo GetStyleProperty(string propertyName)
        {
            Type stype = Global.userData.settings.Styles.GetType();
            return stype.GetProperty(propertyName);
        }

        private bool styleFirstSave = true;
        private bool saveStyle = true;
        private void SetStyleProperty(string propertyName, Style _style)
        {
            PropertyInfo pi = GetStyleProperty(propertyName);
            if (pi != null)
            {
                pi.SetValue(Global.userData.settings.Styles, _style);

                if (!styleFirstSave)
                    Global.userData.settings.Styles.ApplyStyle(saveStyle);

                styleFirstSave = false;
                saveStyle = true;
            }
        }

        private void BT_DefaultStyle_Click(object sender, RoutedEventArgs e)
        {
            PropertyInfo pi = GetStyleProperty(selectedStyle);

            if (pi != null)
            {
                Style s = (Style)pi.GetValue(Styles.DefaultStyle);
                SetStyleProperty(selectedStyle, s);

                this.SS_Style.SStyle = s;
            }
        }

        #endregion

    }
}
