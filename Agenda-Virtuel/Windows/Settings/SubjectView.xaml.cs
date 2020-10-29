using Agenda_Virtuel.Manager;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agenda_Virtuel.Windows.Settings
{
    internal partial class SubjectView : UserControl
    {
        //Variables
        private readonly Subject subject;

        //Constructor
        public SubjectView(Subject subject)
        {
            InitializeComponent();

            this.subject = subject;
            ShowSubjectInfo();
        }

        private void ShowSubjectInfo()
        {
            Color mColor = Color.FromRgb(subject.Color.R, subject.Color.G, subject.Color.B);
            this.EL_Color.Fill = new SolidColorBrush(mColor);
            this.LB_Subject.Content = subject.Name;
        }

        private void BT_Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingsManager.DeleteSubject(subject);
        }
    }
}
