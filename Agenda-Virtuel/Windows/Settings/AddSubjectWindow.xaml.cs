using System;
using System.Windows;
using System.Windows.Media;
using forms = System.Windows.Forms;
using Color = System.Drawing.Color;
using WpfColor = System.Windows.Media.Color;
using Agenda_Virtuel.Manager;
using System.Windows.Forms;

namespace Agenda_Virtuel.Windows.Settings
{
    internal partial class AddSubjectWindow : Window
    {
        //Variables
        private readonly forms.ColorDialog colorDialog;

        public AddSubjectWindow()
        {
            InitializeComponent();

            colorDialog = new forms.ColorDialog()
            {
                AnyColor = true,
                AllowFullOpen = true,
                FullOpen = true,
                SolidColorOnly = false,
                Color = Color.Gray
            };
        }

        private void BT_Color_Click(object sender, RoutedEventArgs e)
        {
            if(this.colorDialog.ShowDialog() == forms.DialogResult.OK)
            {
                Color color = this.colorDialog.Color;
                WpfColor backColor = WpfColor.FromRgb(color.R, color.G, color.B);
                this.BT_Color.Background = new SolidColorBrush(backColor);
            }
        }

        private void BT_Validate_Click(object sender, RoutedEventArgs e)
        {
            string name = this.TB_Name.Text;
            double coeff = this.NUD_Coeff.Value;
            Color color = this.colorDialog.Color;

            if (!string.IsNullOrEmpty(name))
            {
                Subject subject = new Subject(name, (float)coeff, color);
                SettingsManager.AddSubject(subject);

                this.DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Saisissez le nom de la matière !", 
                                                "Agenda - Virtuel", 
                                                MessageBoxButton.OK, 
                                                MessageBoxImage.Information);
                this.TB_Name.Focus();
            }
        }
    }
}
