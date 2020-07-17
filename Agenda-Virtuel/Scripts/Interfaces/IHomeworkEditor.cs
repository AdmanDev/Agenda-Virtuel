namespace Agenda_Virtuel
{
    /// <summary>
    /// This interface allows to create a homework editor. Use it on a WPF UserControl.
    /// </summary>
    public interface IHomeworkEditor
    {
        //Properties
        /// <summary>
        /// The mode of editor
        /// </summary>
        HomeworkEditorMode Mode { get; set; }

        //Functions
        /// <summary>
        /// Method executed whenever user edits a homework. The editor mode will return to "Change" mode
        /// </summary>
        /// <param name="homeworkToChange">This is the homework to change</param>
        void BeginChangeHomework(Homework homeworkToChange);
        /// <summary>
        /// Method executed to automatically fill the subject field
        /// </summary>
        /// <param name="subject">The subject to set to "subject" field</param>
        void SetSubject(string subject);
    }
}
