using Agenda_Virtuel.Manager;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This is school grades window
    /// </summary>
    internal partial class SchoolGradesWindow : Window
    {
        //Enums
        private static class Comments
        {
            internal const string VeryBad_0_7 = "Votre moyenne est très faible, essayez de faire plus d'efforts ! :)";
            internal const string Bad_8_9 = "Vous avez presque la moyenne ! Courage ! :p";
            internal const string Good_10_13 = "Vous êtes au dessus de la moyenne, bravo ! Poursuivez vos efforts ! ;)";
            internal const string VeryGood_14_20 = "Félicitation ! Vous avez une très bonne moyenne ! :D";
        }

        //Variables
        private Subject selectedSubject;

        //Properties
        public bool Highlight { get; private set; }

        //Constructor
        public SchoolGradesWindow()
        {
            InitializeComponent();

            EventsManager.DatasDownloaded += OnDatasDownloaded;
        }

        private void OnDatasDownloaded(Save obj)
        {
            UpdateAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            Highlight = Global.userData.schoolGrades.HighlighResult;
            LoadSubjects();
        }

        private void ShowSummary()
        {
            List<Subject> _subjects = new List<Subject>(Global.userData.schoolGrades.Subjects);
            _subjects.Insert(0, Global.userData.schoolGrades.OverallAverage);

            this.DG_Summary.ItemsSource = null;
            this.DG_Summary.ItemsSource = _subjects;
        }

        private void LoadSubjects()
        {
            this.CBB_Subjects.Items.Clear();

            Global.userData.settings.Subjects.ForEach
                (
                    x =>
                    {
                        this.CBB_Subjects.Items.Add(x);
                    }
                );

            if (this.CBB_Subjects.Items.Count > 0)
                this.CBB_Subjects.SelectedIndex = 0;
            else
                Reset();
        }


        private void CBB_Subjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;

            selectedSubject = (Subject)e.AddedItems[0];
            ShowSubject();
        }

        private void DG_Summary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;

            this.CBB_Subjects.SelectedItem = (Subject)e.AddedItems[0];

        }

        private void ShowSubject()
        {
            if (selectedSubject == null)
                return;

            this.DG_Grades.ItemsSource = null;
            this.DG_Grades.ItemsSource = selectedSubject.grades;

            ShowSubjectInfos();
            ShowSummary();
        }

        private void ShowSubjectInfos()
        {
            if (selectedSubject == null)
                return;

            this.TB_Average.Text = selectedSubject.Average + " / 20";
            this.TB_BestGrade.Text = selectedSubject.BestGrade + " / 20";
            this.TB_WorstGrade.Text = selectedSubject.WorstGrade + " / 20";

            if (Global.userData.schoolGrades.ShowComments)
            {
                string comment;
                double average = selectedSubject.Average;

                if (average <= 7)
                    comment = Comments.VeryBad_0_7;
                else if (average <= 9)
                    comment = Comments.Bad_8_9;
                else if (average <= 13)
                    comment = Comments.Good_10_13;
                else
                    comment = Comments.VeryGood_14_20;

                this.TB_Comment.Text = comment;
            }
            else
            {
                this.TB_Comment.Text = "Commentaire :";
            }
        }

        private void Reset()
        {
            this.TB_Average.Text = "?";
            this.TB_BestGrade.Text = "?";
            this.TB_WorstGrade.Text = "?";
            this.TB_Comment.Text = "Commentaire :";

            selectedSubject = null;
        }

        private void BT_AddGrade_Click(object sender, RoutedEventArgs e)
        {
            if (this.Panel_AddGrade.Visibility == Visibility.Visible)
                this.Panel_AddGrade.Visibility = Visibility.Collapsed;
            else
                this.Panel_AddGrade.Visibility = Visibility.Visible;
        }

        private void ShowGradeOutOf20()
        {
            if (this.LB_GradeOutOf20 == null || this.NUD_AddGrade_Value == null || this.NUD_AddGrade_OutOf == null)
                return;

            double grade = this.NUD_AddGrade_Value.Value;
            double outOf = this.NUD_AddGrade_OutOf.Value;
            double result = (grade / outOf) * 20;

            this.LB_GradeOutOf20.Content = Math.Round(result, 2) + " / 20";
        }

        private void NUD_AddGrade_Value_ValueChanged(double value)
        {
            ShowGradeOutOf20();
        }

        private void NUD_AddGrade_OutOf_ValueChanged(double value)
        {
            this.NUD_AddGrade_Value.Max = value;
            ShowGradeOutOf20();
        }

        //Add grade
        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSubject == null)
                return;

            float grade = (float)this.NUD_AddGrade_Value.Value;
            float outOf = (float)this.NUD_AddGrade_OutOf.Value;
            float coeff = (float)this.NUD_AddGrade_Coef.Value;

            SchoolGradesManager.AddGrade(selectedSubject, new Grade(grade, outOf, coeff));
            ShowSubject();

            this.Panel_AddGrade.Visibility = Visibility.Collapsed;

            this.NUD_AddGrade_Value.Value = 10;
            this.NUD_AddGrade_OutOf.Value = 20;
            this.NUD_AddGrade_Coef.Value = 1;
        }

        private void BT_DeleteGrade_Click(object sender, RoutedEventArgs e)
        {
            if (this.DG_Grades.SelectedIndex >= 0)
            {
                Grade gradeToDelete = (Grade)this.DG_Grades.SelectedItem;
                SchoolGradesManager.RemoveGrade(selectedSubject, gradeToDelete);

                ShowSubject();
            }
        }

        private void BT_NewTrimester_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Voulez vous vraiment commencer un nouveau trimèstre ? Les notes seront effacées.";
            if (System.Windows.Forms.MessageBox.Show(msg, "Agenda - Virtuel", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                SchoolGradesManager.NewTrimester(Global.userData.schoolGrades);
                ShowSubject();
            }

        }

    }
}
