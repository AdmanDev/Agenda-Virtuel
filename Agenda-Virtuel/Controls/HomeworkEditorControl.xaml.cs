using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Agenda_Virtuel.Manager;
using MyFunctions;
using VelerSoftware.VoiceRecognitionLib;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This is the default homework editor. You can replace it by your own editor.
    /// </summary>
    /// <remarks>This UserControl inherits from the <c>IHomeworkEditor</c> interface.</remarks>
    public partial class HomeworkEditorControl : UserControl, IHomeworkEditor
    {
        //Variables
        private Homework HomeworkToEdit;
        private string defaultCB_SebjectText;

        private VoiceRecognition voiceRecorder;
        private bool isVoiceRecording;

        //Properties
        private Settings Settings { get; set; }
        /// <summary>
        /// Get the current mode of the editor.
        /// Don't set it otherwise there will be a bug !
        /// </summary>
        /// <remarks><seealso cref="HomeworkEditorMode"/></remarks>
        public HomeworkEditorMode Mode { get; set; }

        //Constructor
        public HomeworkEditorControl()
        {
            InitializeComponent();

            Mode = HomeworkEditorMode.Add;

            voiceRecorder = new VoiceRecognition();
            voiceRecorder.OnSpeechRecognized += OnSpeechRecognized;
            isVoiceRecording = false;

            defaultCB_SebjectText = this.CB_Subject.Text;
            
            LoadSettings();

            EventsManager.DatasDownloaded += OnDatasDownloaded;
            EventsManager.SubjectsListChanged += OnSubjectsListChanged;
            EventsManager.ShortcutWordsChanged += OnShortcutWordsChanged;
        }

        private void LoadSettings()
        {
            LoadSubjectsList();
            LoadShortcutWords();
        }

        private void OnDatasDownloaded(Save save)
        {
            LoadSettings();
        }

        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            string subject = this.CB_Subject.Text;
            string job = this.TB.Text;
            bool isTest = this.CB_IsTest.IsChecked == true;

            if (string.IsNullOrEmpty(subject) || subject == defaultCB_SebjectText || string.IsNullOrEmpty(job))
                return;

            Homework hm = new Homework(HomeworkManager.SelectedDate, subject, job, isTest);

            switch (Mode)
            {
                case HomeworkEditorMode.Add:
                    HomeworkManager.SaveAndShow(hm);
                    break;

                case HomeworkEditorMode.Change:
                    hm.ID = HomeworkToEdit.ID;
                    hm.Date = HomeworkToEdit.Date;
                    HomeworkManager.ReplaceHomework(HomeworkToEdit, hm);
                    break;
            }

            Reset();
        }

        /// <summary>
        /// Begin change a homework and change the editor mode to "Change" mode.
        /// </summary>
        /// <param name="hm">The <c>Homework</c> to change.</param>
        public void BeginChangeHomework(Homework hm)
        {
            Mode = HomeworkEditorMode.Change;
            HomeworkToEdit = hm;

            this.CB_Subject.Text = hm.Subject;
            this.TB.Text = hm.Job;
            this.CB_IsTest.IsChecked = hm.IsTest;
        }

        private void Reset()
        {
            SetSubject(defaultCB_SebjectText);
            this.TB.Text = "";
            this.CB_IsTest.IsChecked = false;
        }

        /// <summary>
        /// Fill the subject field.
        /// </summary>
        /// <param name="subject">The subject.</param>
        public void SetSubject(string subject)
        {
            this.CB_Subject.Text = subject;
        }

        private void LoadSubjectsList()
        {
            this.CB_Subject.Items.Clear();
            this.Settings = Global.userData.settings;

            foreach (Subject s in Settings.Subjects)
            {
                this.CB_Subject.Items.Add(s.Name);
            }
        }

        private void OnSubjectsListChanged(List<Subject> subjects)
        {
            LoadSubjectsList();
        }

        private void LoadShortcutWords()
        {
            this.SP_ShortcutButtons.Children.Clear();
            this.Settings = Global.userData.settings;

            Button bt;
            foreach (string w in this.Settings.ShortcutWords)
            {
                bt = new Button() { Content = w };
                bt.Click += BT_ShortcutWords_Click;

                this.SP_ShortcutButtons.Children.Add(bt);
            }
        }

        private void OnShortcutWordsChanged(string[] words)
        {
            LoadShortcutWords();
        }

        private void BT_ShortcutWords_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string text = button.Content as string;
            ApplyShortcut(text);

            EventsManager.Call_OnWordShorcut_ButtonClick(button, text);
        }

        private void ApplyShortcut(string text)
        {
            if (this.TB.Text.Length <= 0)
            {
                text = text.FirstLetterInUppercase() + " ";
            }
            else
            {
                text = text.ToLower();

                if (!this.TB.Text.EndsWith(" "))
                    text = " " + text;

                text = this.TB.Text + text + " ";
            }

            this.TB.Text = text;
            this.TB.Focus();
            this.TB.CaretIndex = this.TB.Text.Length;
        }

        private void BT_speach_Click(object sender, RoutedEventArgs e)
        {
            if(!isVoiceRecording)
            {
                try
                {
                    voiceRecorder.Start_SpeechRecognition();
                    this.BT_speach.Background = new SolidColorBrush(Colors.Green);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine +
                        "Erreur ! Verifiez qu'un micro soit bien branché.", "Agenda - Virtuel", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                    
                }
            }
            else
            {
                voiceRecorder.Stop_SpeechRecognition();
                this.BT_speach.Background = null;
            }

            isVoiceRecording = !isVoiceRecording;
        }

        private void OnSpeechRecognized(object sender, string e)
        {
            ApplyShortcut(e);
        }

        private void HomeworkWriting()
        {
            EventsManager.Call_OnHomeworkWriting(
                new Homework(
                    HomeworkManager.SelectedDate,
                    this.CB_Subject.Text,
                    this.TB.Text,
                    this.CB_IsTest.IsChecked == true));
        }

        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            HomeworkWriting();
        }

        private void CB_Subject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HomeworkWriting();
        }

        private void CB_IsTest_Checked(object sender, RoutedEventArgs e)
        {
            HomeworkWriting();
        }
    }
}
