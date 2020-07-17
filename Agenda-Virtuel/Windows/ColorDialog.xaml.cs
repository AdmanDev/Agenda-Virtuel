using System;
using System.Windows;
using System.Windows.Media;

namespace Agenda_Virtuel.Windows
{
    /// <summary>
    /// This window allows to user to choose a Brush (WPF color)
    /// </summary>
    public partial class ColorDialog : Window
    {
        //Properties
        /// <summary>
        /// The Brush (WPF color) selected by user.
        /// </summary>
        public Brush SelectedBrush { get; private set; }

        //Constructor
        /// <summary>
        /// Instantiate new ColorDialog window
        /// </summary>
        public ColorDialog()
        {
            InitializeComponent();
        }

        private void BT_OK_Click(object sender, RoutedEventArgs e)
        {
            SelectedBrush = editor.Brush;

            DialogResult = true;
            this.Close();
        }

        private void BT_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
