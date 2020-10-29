using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;
using Agenda_Virtuel.Windows;

namespace Agenda_Virtuel.Controls
{
    internal partial class StyleSetter : UserControl
    {
        //Variables
        private Style style;
        private Type ownerType;

        //Properties
        public Style SStyle
        {
            get => style;
            set
            {
                if (value != null)
                {
                    ownerType = value.TargetType;
                    style = new Style(ownerType);
                    foreach (Setter s in value.Setters)
                    {
                        style.Setters.Add(s);
                    }

                    foreach (Trigger t in value.Triggers)
                    {
                        style.Triggers.Add(t);
                    }

                    foreach (ResourceDictionary r in value.Resources)
                    {
                        style.Resources.MergedDictionaries.Add(r);
                    }

                    ShowStyle();
                }

                if (value.TargetType == typeof(Window))
                    this.Group_Background.Visibility = Visibility.Collapsed;
                else
                    this.Group_Background.Visibility = Visibility.Visible;

                StylesChanged?.Invoke(GetStyle());
            }
        }

        public Type OwnerType
        {
            get => ownerType;
            set
            {
                ownerType = value;

                style = new Style(value);

                if (SStyle != null)
                    SStyle.TargetType = value;
            }
        }

        //Events
        public event Action<Style> StylesChanged;

        //Constructor
        public StyleSetter()
        {
            InitializeComponent();

            style = new Style();

        }

        public Style GetStyle()
        {
            Style s = new Style(OwnerType);
            foreach (Setter se in SStyle.Setters)
            {
                s.Setters.Add(se);
            }

            return s;
        }

        private void ShowStyle()
        {
            this.BackgroundPreview.Background = GetProperty(Control.BackgroundProperty)?.Value as Brush;
            this.ForegroundPreview.Background = GetProperty(Control.ForegroundProperty)?.Value as Brush;
            this.BorderBrushPreview.Background = GetProperty(Control.BorderBrushProperty)?.Value as Brush;

            Setter s = GetProperty(Control.BorderThicknessProperty);
            if (s != null && s.Value != null)
                this.NUD_BorderThickness.Value = ((Thickness)s.Value).Left;

            this.LB_FontFamily.Text = ((FontFamily)GetProperty(Control.FontFamilyProperty)?.Value)?.ToString();
        }

        private Setter GetProperty(DependencyProperty property)
        {

            return (Setter)SStyle.Setters.FirstOrDefault(x => ((Setter)x).Property == property);
        }

        private void SetProperty(DependencyProperty property, object value)
        {
            Setter s = GetProperty(property);
            if (s != null)
            {
                style.Setters.Remove(s);
            }

            s = new Setter(property, value);
            SStyle.Setters.Add(s);

            StylesChanged?.Invoke(GetStyle());
        }

        private void BT_ChooseBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == true)
            {
                SetProperty(Control.BackgroundProperty, colorDialog.SelectedBrush);
                ShowStyle();
            }
        }

        private void BT_ChooseForeground_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == true)
            {
                SetProperty(Control.ForegroundProperty, colorDialog.SelectedBrush);
                ShowStyle();
            }
        }

        private void BT_ChooseBorderBrush_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == true)
            {
                SetProperty(Control.BorderBrushProperty, colorDialog.SelectedBrush);
                ShowStyle();
            }
        }

        private void NUD_BorderThickness_ValueChanged(double value)
        {
            if(value < 0)
            {
                this.NUD_BorderThickness.Value = 0;
                return;
            }

            SetProperty(Control.BorderThicknessProperty, new Thickness(value));
        }

        private void BT_ChooseFont_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog();
            if(fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FontGroup fg = new FontGroup(fd.Font);
                SetProperty(Control.FontFamilyProperty, fg.Font_Family);
                SetProperty(Control.FontSizeProperty, fg.fontSize);
                SetProperty(Control.FontStyleProperty, fg.Font_Style);
                SetProperty(Control.FontWeightProperty, fg.Font_Weight);

                ShowStyle();
            }
        }

     
 
    }
}
