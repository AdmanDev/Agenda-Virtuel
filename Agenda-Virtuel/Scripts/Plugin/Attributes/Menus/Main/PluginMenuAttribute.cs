using System;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// Allows to create menus for your plugin with public methods.
    /// Your class must be public and static.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Serializable]
    public class PluginMenuAttribute : Attribute
    {
        //Constructors
        /// <summary>
        /// Instantiate a PluginMenuAttribute object
        /// </summary>
        public PluginMenuAttribute()
        {
        }
    }
}
