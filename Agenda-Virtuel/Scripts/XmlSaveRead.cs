using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This static class allows to save data to a xml file
    /// </summary>
    public static class XmlSaveRead
    {
        //XML Elements name
        private const string HOMEWORKSLIST = "Homeworks";
        private const string HOMEWORK = "Homework";
        private const string SETTINGS = "Settings";
        private const string SUBJECTSLIST = "SubjectsList";
        private const string SHORTCUTWORDSList = "ShortcutWordsList";
        private const string SHORTCUTWORD = "ShortcutWord";
        private const string SCHOOLGRADES = "SchoolGrades";
        private const string GRADESSUBJECT = "GradesSubject";
        private const string GRADE = "Grade";
        private const string SCHEDULE = "Schedule";
        private const string SUBJECTSCOLORSLIST = "SubjectsColorsList";
        private const string SUBJECTCOLOR = "SubjectColor";
        private const string SUBJECTCOLOR_NAME = "SubjectName";
        private const string SUBJECTCOLOR_COLOR = "Color";
        private const string APPOINTMENTSLIST = "AppointmentsList";
        private const string APPOINTMENT = "Appointment";
        private const string DAYINDEX = "DayIndex";
        private const string REMINDERLIST = "RemindersList";
        private const string REMINDER = "Reminders";

        //METHODS
        /// <summary>
        /// Convert a Save object to Xml
        /// </summary>
        /// <param name="userData">Data to convert</param>
        /// <returns>Return XElement containing data</returns>
        public static XElement DataToXml(Save userData)
        {
            XElement root = new XElement("UserData");

            //Add homeworks list
            XElement xmlHomeworks = new XElement(HOMEWORKSLIST, userData.homeworks.Select(x =>
                                      new XElement(HOMEWORK,
                                                new XAttribute(nameof(x.Date), x.Date.Value.ToShortDateString()),
                                                new XAttribute(nameof(x.Subject), x.Subject),
                                                new XAttribute(nameof(x.Job), x.Job),
                                                new XAttribute(nameof(x.IsTest), x.IsTest.ToString()))
                                ));
            root.Add(xmlHomeworks);

            //ADD SETTINGS

            //Subject List
            XElement xmlSettings = new XElement(SETTINGS);
            xmlSettings.Add(new XElement(SUBJECTSLIST, userData.settings.SubjectsStrings.Select(x =>
                                        new XElement(nameof(Homework.Subject), x))));
            root.Add(xmlSettings);

            //Shortcut words
            xmlSettings.Add(new XElement(SHORTCUTWORDSList, userData.settings.ShortcutWords.Select(x =>
                                         new XElement(SHORTCUTWORD, x))));

            //Add school grades
            SchoolGrades sg = userData.schoolGrades;
            XElement xmlSchoolGrades = new XElement(SCHOOLGRADES,
                                                     new XAttribute(nameof(sg.ShowComments), sg.ShowComments),
                                                     new XAttribute(nameof(SchoolGrades.HighlighResult), sg.HighlighResult));
            root.Add(xmlSchoolGrades);
            sg.Subjects.ForEach(s =>
            {
                XElement xmlSub = new XElement(GRADESSUBJECT,
                                                    new XAttribute(nameof(Subject.Name), s.Name),
                                                    new XAttribute(nameof(Subject.Coeff), s.Coeff));
                xmlSub.Add(s.grades.Select(g =>
                    new XElement(GRADE,
                                    new XAttribute(nameof(Grade.TheGrade), g.TheGrade),
                                    new XAttribute(nameof(Grade.outOf), g.outOf),
                                    new XAttribute(nameof(Grade.Coeff), g.Coeff))
                ));

                xmlSchoolGrades.Add(xmlSub);
            });

            //Add schedule
            XElement xmlSchedule = new XElement(SCHEDULE);
            root.Add(xmlSchedule);
            Subjects subs = userData.settings.Subjects;
            XElement xmlSubjectsColors = new XElement(SUBJECTSCOLORSLIST, subs.TheSubjects.Select(x =>
                                                                           new XElement(SUBJECTCOLOR,
                                                                                new XAttribute(SUBJECTCOLOR_NAME, x),
                                                                                new XAttribute(SUBJECTCOLOR_COLOR, ColorTranslator.ToHtml(subs.GetColorOf(x))))));
            xmlSchedule.Add(xmlSubjectsColors);
            XElement xmlAppointments = new XElement(APPOINTMENTSLIST, userData.scheduleAppointments.Select(x =>
                                                                        new XElement(APPOINTMENT,
                                                                            new XAttribute(nameof(Appointment.Title), x.Title),
                                                                            new XAttribute(nameof(Appointment.StartTime), Appointment.GenerateNormalizedTime(1, x.StartTime.Hour, x.StartTime.Minute).ToString()),
                                                                            new XAttribute(nameof(Appointment.EndTime), Appointment.GenerateNormalizedTime(1, x.EndTime.Hour, x.EndTime.Minute).ToString()),
                                                                            new XAttribute(DAYINDEX, x.StartTime.Day))));
            xmlSchedule.Add(xmlAppointments);

            //Add reminders
            XElement xmlReminders = new XElement(REMINDERLIST, userData.reminders.Select(x =>
                                                                new XElement(REMINDER, x)));
            root.Add(xmlReminders);

            return root;
        }

        /// <summary>
        /// Read xml code and return a Save object
        /// </summary>
        /// <param name="xmlCode">String XML code to read</param>
        /// <returns>Return Save object containind read data in XML code</returns>
        public static Save Read(string xmlCode)
        {
            if (string.IsNullOrEmpty(xmlCode))
                return null;

            Save save = new Save();

            //Read XML file
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(xmlCode);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Vos données sont corrompus.");
                return new Save();
            }

            //Get homeworks list
            XmlNodeList xmlHomeworks = xml.GetElementsByTagName(HOMEWORK);
            List<Homework> hmsList = new List<Homework>();
            Homework h;
            foreach (XmlNode node in xmlHomeworks)
            {
                h = new Homework(
                                 DateTime.Parse(node.Attributes[nameof(h.Date)]?.InnerText),
                                 node.Attributes[nameof(h.Subject)]?.InnerText,
                                 node.Attributes[nameof(h.Job)]?.InnerText,
                                 bool.Parse(node.Attributes[nameof(h.IsTest)]?.InnerText)
                               );
                hmsList.Add(h);
            }
            save.homeworks = hmsList;

            //Get settings
            XmlNodeList xmlSettings = xml.GetElementsByTagName(SETTINGS);

            //Subjects
            Settings settings = new Settings();
            List<string> subs = new List<string>();
            XmlNodeList xmlSubjects = xml.GetElementsByTagName(nameof(Homework.Subject));
            foreach (XmlNode node in xmlSubjects)
            {
                subs.Add(node?.InnerText);

            }
            if (subs.Count > 0)
                settings.SubjectsStrings = subs.ToArray();

            save.settings = settings;

            //School Grades
            XmlNodeList XmlSchoolGradesParent = xml.GetElementsByTagName(SCHOOLGRADES);
            if (XmlSchoolGradesParent.Count <= 0)
                save.schoolGrades = null;
            foreach (XmlNode xsgp in XmlSchoolGradesParent)
            {
                save.schoolGrades.ShowComments = bool.Parse(xsgp.Attributes[nameof(SchoolGrades.ShowComments)].Value);
                save.schoolGrades.HighlighResult = bool.Parse(xsgp.Attributes[nameof(SchoolGrades.HighlighResult)].Value);
            }

            List<Subject> schoolSubjects = new List<Subject>();
            XmlNodeList xmlGradesSubject = xml.GetElementsByTagName(GRADESSUBJECT);
            foreach (XmlNode s in xmlGradesSubject)
            {//s = subject
                string sName = s.Attributes[nameof(Subject.Name)].Value;

                string sCoeffString = s.Attributes[nameof(Subject.Coeff)].Value;
                float sCoeff = float.Parse(sCoeffString, NumberStyles.Any, CultureInfo.InvariantCulture);

                Subject subjectObj = new Subject(sName, sCoeff);

                foreach (XmlNode g in s.ChildNodes)
                {//g = grade
                    string gradeString = g.Attributes[nameof(Grade.TheGrade)].Value;
                    string outOfString = g.Attributes[nameof(Grade.outOf)].Value;
                    string gCoeffString = g.Attributes[nameof(Grade.Coeff)].Value;

                    float grade = float.Parse(gradeString, NumberStyles.Any, CultureInfo.InvariantCulture);
                    float outOf = float.Parse(outOfString, NumberStyles.Any, CultureInfo.InvariantCulture);
                    float gCoeff = float.Parse(gCoeffString, NumberStyles.Any, CultureInfo.InvariantCulture);

                    Grade gradeObj = new Grade(grade, outOf, gCoeff);
                    subjectObj.grades.Add(gradeObj);
                }

                schoolSubjects.Add(subjectObj);
            }
            if (save.schoolGrades != null)
                save.schoolGrades.Subjects = schoolSubjects;

            //Schedule
            //SubjectsColors
            XmlNodeList xmlSubjectsColorParent = xml.GetElementsByTagName(SUBJECTSCOLORSLIST);
            if (xmlSubjectsColorParent.Count <= 0)
                settings.Subjects = null;
            XmlNodeList xmlSubjectsColorsList = xml.GetElementsByTagName(SUBJECTCOLOR);
            foreach (XmlNode s in xmlSubjectsColorsList)
            {//s = subjectColor
                string sName = s.Attributes[SUBJECTCOLOR_NAME].Value;
                string sColor = s.Attributes[SUBJECTCOLOR_COLOR].Value;

                settings.Subjects.SetColorOf(sName, ColorTranslator.FromHtml(sColor));
            }
            //Appointments
            XmlNodeList xmlAppointmentsParent = xml.GetElementsByTagName(APPOINTMENTSLIST);
            if (xmlAppointmentsParent.Count <= 0)
                save.scheduleAppointments = null;
            XmlNodeList xmlAppointments = xml.GetElementsByTagName(APPOINTMENT);
            foreach (XmlNode a in xmlAppointments)
            {//a = Appointment
                string text = a.Attributes[nameof(Appointment.Title)].InnerText;
                DateTime startTime = DateTime.Parse(a.Attributes[nameof(Appointment.StartTime)].InnerText);
                DateTime endTime = DateTime.Parse(a.Attributes[nameof(Appointment.EndTime)].InnerText);
                int dayIndex = Convert.ToInt32(a.Attributes[DAYINDEX].InnerText);
                startTime = Appointment.GenerateNormalizedTime(dayIndex, startTime.Hour, startTime.Minute);
                endTime = Appointment.GenerateNormalizedTime(dayIndex, endTime.Hour, endTime.Minute);
                save.scheduleAppointments.Add(new Appointment(text, startTime, endTime));
            }

            //Reminders
            XmlNodeList xmlReminderParent = xml.GetElementsByTagName(REMINDERLIST);
            if (xmlReminderParent.Count <= 0)
                save.reminders = null;
            XmlNodeList xmlReminders = xml.GetElementsByTagName(REMINDER);
            foreach (XmlNode r in xmlReminders)
            {
                save.reminders.Add(r.InnerText);
            }

            return save;
        }


    }
}
