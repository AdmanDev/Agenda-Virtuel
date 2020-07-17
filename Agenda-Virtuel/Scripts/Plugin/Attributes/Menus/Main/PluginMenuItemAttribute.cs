using System;
using System.Reflection;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// This attribute allows to create a MenuItem that invokes your method, in main window.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Serializable]
    public class PluginMenuItemAttribute : Attribute
    {
        //Properties
        /// <summary>
        /// The title of the MenuItem
        /// </summary>
        public string ItemTitle { get; set; }
        /// <summary>
        /// The description of your MenuItem
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The parent (MenuItem) of this MenuItem 
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Ignore this ! You don't set it !
        /// </summary>
        public MethodInfo method;

        //Constructors
        /// <summary>
        /// Instantiate a PluginMenuItemAttribute object with default title
        /// </summary>
        public PluginMenuItemAttribute()
        {
            ItemTitle = "Your title";
        }

        /// <summary>
        /// Instantiate a PluginMenuItemAttribute object with title, description and group namme
        /// </summary>
        /// <param name="_itemTitle">The title of item menu</param>
        /// <param name="_description">The description of item menu</param>
        /// <param name="_groupName">The group of this item menu. Each groups will be separate by a separator</param>
        public PluginMenuItemAttribute(string _itemTitle, string _description, string _groupName)
        {
            ItemTitle = _itemTitle;
            Description = _description;
            GroupName = _groupName;
        }

    }
}
