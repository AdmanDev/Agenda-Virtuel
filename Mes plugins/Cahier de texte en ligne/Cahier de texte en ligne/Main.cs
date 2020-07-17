using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Agenda_Virtuel;
using Agenda_Virtuel.Plugin;
using Agenda_Virtuel.Manager;

namespace Cahier_de_texte_en_ligne
{
    public class Main : Plugin
    {
        public static Main instance;

        private WebBrowser wb = new WebBrowser();

        public const string cahierUrl = "https://edu-cdt.ac-versailles.fr/";

        public static string cahierID;

        public static int? classeID;

        public static string mdp;

        private List<Homework> homeworks;

        public static List<Homework> hmsDeletedDefinitely;

        public void LoadProperties()
        {
            Main.cahierID = base.GetSetting<string>("cahierID");
            Main.classeID = new int?(base.GetSetting<int>("classeID"));
            Main.mdp = base.GetSetting<string>("mdp");
            Main.hmsDeletedDefinitely = base.GetSetting<List<Homework>>("hmsDeletedDefinitely");
        }

        protected override void OnStartup()
        {
            Main.instance = this;
            this.SetSettingWindow(new Settings_Control());

            this.LoadProperties();
            bool flag = string.IsNullOrEmpty(Main.cahierID);
            if (flag)
            {
                base.DisplayPluginSettings();
            }
            this.wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.Wb_DocumentCompleted);
            this.FindHomeworks();
        }

        public void FindHomeworks()
        {
            this.wb.Navigate("https://edu-cdt.ac-versailles.fr/" + Main.cahierID + "/");
        }

        private void Wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            bool flag = this.wb.Url.AbsoluteUri == "https://edu-cdt.ac-versailles.fr/" + Main.cahierID + "/";
            if (flag)
            {
                this.wb.Document.Body.GetElementsByTagName("select")[0].SetAttribute("value", Main.classeID.ToString());
                this.wb.Document.Body.GetElementsByTagName("input")[0].SetAttribute("value", Main.mdp);
                this.wb.Document.Body.GetElementsByTagName("input")[1].InvokeMember("click");
            }
            else
            {
                this.homeworks = this.GetNewHomeworks();
                HomeworkManager.SaveHomeworks(this.homeworks);
                HomeworkManager.ShowHomeworksOfSelectedDate();
                base.SaveDatas();
            }
        }

        private List<Homework> GetNewHomeworks()
        {
            List<Homework> list = new List<Homework>();
            HtmlElementCollection elementsByTagName = this.wb.Document.Body.GetElementsByTagName("tr");
            foreach (HtmlElement htmlElement in elementsByTagName)
            {
                string text = "";
                string text2 = "";
                string text3 = "";
                bool flag = false;
                int num = 0;
                foreach (HtmlElement htmlElement2 in htmlElement.GetElementsByTagName("b"))
                {
                    bool flag2 = string.IsNullOrEmpty(text2);
                    if (flag2)
                    {
                        text2 = htmlElement.GetElementsByTagName("td")[4].OuterText;
                    }
                    bool flag3 = num == 2;
                    if (flag3)
                    {
                        text3 = htmlElement2.OuterText;
                        break;
                    }
                    bool flag4 = htmlElement2.OuterText != "DEVOIR";
                    if (flag4)
                    {
                        int num2 = num;
                        if (num2 != 0)
                        {
                            if (num2 == 1)
                            {
                                text3 = htmlElement2.OuterText;
                            }
                        }
                        else
                        {
                            text = htmlElement2.OuterText;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    num++;
                }
                bool flag5 = string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3);
                if (!flag5)
                {
                    string[] array = text3.Split(new char[]
                    {
                        ' '
                    });
                    text3 = array[1];
                    DateTime dateTime = DateTime.ParseExact(text3, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    list.Add(new Homework(dateTime, text, text2, flag));
                }
            }
            foreach (Homework current in HomeworkManager.HomeworksList)
            {
                foreach (Homework current2 in list)
                {
                    bool flag6 = current2.Equals(current);
                    if (flag6)
                    {
                        list.Remove(current2);
                        break;
                    }
                }
            }
            foreach (Homework current3 in Main.hmsDeletedDefinitely)
            {
                foreach (Homework current4 in list)
                {
                    bool flag7 = current4.Equals(current3);
                    if (flag7)
                    {
                        list.Remove(current4);
                        break;
                    }
                }
            }
            return list;
        }
    }
}
