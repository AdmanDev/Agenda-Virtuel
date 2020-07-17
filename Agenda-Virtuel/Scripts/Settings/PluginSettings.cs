using System;

namespace Agenda_Virtuel
{
    /// <summary>
    /// This class allows to get settings of a pugin
    /// </summary>
    [Serializable]
    internal class PluginSettings
    {
        //Variables
        /// <summary>
        /// If true, the plugin will be activated on application start
        /// </summary>
        internal bool enabled;

        //Constructor
        /// <summary>
        /// Instantiate a new PluginSettings object
        /// </summary>
        internal PluginSettings()
        {
            enabled = true;
        }

    }
}
