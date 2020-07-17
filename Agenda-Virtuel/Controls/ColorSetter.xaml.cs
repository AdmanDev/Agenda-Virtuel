using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Agenda_Virtuel.Controls
{
    internal partial class ColorSetter : UserControl
    {
        //Variables
        private System.Drawing.Color selectedColor;

        //Properties
        public string Title
        {
            get => this.BT_ChooseColor.Content as string;
            set => this.BT_ChooseColor.Content = value;
        }
        public System.Drawing.Color SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                this.Grid_Preview.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(value.R, value.G, value.B));
            }
        }

        //Events
        public event Action<System.Windows.Media.Color> SelectedColorChanged;

        //Constructor
        public ColorSetter()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        private void BT_ChooseColor_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedColor = colorDialog.Color;
                SelectedColorChanged?.Invoke(System.Windows.Media.Color.FromRgb(SelectedColor.R, SelectedColor.G, SelectedColor.B));
            }
        }

        public void SetColor(System.Windows.Media.Brush color)
        {
            if (color is SolidColorBrush colorBrush)
            {
                SelectedColor = System.Drawing.Color.FromArgb(colorBrush.Color.R, colorBrush.Color.G, colorBrush.Color.B);
            }
            else if(color is LinearGradientBrush linearBrush)
            {
                if(linearBrush.GradientStops.Count > 0)
                    SelectedColor = System.Drawing.Color.FromArgb(linearBrush.GradientStops[0].Color.R, linearBrush.GradientStops[0].Color.G, linearBrush.GradientStops[0].Color.B);
            }

        }

    }

}
