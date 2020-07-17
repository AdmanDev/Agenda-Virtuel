using System;
using System.Windows.Controls;
using Agenda_Virtuel;
using Agenda_Virtuel.Manager;
using ViewersPlugin.Viewers.Schedule;

namespace ViewersPlugin
{
    public partial class SHDViewer : UserControl, IHomeworkViewerContainer
    {
        //Properties
        public Type ViewerItemType { get; set; }

        //Constructor
        public SHDViewer()
        {
            InitializeComponent();

            ViewerItemType = typeof(SHDViewerItem);
        }

        public void Clear()
        {
            this.SP_Hms.Children.Clear();
        }

        public void DisplayHomework(IHomeworkViewer _homeworkViewerItem)
        {
            this.SP_Hms.Children.Add(_homeworkViewerItem as UserControl);
        }

        public void RemoveViewer(IHomeworkViewer viewer)
        {
            this.SP_Hms.Children.Remove(viewer as UserControl);
        }
    }
}
