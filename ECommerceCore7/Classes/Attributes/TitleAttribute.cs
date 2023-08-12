using System;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TitleAttribute : Attribute
    {
        public TitleAttribute(string Title)
        {
            this.DisplayTitle = Title;
        }
        public string DisplayTitle { get; set; }
    }
}