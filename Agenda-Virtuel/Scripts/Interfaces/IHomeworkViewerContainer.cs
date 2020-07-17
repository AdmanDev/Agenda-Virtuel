using System;

namespace Agenda_Virtuel
{
    /// <summary>
    /// Allows to create (or get) the container of IHomeworkViewer s. It is a WPF UserControl that contains homework viewers.
    /// You can create your own viewer container if you create your <c>IHomeworkViewer</c>
    /// </summary>
    public interface IHomeworkViewerContainer 
    {
        /*______________________________________PROPERTIES______________________________________*/
        /// <summary>
        /// The type of viewer item that inherit from <c>IHomeworkViewer</c> interfface
        /// </summary>
        Type ViewerItemType { get; set; }

        /*______________________________________FUNCTIONS______________________________________*/
        /// <summary>
        /// This method is executed to display a homework viewer that will be add to this container.
        /// </summary>
        /// <param name="_homeworkViewerItem">Viewer item to display</param>
        void DisplayHomework(IHomeworkViewer _homeworkViewerItem);
        /// <summary>
        /// Remove all viewers
        /// </summary>
        void Clear();
        /// <summary>
        /// Remove a homework viewer
        /// </summary>
        /// <param name="viewer">Viewer to remove</param>
        void RemoveViewer(IHomeworkViewer viewer);
        
    }
}
