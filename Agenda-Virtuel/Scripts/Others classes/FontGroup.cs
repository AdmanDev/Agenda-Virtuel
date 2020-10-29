using System;
using System.Windows;
using System.Windows.Media;

namespace Agenda_Virtuel
{
    /// <summary>
    /// Allow to save / serialize WPF fonts
    /// </summary>
    [Serializable]
    public class FontGroup
    {
        //Variables
        /// <summary>
        /// The name of font family
        /// </summary>
        public string fontFamilyName;
        /// <summary>
        /// The size of the font
        /// </summary>
        public double fontSize;
        /// <summary>
        /// Determine if the font is in italic
        /// </summary>
        public bool italic;
        /// <summary>
        /// Determines if the font is in bold
        /// </summary>
        public bool bold;

        [NonSerialized] FontFamily fontFamily;

        //Properties
        /// <summary>
        /// Get or set the font family
        /// </summary>
        public FontFamily Font_Family
        {
            get
            {
                if (fontFamily == null)
                    fontFamily = new FontFamily(fontFamilyName);
                
                return fontFamily;
            }

            set
            {
                fontFamily = value;
                fontFamilyName = value.Source;
            }
        }

        /// <summary>
        /// Get or set the font style
        /// </summary>
        public FontStyle Font_Style
        {
            get=> italic? FontStyles.Italic : FontStyles.Normal;
            set=> italic = value != FontStyles.Normal;
        }

        /// <summary>
        /// Get or set the font weight
        /// </summary>
        public FontWeight Font_Weight
        {
            get => bold ? FontWeights.Bold : FontWeights.Normal;
            set => bold = value != FontWeights.Normal;
        }

        //Constructors
        /// <summary>
        /// Instanciate a new FontGroup
        /// </summary>
        public FontGroup()
        {
            fontFamilyName = "Segoe UI";
            fontSize = 20;
            italic = false;
            bold = false;
        }

        /// <summary>
        /// Instanciate a new FontGroup
        /// </summary>
        /// <param name="_fontFamily">Font name</param>
        /// <param name="_size">Font size</param>
        /// <param name="_style">Font style (italic, bold...)</param>
        /// <param name="_fontWeight"></param>
        public FontGroup(string _fontFamily, double _size, FontStyle _style, FontWeight _fontWeight)
        {
            fontFamilyName = _fontFamily;
            fontSize = _size;
            Font_Style = _style;
            Font_Weight = _fontWeight;
        }

        /// <summary>
        /// Create a FontGroup (WPF font) from a System.Drawing.Font object
        /// </summary>
        /// <param name="font">Font object</param>
        public FontGroup(System.Drawing.Font font)
        {
            fontFamilyName = font.FontFamily.Name;
            fontSize = font.Size * 98.0 / 72.0;
            Font_Style = font.Italic ? FontStyles.Italic : FontStyles.Normal;
            Font_Weight = font.Bold ? FontWeights.Bold : FontWeights.Normal;
        }

        //Apply this font to an element
        /// <summary>
        /// Apply this font to an UIElement object
        /// </summary>
        /// <param name="element">The UIElement</param>
        public void ApplyTo(UIElement element)
        {
            dynamic el = element;

            el.FontFamily = Font_Family;
            el.FontSize = fontSize;
            el.FontStyle = Font_Style;
            el.FontWeight = Font_Weight;
        }
    }
}
