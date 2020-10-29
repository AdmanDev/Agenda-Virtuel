using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
using Agenda_Virtuel.Manager;
using MyFunctions;
using FontStyle = System.Windows.FontStyle;

namespace Agenda_Virtuel
{
    internal static class SaveInServer
    {
        /*___________________________VARIABLES___________________________*/
        private const string PHPupdateDataURL = "https://admandev.fr/api/agenda_virtuel/dataupdateavailable.php";
        private const string PHPhomeworksURL = "https://admandev.fr/api/agenda_virtuel/homework.php";
        private const string PHPreminderURL = "https://admandev.fr/api/agenda_virtuel/others/reminder.php";
        private const string PHPappointmentURL = "https://admandev.fr/api/agenda_virtuel/others/appointment.php";
        private const string PHPschoolgradesURL = "https://admandev.fr/api/agenda_virtuel/others/schoolgrades/schoolgrades.php";
        private const string PHPschoolsubjectURL = "https://admandev.fr/api/agenda_virtuel/others/schoolgrades/school_subject.php";
        private const string PHPsgradeURL = "https://admandev.fr/api/agenda_virtuel/others/schoolgrades/school_grade.php";
        private const string PHPsubjectURL = "https://admandev.fr/api/agenda_virtuel/settings/subjects.php";
        private const string PHPsettingsURL = "https://admandev.fr/api/agenda_virtuel/settings/settings.php";
        private const string PHPcolorURL = "https://admandev.fr/api/agenda_virtuel/settings/color.php";
        private const string PHPshortcutwordURL = "https://admandev.fr/api/agenda_virtuel/settings/shortcutword.php";
        private const string PHPfontsURL = "https://admandev.fr/api/agenda_virtuel/settings/fonts.php";
        private const string PHPstyleURL = "https://admandev.fr/api/agenda_virtuel/settings/style.php";

        public static bool queryAllowed = true;
        private static DateTime lastDataUpdate = DateTime.Now;

        /*___________________________PROPERTIES___________________________*/
        public static int UserID { get => Properties.Settings.Default.UserID; }
        private static System.Windows.Forms.Timer UpdateTimer { get; set; } = new System.Windows.Forms.Timer()
        {
            Interval = 10000
        };

        /*___________________________FUNCTIONS___________________________*/

        internal static void Intitialize()
        {
            //Homeworks
            EventsManager.NewHomeworkSaved += OnNewHomeworkSaved;
            EventsManager.HomeworkDeleted += OnHomeworkDeleted;
            EventsManager.HomewokIsReplaced += OnHomewokIsReplaced;

            ////Settings
            EventsManager.SubjectsListChanged += OnSubjectsListChanged;
            EventsManager.ShortcutWordsChanged += OnShortcutWordsChanged;
            EventsManager.StylesChanged += OnStylesChanged;
            EventsManager.ColorChanged += OnColorChanged;
            EventsManager.WinBackgroundChanged += OnWinBackgroundChanged;
            EventsManager.FontChanged += OnFontChanged;

            ////Reminders
            EventsManager.ReminderAdded += OnReminderAdded;
            EventsManager.ReminderRemoved += OnReminderRemoved;

            ////Schedule
            EventsManager.AppointmentAdded += OnAppointmentAdded;
            EventsManager.AppointmentRemoved += OnAppointmentRemoved;
            EventsManager.SubjectColorChanged += OnSubjectColorChanged;

            ////SchoolGrades
            EventsManager.SchoolGradeAdded += On_SchoolGrades_GradeAdded;
            EventsManager.SchoolGradeRemoved += On_SchoolGrades_GradeRemoved;
            EventsManager.NewTrimesterEvent += OnNewTrimesterEvent;

            //Initialize update timer
            UpdateTimer.Tick += UpdateTimer_Tick;
        }

        public static void Connection()
        {
            SartUpdateTimer();
        }

        internal static void SartUpdateTimer()
        {
            UpdateTimer.Start();
        }

        internal static bool IsEnabled()
        {
            return UserID >= 0;
        }

        private static void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!IsEnabled())
            {
                UpdateTimer.Stop();
                return;
            }

            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("appDate", lastDataUpdate.ToString("yyyy-MM-dd HH':'mm':'ss")),
                    new KeyValuePair<string, string>("user", UserID.ToString())
                });

                XElement json = await Functions.SendPostRequest(elements, PHPupdateDataURL);

                if (json.Value == "false")
                {
                    return;
                }

                Download();

            });
        }

        private static void DataUpdating()
        {
            Execute(() =>
            {
                lastDataUpdate = MyWebRequest.GetWebTime();
            });
        }

        internal static void DisableQuery()
        {
            if (IsEnabled())
            {
                queryAllowed = false;
            }
        }

        private static void Execute(Action method)
        {
            if (!IsEnabled() || !Functions.InternetConnection())
                return;
            method?.Invoke();
        }

        #region Write

        #region Write Homeworks

        private static void OnNewHomeworkSaved(Homework h)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "insert"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("date", h.Date?.ToString("yyyy-MM-dd")),
                    new KeyValuePair<string, string>("subject", h.Subject),
                    new KeyValuePair<string, string>("job", h.Job),
                    new KeyValuePair<string, string>("isTest", Convert.ToInt32(h.IsTest).ToString())
                });

                XElement json = await Functions.SendPostRequest(elements, PHPhomeworksURL);

                if (json.Element("homeworkID") != null)
                {
                    h.ID = Convert.ToInt32(json.Element("homeworkID").Value);
                }

                DataUpdating();
            });
        }

        private static void OnHomeworkDeleted(Homework h)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "delete"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("hmID", h.ID.ToString()),
                });

                await Functions.SendPostRequest(elements, PHPhomeworksURL);

                DataUpdating();
            });
        }

        private static void OnHomewokIsReplaced(Homework oldHomework, Homework h)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "update"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("hmID", h.ID.ToString()),
                    new KeyValuePair<string, string>("date", h.Date?.ToString("yyyy-MM-dd")),
                    new KeyValuePair<string, string>("subject", h.Subject),
                    new KeyValuePair<string, string>("job", h.Job),
                    new KeyValuePair<string, string>("isTest", Convert.ToInt32(h.IsTest).ToString())
                });

                await Functions.SendPostRequest(elements, PHPhomeworksURL);

                DataUpdating();
            });
        }

        #endregion

        #region Write Settings

        internal static void UpdatePluginIsEnabled(bool isEnabled)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "updatesettings"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("pluginEnabled", Convert.ToInt32(isEnabled).ToString())
                });

                await Functions.SendPostRequest(elements, PHPsettingsURL);

                DataUpdating();
            });
        }

        private static void OnSubjectsListChanged(List<Subject> subjects)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "deleteall"),
                    new KeyValuePair<string, string>("user", UserID.ToString())
                });

                await Functions.SendPostRequest(elements, PHPsubjectURL);

                string values = "";
                for (int i = 0; i < subjects.Count; i++)
                {
                    string colorString = ColorTranslator.ToHtml(subjects[i].Color);

                    if(i > 0)
                    {
                        values += Environment.NewLine;
                    }

                    values += subjects[i].Name + ";" + colorString + ";" + subjects[i].Coeff;
                }

                elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "insertall"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("values", values)
                });

                await Functions.SendPostRequest(elements, PHPsubjectURL);

                DataUpdating();
            });
        }

        private static void OnShortcutWordsChanged(string[] words)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "deleteall"),
                    new KeyValuePair<string, string>("user", UserID.ToString())
                });

                await Functions.SendPostRequest(elements, PHPshortcutwordURL);

                string values = "";
                for (int i = 0; i < words.Length; i++)
                {
                    if (i > 0)
                    {
                        values += ",";
                    }

                    values += words[i];
                }

                elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "insertall"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("words", values)
                });

                await Functions.SendPostRequest(elements, PHPshortcutwordURL);

                DataUpdating();
            });

        }

        private static async Task<XElement> StyleRequestAsync(string mode, Styles s)
        {
            string secButStyle;
            if (string.IsNullOrEmpty(s.SecButtonStyle))
            {
                secButStyle = XamlWriter.Save(s.SecondaryButtonStyle);
            }
            else
            {
                secButStyle = s.SecButtonStyle;
            }

            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", mode),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("WinStyle", s.WinStyle),
                new KeyValuePair<string, string>("PrimButtonStyle", s.PrimButtonStyle),
                new KeyValuePair<string, string>("SecButtonStyle", secButStyle),
            });

            XElement json = await Functions.SendPostRequest(elements, PHPstyleURL);

            DataUpdating();

            return json;
        }

        private static async Task<bool?> InsertStyleAsync(Styles s)
        {
            XElement json = await StyleRequestAsync("insertifnotexists", s);

            string result = "";
            if (json.Element("success") != null)
            {
                result = json.Element("success").Value;
            }

            switch (result)
            {
                case "exists":
                    return false;

                case "true":
                    return true;

                default:
                    return null;
            }
        }

        private static void OnStylesChanged(Styles s)
        {
            Execute(async () =>
            {
                if (await InsertStyleAsync(s) == false)
                {//Update
                    XElement json = await StyleRequestAsync("update", s);
                }

                DataUpdating();
            });
        }

        private static async Task UpdateColorAsync(string target, string color)
        {
            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", "update"),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("target", target),
                new KeyValuePair<string, string>("color", color)
            });

            await Functions.SendPostRequest(elements, PHPcolorURL);

            DataUpdating();
        }

        private static void OnColorChanged(ColorTarget target, System.Windows.Media.Brush brush)
        {
            string brushString = XamlWriter.Save(brush);
            Execute(async () =>
            {
                if (await InsertColorsSettingsAsync() == false)
                {
                    string atr = "";

                    switch (target)
                    {
                        case ColorTarget.NormalHomeworks:
                            atr = "NormalHomeworkColor";
                            break;
                        case ColorTarget.Tests:
                            atr = "TestColor";
                            break;
                        case ColorTarget.subjects:
                            atr = "SubjectColor";
                            break;
                        case ColorTarget.HighlightedHomeworks:
                            atr = "HighlightColor";
                            break;
                    }

                    await UpdateColorAsync(atr, brushString);

                    DataUpdating();
                }
                
            });
        }

        private static bool backgroundFirstChanged = true;
        private static void OnWinBackgroundChanged(System.Windows.Media.Brush brush)
        {
            if(backgroundFirstChanged)
            {
                backgroundFirstChanged = false;
                return;
            }

            if (!queryAllowed)
            {
                queryAllowed = true;
                return;
            }

            Execute(async () =>
            {
                if(await InsertColorsSettingsAsync() == false)
                {
                    await UpdateColorAsync("BackgroundColor", XamlWriter.Save(brush));
                }
            });

            DataUpdating();
        }

        //Return true if it insert data, false if data exists already, null if SQL queries failed.
        private static async Task<bool?> InsertColorsSettingsAsync()
        {
            List<string> cs = Global.userData.settings.serialisableColors;
            if (cs.Count < 5)
            {
                return null;
            }

            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", "insertifnotexists"),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("BackgroundColor", cs[0]),
                new KeyValuePair<string, string>("NormalHomeworkColor", cs[1]),
                new KeyValuePair<string, string>("TestColor", cs[2]),
                new KeyValuePair<string, string>("SubjectColor", cs[3]),
                new KeyValuePair<string, string>("HighlightColor", cs[4])
            });

            XElement json = await Functions.SendPostRequest(elements, PHPcolorURL);

            string result = "";
            if(json.Element("success") != null)
            {
                result = json.Element("success").Value;
            }

            switch (result)
            {
                case "exists":
                    return false;

                case "true":
                    return true;

                default:
                    return null;
            }
        }

        private static void OnFontChanged(FontTarget target, FontGroup f)
        {
            Execute(async () =>
            {
                if(await InsertFontsSettingsAsync() == false)
                {
                    string targetColumn = "";

                    switch (target)
                    {
                        case FontTarget.NormalHomeworks:
                            targetColumn = "ID_NormalHMFont";
                            break;

                        case FontTarget.Tests:
                            targetColumn = "ID_TestFont";
                            break;

                        case FontTarget.subjects:
                            targetColumn = "ID_SubjectFont";
                            break;
                    }

                    string size = Math.Round(f.fontSize).ToString().Replace(",", ".");
                    string italic = Convert.ToInt32(f.italic).ToString();
                    string bold = Convert.ToInt32(f.bold).ToString();

                    FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("mode", "update"),
                        new KeyValuePair<string, string>("user", UserID.ToString()),
                        new KeyValuePair<string, string>("target", targetColumn),
                        new KeyValuePair<string, string>("FontName", f.fontFamilyName),
                        new KeyValuePair<string, string>("FontSize", size),
                        new KeyValuePair<string, string>("Italic", italic),
                        new KeyValuePair<string, string>("Bold", bold)
                    });

                    await Functions.SendPostRequest(elements, PHPfontsURL);

                    DataUpdating();
                }
            });
        }

        //Return true if it insert data, false if data exists already, null if SQL queries failed.
        private static async Task<bool?> InsertFontsSettingsAsync()
        {
            FontsSettings fonts = Global.userData.settings.fonts;
            FontGroup normalF = fonts.NormalHomeworksFont;
            FontGroup testF = fonts.TestsFont;
            FontGroup subjectF = fonts.SubjectsFont;

            FontGroup[] fgs = new FontGroup[] { normalF, testF, subjectF };
            string values = "";
            for (int i = 0; i < fgs.Length; i++)
            {
                if(i > 0)
                {
                    values += Environment.NewLine;
                }

                string size = Math.Round(fgs[i].fontSize, 3).ToString().Replace(",", ".");
                int italic = Convert.ToInt32(fgs[i].italic);
                int bold = Convert.ToInt32(fgs[i].bold);

                values += fgs[i].fontFamilyName + ";" + size + ";" + italic + ";" + bold;
            }

            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", "insertifnotexists"),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("Fonts", values)
            });

            XElement json = await Functions.SendPostRequest(elements, PHPfontsURL);

            string result = "";
            if (json.Element("success") != null)
            {
                result = json.Element("success").Value;
            }

            switch (result)
            {
                case "exists":
                    return false;

                case "true":
                    return true;

                default:
                    return null;
            }
        }

        #endregion

        #region Write Reminders

        private static async void WriteReminder(string mode, string reminder)
        {
            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", mode),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("value", reminder)
            });

            await Functions.SendPostRequest(elements, PHPreminderURL);

            DataUpdating();
        }

        private static void OnReminderAdded(string reminder)
        {
            Execute(() =>
            {
                WriteReminder("insert", reminder);
            });
        }

        private static void OnReminderRemoved(string reminder)
        {
            Execute(() =>
            {
                WriteReminder("delete", reminder);
            });
        }

        #endregion

        #region Write Schedule

        private static async void WriteAppointment(string mode, Appointment ap)
        {
            string dateFormat = "yyyy-MM-dd HH':'mm':'ss";
            string startTime = ap.StartTime.ToString(dateFormat);
            string endTime = ap.EndTime.ToString(dateFormat);

            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", mode),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("title", ap.Title),
                new KeyValuePair<string, string>("startTime", startTime),
                new KeyValuePair<string, string>("endTime", endTime)
            });

            await Functions.SendPostRequest(elements, PHPappointmentURL);

            DataUpdating();
        }

        private static void OnAppointmentAdded(Appointment ap)
        {
            Execute(() =>
            {
                WriteAppointment("insert", ap);
            });
        }

        private static void OnAppointmentRemoved(Appointment ap)
        {
            Execute(() =>
            {
                WriteAppointment("delete", ap);
            });
        }

        private static void OnSubjectColorChanged(Subject subject, Color color)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "update"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("title", subject.Name),
                    new KeyValuePair<string, string>("color", ColorTranslator.ToHtml(color))
                });

                await Functions.SendPostRequest(elements, PHPsubjectURL);

                DataUpdating();
            });
        }

        #endregion

        #region Write SchoolGrades

        private static async void WriteSchoolSubject(string mode, Subject s)
        {
            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", mode),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("name", s.Name),
                new KeyValuePair<string, string>("coeffSubject", s.Coeff.ToString())
            });

            await Functions.SendPostRequest(elements, PHPschoolsubjectURL);

            DataUpdating();
        }

        public static void UpdateSchoolOptions(bool showComments, bool highlightResult)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("mode", "update"),
                    new KeyValuePair<string, string>("user", UserID.ToString()),
                    new KeyValuePair<string, string>("showComments", Convert.ToInt32(showComments).ToString()),
                    new KeyValuePair<string, string>("highlighResult", Convert.ToInt32(highlightResult).ToString())
                });

                await Functions.SendPostRequest(elements, PHPschoolgradesURL);

                DataUpdating();
            });
        }

        private static void On_SchoolGrade_SubjectAdded(Subject subject)
        {
            Execute(()=>
            {
                WriteSchoolSubject("insert", subject);
            });
        }

        private static void On_SchoolGrade_SubjectRemoved(Subject subject)
        {
            Execute(() =>
            {
                WriteSchoolSubject("delete", subject);
            });
        }

        private static async void WriteGrade(string mode, Subject s, Grade g)
        {
            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", mode),
                new KeyValuePair<string, string>("user", UserID.ToString()),
                new KeyValuePair<string, string>("name", s.Name),
                new KeyValuePair<string, string>("grade", g.TheGrade.ToString()),
                new KeyValuePair<string, string>("outOf", g.outOf.ToString()),
                new KeyValuePair<string, string>("coeffGrade", g.Coeff.ToString()),
                new KeyValuePair<string, string>("date", g.date.ToString("yyyy-MM-dd HH':'mm':'ss"))
            });

            await Functions.SendPostRequest(elements, PHPsgradeURL);

            DataUpdating();
        }

        private static void On_SchoolGrades_GradeAdded(Subject subject, Grade grade)
        {
            Execute(() =>
            {
                WriteGrade("insert", subject, grade);
            });
        }

        private static void On_SchoolGrades_GradeRemoved(Subject subject, Grade grade)
        {
            Execute(() =>
            {
                WriteGrade("delete", subject, grade);
            });
        }

        private static void OnNewTrimesterEvent(SchoolGrades oldGrades)
        {
            Execute(async () =>
            {
                FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("mode", "newTrimester"),
                new KeyValuePair<string, string>("user", UserID.ToString())
                });

                await Functions.SendPostRequest(elements, PHPschoolsubjectURL);

                DataUpdating();
            });
        }

        #endregion

        #endregion

        #region Read

        internal static async void Download()
        {
            try
            {
                //If syncronization data is not activated, we stop this method.
                if (!IsEnabled() || !Functions.InternetConnection())
                    return;

                //Initialize userData variable
                if (Global.userData == null)
                {
                    Global.userData = new Save();
                }

                //Get homeworkks
                Global.userData.homeworks = await GetHomeworksAsync();

                //Get settings
                object[] subjects = await GetSubjectsAsync();

                Global.userData.settings.PluginSettings.enabled = await GetPluginEnabledAsync();
                Global.userData.settings.Subjects = (List<Subject>)subjects[1];
                Global.userData.settings.serialisableColors = await GetColorsSettingsAsync();
                Global.userData.settings.ShortcutWords = await GetShortcutWordsAsync();
                Global.userData.settings.fonts = await GetFontsSettingsAsync();
                Global.userData.settings.Styles = await GetStylesAsync();

                //Get others modules
                Global.userData.reminders = await GetRemindersAsync();
                Global.userData.scheduleAppointments = await GetAppointmentsAsync();
                Global.userData.schoolGrades = (SchoolGrades)subjects[0];

                //update
                lastDataUpdate = MyWebRequest.GetWebTime();

                //Call "Downloaded data" event
                EventsManager.Call_DatasDownloaded(Global.userData);
            }
            catch (Exception e)
            {
                UpdateTimer.Stop();
                System.Windows.Forms.MessageBox.Show(e.Message, "Erreur");
            }
        }

        private static async Task<XElement> GetUserDataAsync(string url)
        {
            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mode", "select"),
                new KeyValuePair<string, string>("user", UserID.ToString())
            });

            return await Functions.SendPostRequest(elements, url);
        }

        public static async Task<List<Homework>> GetHomeworksAsync()
        {
            List<Homework> homeworks = new List<Homework>();

            XElement json = await GetUserDataAsync(PHPhomeworksURL);

            foreach (XElement xe in json.Elements("item"))
            {
                int id = Convert.ToInt32(xe.Element("ID")?.Value);
                DateTime date = DateTime.Parse(xe.Element("TheDate")?.Value);
                string subject = xe.Element("Subject")?.Value;
                string job = xe.Element("Job")?.Value;
                bool isTest = xe.Element("IsTest")?.Value == "1";

                homeworks.Add(new Homework(date, subject, job, isTest) { ID = id });
            }

            return homeworks;
        }

        private static async Task<object[]> GetSubjectsAsync()
        {
            SchoolGrades schoolGrades = new SchoolGrades();
            List<Subject> subjectsList = new List<Subject>();

            XElement json = await GetUserDataAsync(PHPschoolgradesURL);

            if (json.Element("options") != null)
            {
                XElement jOption = json.Element("options");
                schoolGrades.ShowComments = jOption.Element("ShowComments")?.Value == "1";
                schoolGrades.HighlighResult = jOption.Element("HighlighResult")?.Value == "1";
            }
            if (json.Element("subjects") != null)
            {
                XElement jSubjects = json.Element("subjects");
                foreach (XElement sub in jSubjects.Elements())
                {
                    string title = sub.Element("Title")?.Value;
                    float coeff = float.Parse(sub.Element("Coeff")?.Value.Replace(".", ","));
                    Color color = ColorManager.FromHexa(sub.Element("Color")?.Value);
                    XElement grades = sub.Element("grades");

                    Subject subject = new Subject(title, coeff, color);

                    foreach (XElement g in grades.Elements())
                    {
                        float grade = float.Parse(g.Element("Grade")?.Value.Replace(".", ","));
                        float outOf = float.Parse(g.Element("OutOf")?.Value.Replace(".", ","));
                        float gCoeff = float.Parse(g.Element("Coeff")?.Value.Replace(".", ","));
                        DateTime date = Convert.ToDateTime(g.Element("TheDate")?.Value);

                        Grade theGrade = new Grade(grade, outOf, gCoeff, date);
                        subject.grades.Add(theGrade);
                    }

                    subjectsList.Add(subject);
                }
            }

            return new object[] { schoolGrades, subjectsList };

        }

        private static async Task<List<Appointment>> GetAppointmentsAsync()
        {
            XElement json = await GetUserDataAsync(PHPappointmentURL);

            List<Appointment> appointments = new List<Appointment>();
            foreach (XElement xe in json.Elements())
            {
                string title = xe.Element("Title")?.Value;
                string startTimeString = xe.Element("StartTime")?.Value;
                string endTimeString = xe.Element("EndTime")?.Value;

                DateTime startTime = Convert.ToDateTime(startTimeString);
                DateTime endTime = Convert.ToDateTime(endTimeString);

                Appointment ap = new Appointment(title, startTime, endTime);
                appointments.Add(ap);
            }

            return appointments;
        }

        public static async Task<List<string>> GetRemindersAsync()
        {
            XElement json = await GetUserDataAsync(PHPreminderURL);

            List<string> reminders = new List<string>();
            foreach (XElement xe in json.Elements())
            {
                string value = xe.Element("Value")?.Value;
                reminders.Add(value);
            }

            return reminders;
        }

        #region Settings

        public static async Task<bool> GetPluginEnabledAsync()
        {
            FormUrlEncodedContent elements = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("mode", "selectsettings"),
                    new KeyValuePair<string, string>("user", UserID.ToString())
            });

            XElement json = await Functions.SendPostRequest(elements, PHPsettingsURL);

            bool result = true;
            if(json.Element("PluginsEnabled") != null)
            {
                result = json.Element("PluginsEnabled").Value == "1";
            }

            return result;
        }

        private static async Task<FontsSettings> GetFontsSettingsAsync()
        {
            await InsertFontsSettingsAsync();

            XElement json = await GetUserDataAsync(PHPfontsURL);

            List<FontGroup> userFonts = new List<FontGroup>();
            foreach (XElement xe in json.Elements())
            {
                string name = xe.Element("FontName")?.Value;
                double size = Convert.ToDouble(xe.Element("FontSize")?.Value?.Replace(".", ","));
                bool italic = Convert.ToBoolean(Convert.ToInt32(xe.Element("Italic")?.Value));
                bool bold = Convert.ToBoolean(Convert.ToInt32(xe.Element("Bold")?.Value));

                FontStyle italicStyle = italic ? FontStyles.Italic : FontStyles.Normal;
                FontWeight boldStyle = bold ? FontWeights.Bold : FontWeights.Normal;

                FontGroup fg = new FontGroup(name, size, italicStyle, boldStyle);
                userFonts.Add(fg);
            }

            FontsSettings fonts = new FontsSettings(userFonts[2], userFonts[1], userFonts[0]);

            return fonts;
        }

        private static async Task<Styles> GetStylesAsync()
        {
            XElement json = await StyleRequestAsync("select", new Styles());

            Styles styles = new Styles();
            foreach (XElement xe in json.Elements())
            {
                string winStyleString = FixBalise(xe.Element("WinStyle")?.Value);
                string primButStyleString = FixBalise(xe.Element("PrimButtonStyle")?.Value);
                string secButStyleString = FixBalise(xe.Element("SecButtonStyle")?.Value);

                styles.WindowsStyle = (Style)XamlReader.Parse(winStyleString);
                styles.PrimaryButtonStyle = (Style)XamlReader.Parse(primButStyleString);
                styles.SecondaryButtonStyle = (Style)XamlReader.Parse(secButStyleString);
            }

            return styles;
        }

        private static async Task<string[]> GetShortcutWordsAsync()
        {
            XElement json = await GetUserDataAsync(PHPshortcutwordURL);

            List<string> scw = new List<string>();
            foreach (XElement xe in json.Elements())
            {
                string word = xe.Element("Word")?.Value;
                scw.Add(word);
            }

            if(scw.Count <= 0)
            {
                scw = new List<string>(Settings.defaultSettings.ShortcutWords);
            }

            return scw.ToArray();
        }

        private static async Task<List<string>> GetColorsSettingsAsync()
        {
            string backColor = "";
            string normalHmColor = "";
            string testColor = "";
            string subjectColor = "";
            string highlightColor = "";

            XElement json = await GetUserDataAsync(PHPcolorURL);

            foreach (XElement color in json.Elements())
            {
                backColor = FixBalise(color.Element("BackgroundColor")?.Value);
                normalHmColor = FixBalise(color.Element("NormalHomeworkColor")?.Value);
                testColor = FixBalise(color.Element("TestColor")?.Value);
                subjectColor = FixBalise(color.Element("SubjectColor")?.Value);
                highlightColor = FixBalise(color.Element("HighlightColor")?.Value);
            }

            if (string.IsNullOrEmpty(backColor) || string.IsNullOrEmpty(normalHmColor) || string.IsNullOrEmpty(testColor) || string.IsNullOrEmpty(subjectColor) || string.IsNullOrEmpty(highlightColor))
            {
                ColorsSettings cs = ColorsSettings.GetDefaultColors();
                return new List<string>
                {
                    XamlWriter.Save(cs.BackgroundColor),
                    XamlWriter.Save(cs.NormalHomeworksColor),
                    XamlWriter.Save(cs.TestsColor),
                    XamlWriter.Save(cs.SubjectsColor),
                    XamlWriter.Save(cs.HighlightColor),

                };
            }

            return new List<string>() { backColor, normalHmColor, testColor, subjectColor, highlightColor };
        }

        private static string FixBalise(string input)
        {
            string output = input;
            output = output.Replace("&lt;", "<");
            output = output.Replace("&gt;", ">");
            output = output.Replace("&quot;", "\"");

            return output;
        }

        #endregion

        #endregion

    }
}