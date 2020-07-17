using System;
using System.Windows;

namespace Agenda_Virtuel.Windows.Shedule
{
    internal partial class CustomizeSubjectsColorsWindow : Window
    {
        public CustomizeSubjectsColorsWindow()
        {
            InitializeComponent();

            ShowSubjects();
        }

        private void ShowSubjects()
        {
            this.SP_Colors.Children.Clear();

            foreach (string s in Global.userData.settings.SubjectsStrings)
            {
                this.SP_Colors.Children.Add(new ColorEditor(s));
            }
        }
    }
}
