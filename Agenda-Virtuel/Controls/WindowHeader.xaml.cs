using System.Windows;
using System.Windows.Controls;
using MyFunctions;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This control allows to diplay window header as the application windows, with the title of your window, a button to minimize and a button to close your window.
    /// </summary>
    public partial class WindowHeader : UserControl
    {
        //Variables
        private Window w; //The window of this control

        private WPF_ADMANMenu admanMenu;
        
        //Constructor
        public WindowHeader()
        {
            InitializeComponent();
            admanMenu = new WPF_ADMANMenu();
        }

        //On load
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            w = Window.GetWindow(this);

            if (w != null)
                this.LB_Title.Text = w.Title;
        }

        //Close the window or the application
        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            if (w == Global.mainWindow)
                Application.Current.Shutdown();
            else
                w.Close();
        }

        //Move the window
        private void Grid_Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            w.DragMove();
        }

        //Show ADMAN Software-FR menu
        private void BT_ADMANSoftware_Click(object sender, RoutedEventArgs e)
        {
            admanMenu.ShowMenu();
        }

        private void BT_Minimize_Click(object sender, RoutedEventArgs e)
        {
            w.WindowState = WindowState.Minimized;
        }
    }
}
