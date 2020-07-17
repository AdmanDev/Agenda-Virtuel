using System;
using System.Reflection;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// This attribute allows to create a MenuItem that invokes your method, in a <c>HomeworkViewer</c> menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Serializable]
    public class HomeworksMenuItemAttribute : Attribute
    {
        //Properties
        /// <summary>
        /// Get or set the title of your MenuItem
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Ignore this ! You don't set it !
        /// </summary>
        public MethodInfo method;

        //Constructors
        /// <summary>
        /// Instantiate HomeworksMenuItemAttribute object
        /// </summary>
        public HomeworksMenuItemAttribute()
        {
        }

        /// <summary>
        /// Instantiate HomeworksMenuItemAttribute object
        /// </summary>
        /// <param name="_title">Title of menu item</param>
        public HomeworksMenuItemAttribute(string _title)
        {
            Title = _title;
        }
    }
}
