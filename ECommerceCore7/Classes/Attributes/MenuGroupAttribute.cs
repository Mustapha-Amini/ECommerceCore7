using System;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MenuGroupAttribute : Attribute
    {
        public MenuGroupAttribute(string MenuGroupTitle)
        {
            this.MenuGroupTitle = MenuGroupTitle;
        }
        public string MenuGroupTitle { get; set; }
    }
}