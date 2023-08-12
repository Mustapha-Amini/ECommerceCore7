using System;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerTitleAttribute : Attribute
    {
        public ControllerTitleAttribute(string PluralTitle, string SingularTitle)
        {
            this.PluralTitle = PluralTitle;
            this.SingularTitle = SingularTitle;
        }
        public string SingularTitle { get; set; }
        public string PluralTitle { get; set; }
    }
}