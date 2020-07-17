using System;
using System.Windows.Controls;

namespace Agenda_Virtuel
{
    /// <summary>
    /// Allows to create (or get) a homewok viewer that displays homework informations.
    /// If you create your own viewer use this interface on a WPF UserControl. also, you have to create a <c>IHomeworkViewerContainer</c>
    /// </summary>
    public interface IHomeworkViewer
    {
        //Properties
        /// <summary>
        /// This is the homework to display
        /// </summary>
        Homework Homework { get; }
        /// <summary>
        /// The ContextMenu of your viewer.
        /// </summary>
        ContextMenu ViewerContextMenu { get; }

        //Functions
        /// <summary>
        /// This method is executed to update the homework (informations)
        /// </summary>
        /// <param name="_homework">The new homework to display</param>
        void UpdateHomework(Homework _homework);
        /// <summary>
        /// This method allows to delete the homeworks from data.
        /// You have to use : <c>HomeworkManager</c>.DeleteHomework(this.Homework); ...to delete homeworks
        /// </summary>
        void DeleteHomework();
        /// <summary>
        /// This method allows to highlight the homework / viewer.
        /// </summary>
        /// <param name="enabled">True = highlight else False</param>
        void Highlight(bool enabled);
        /// <summary>
        /// This method is executed to update colors of the viewer (homeworks text color, subject text color...)
        /// </summary>
        void Recolor();
        /// <summary>
        /// This method is executed to update fonts of the viewer (homeworks text font, subject text font...)
        /// </summary>
        void Refont();
        /// <summary>
        /// This method is executed to add menus from plugins
        /// </summary>
        /// <param name="menus">Menu items to add to the ContextMenu</param>
        void AddPluginMenus(MenuItem[] menus);
    }
}
