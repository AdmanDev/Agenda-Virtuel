using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Cahier_de_texte_en_ligne
{
    public partial class Settings_Control : System.Windows.Controls.UserControl
    {
        public Settings_Control()
        {
            InitializeComponent();
            this.IMG_Paste.Source = BitmapToImageSource(Properties.Resources.PasteIcon);

            bool flag = !string.IsNullOrEmpty(Main.mdp);
            if (flag)
            {
                this.TB_MDP.Text = Main.mdp;
            }
            bool flag2 = !string.IsNullOrEmpty(Main.cahierID);
            if (flag2)
            {
                this.TB_ID.Text = Main.cahierID;
                LoadClasses(Main.cahierID + "/");
            }

            this.BT_Save.Style = Agenda_Virtuel.Manager.SettingsManager.GetSettings().Styles.PrimaryButtonStyle;
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void LoadClasses(string cahierID)
        {
            System.Windows.Forms.WebBrowser wb = new System.Windows.Forms.WebBrowser();
            wb.DocumentCompleted += delegate (object sender, WebBrowserDocumentCompletedEventArgs e)
            {
                HtmlElementCollection all = wb.Document.Body.GetElementsByTagName("select")[0].All;
                this.CB_Classes.Items.Clear();
                foreach (HtmlElement htmlElement in all)
                {
                    this.CB_Classes.Items.Add(new ComboBoxItem
                    {
                        Content = htmlElement.InnerText,
                        Tag = htmlElement.GetAttribute("value")
                    });
                    bool flag = Main.classeID.ToString() == htmlElement.GetAttribute("value");
                    if (flag)
                    {
                        this.CB_Classes.SelectedIndex = this.CB_Classes.Items.Count - 1;
                    }
                }
            };
            wb.Navigate("https://edu-cdt.ac-versailles.fr/" + cahierID);
        }

        private void TB_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string value = this.TB_ID.Text;
            string value2 = this.TB_MDP.Text;
            string s = ((ComboBoxItem)this.CB_Classes.SelectedItem).Tag as string;
            bool flag = value.Contains("/");
            if (flag)
            {
                value.Replace("/", "");
            }
            Main.instance.SetSetting("cahierID", value);
            Main.instance.SetSetting("classeID", int.Parse(s));
            bool flag2 = this.TB_MDP.Text == "Mot de passe";
            if (flag2)
            {
                Main.instance.SetSetting("mdp", "");
            }
            else
            {
                Main.instance.SetSetting("mdp", value2);
            }
            Main.instance.LoadProperties();
            Main.instance.FindHomeworks();

            Window.GetWindow(this).Close();
        }

        private void TB_ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.LoadClasses(this.TB_ID.Text + "/");
        }

        private void BT_Paste_Click(object sender, RoutedEventArgs e)
        {
            this.TB_ID.Text = System.Windows.Clipboard.GetText();
            this.LoadClasses(this.TB_ID.Text + "/");
        }
    }
}
