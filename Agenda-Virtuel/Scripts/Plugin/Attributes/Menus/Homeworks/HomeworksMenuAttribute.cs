using System;

namespace Agenda_Virtuel.Plugin
{
    /// <summary>
    /// This attribute allows to create menus for homeworks with public static methods.
    /// Your class must be public and static.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Serializable]
    public class HomeworksMenuAttribute : Attribute
    {
        /// <summary>
        /// Instantiate a HomeworksMenuAttribute object
        /// </summary>
        public HomeworksMenuAttribute()
        {

        }
    }
}
