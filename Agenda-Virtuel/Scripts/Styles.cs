using Agenda_Virtuel.Manager;
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to get or set the application style.
    /// </summary>
    [Serializable]
    public class Styles
    {
        //Variables
        private string winStyle, primButtonStyle, secButtonStyle;

        //Statics variables
        /// <summary>
        /// Determine if default style is initialized
        /// </summary>
        private static bool initialized = false;

        //Properties
        /// <summary>
        /// Get the default application style.
        /// </summary>
        public static Styles DefaultStyle { get; private set; }

        /// <summary>
        /// Get the default windows background.
        /// </summary>
        public static Brush DefaultWinBackgroundColor
        {
            get => (Brush)App.Instence.Resources["DefaultWinBackgroundColor"];
        }

        /// <summary>
        /// Get or set windows background.
        /// </summary>
        public Brush WinBackgroundColor
        {
            get => (Brush)App.Instence.Resources["WinBackgroundColor"];
            set => App.Instence.Resources["WinBackgroundColor"] = value;
        }

        /// <summary>
        /// Get or set the application windows style.
        /// </summary>
        /// <remarks>You have to use <see cref="ApplyStyle(bool)"/> method to apply this style.</remarks>
        public Style WindowsStyle
        {
            get
            {
                if (!string.IsNullOrEmpty(WinStyle))
                    return (Style)XamlReader.Parse(WinStyle);
                else
                    return
                        (Style)App.Instence.Resources["WindowsStyle"];
            }
            set
            {
                WinStyle = XamlWriter.Save(value);
            }
        }

        /// <summary>
        /// Get or set the primary buttons style.
        /// </summary>
        /// <remarks>You have to use <see cref="ApplyStyle(bool)"/> method to apply this style.</remarks>
        public Style PrimaryButtonStyle
        {
            get
            {
                if (!string.IsNullOrEmpty(PrimButtonStyle))
                    return (Style)XamlReader.Parse(PrimButtonStyle);
                else
                    return (Style)App.Instence.Resources["PrimaryButtonStyle"];
            }
            set
            {
                PrimButtonStyle = XamlWriter.Save(value);
            }
        }

        /// <summary>
        /// Get or set the secondary buttons style.
        /// </summary>
        /// <remarks>You have to use <see cref="ApplyStyle(bool)"/> method to apply this style.</remarks>
        public Style SecondaryButtonStyle
        {
            get
            {
                if (!string.IsNullOrEmpty(SecButtonStyle))
                    return (Style)XamlReader.Parse(SecButtonStyle);
                else
                    return (Style)App.Instence.Resources["SecondaryButtonStyle"];
            }
            set
            {
                SecButtonStyle = XamlWriter.Save(value);
            }
        }

        internal string WinStyle { get => winStyle; private set => winStyle = value; }
        internal string PrimButtonStyle { get => primButtonStyle; private set => primButtonStyle = value; }
        internal string SecButtonStyle { get => secButtonStyle; private set => secButtonStyle = value; }

        //Constructor
        /// <summary>
        /// Instanciate new style object.
        /// </summary>
        public Styles()
        {

        }

        /// <summary>
        /// Apply this style.
        /// </summary>
        /// <param name="save">True if you want to save this style.</param>
        public void ApplyStyle(bool save )
        {
            if (PrimaryButtonStyle != null)
                App.Instence.Resources["PrimaryButtonStyle"] = PrimaryButtonStyle;

            if (SecondaryButtonStyle != null)
                App.Instence.Resources["SecondaryButtonStyle"] = SecondaryButtonStyle;

            if (WindowsStyle != null)
            {
                App.Instence.Resources["WindowsStyle"] = WindowsStyle;
            }

            if (save)
            {
                EventsManager.Call_StylesChanged(Global.userData.settings.Styles);
                Save.SaveData();
            }
        }

        /// <summary>
        /// Determine if it is the default style.
        /// </summary>
        public bool IsDefault()
        {
            return DefaultStyle.Equals(this)
                || (string.IsNullOrEmpty(WinStyle) 
                    && string.IsNullOrEmpty(PrimButtonStyle) 
                    && string.IsNullOrEmpty(SecButtonStyle));
        }

        /// <summary>
        /// Initialize default style.
        /// </summary>
        internal static void Initialize()
        {
            if (initialized)
                return;
            initialized = true;

            DefaultStyle = new Styles();
            DefaultStyle.WinStyle = XamlWriter.Save(App.Instence.Resources["WindowsStyle"]);
            DefaultStyle.PrimButtonStyle = XamlWriter.Save(App.Instence.Resources["PrimaryButtonStyle"]);
            DefaultStyle.SecButtonStyle = XamlWriter.Save(App.Instence.Resources["SecondaryButtonStyle"]);

        }

        /// <summary>
        /// Get current styles.
        /// </summary>
        /// <returns><c>Styles</c> object.</returns>
        public static Styles GetSelectedStyles()
        {
            return new Styles()
            {
                PrimaryButtonStyle = (Style)App.Instence.Resources["PrimaryButtonStyle"],
                SecondaryButtonStyle = (Style)App.Instence.Resources["SecondaryButtonStyle"],
                WindowsStyle = (Style)App.Instence.Resources["WindowsStyle"]
            };
        }

        /// <summary>
        /// Determines if an object has the same values as this instance.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        /// <returns>Return true if objects have same values.</returns>
        public override bool Equals(object obj)
        {
            if(obj is Styles s)
            {
                return PrimButtonStyle == s.PrimButtonStyle && SecButtonStyle == s.SecButtonStyle && WinStyle == s.WinStyle;
            }

            return false;
        }

    }
}
