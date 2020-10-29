using Agenda_Virtuel.Manager;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Agenda_Virtuel.Windows.Shedule
{
    internal partial class ColorEditor : System.Windows.Controls.UserControl
    {
        //Variables
        private readonly Subject subject;
        private System.Drawing.Color color;
        private System.Windows.Forms.ColorDialog cd;

        //Properties
        public System.Drawing.Color SelectedColor
        {
            get => color;
            set
            {
                color = value;
                this.LB_Subject.Foreground = new SolidColorBrush(
                    System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B)
                    );
            }
        }

        //Constructor
        public ColorEditor(Subject _subject)
        {
            InitializeComponent();

            cd = new System.Windows.Forms.ColorDialog()
            {
                AllowFullOpen = true,
                AnyColor = true,
                FullOpen = true,
                SolidColorOnly = false
            };

            subject = _subject;
            SelectedColor = subject.Color;

            this.LB_Subject.Content = _subject.Name;
        }

        private void BT_ChangeColor_Click(object sender, RoutedEventArgs e)
        {
            if(cd.ShowDialog() == DialogResult.OK)
            {
                SelectedColor = cd.Color;

                ScheduleManager.SetSubjectColor(subject, cd.Color);
            }
        }
    }
}
