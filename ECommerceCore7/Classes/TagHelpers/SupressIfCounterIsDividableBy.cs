using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ECommerceCore7.Classes.TagHelpers
{
    [HtmlTargetElement("add-element-if", Attributes = ConditionName, TagStructure = TagStructure.WithoutEndTag)]
    public class SupressIfCounterIsDividableBy : TagHelper
    {
        private const string ConditionName = "condition";
        private const string ElementTagName = "element";
        //private const string ParentElementEndTagName = "parent-element-end";

        [HtmlAttributeName(ConditionName)]
        public bool Condition { get; set; }

        [HtmlAttributeName(ElementTagName)]
        public string ElementTag { get; set; }

        //[HtmlAttributeName(ParentElementEndTagName)]
        //public string ParentElementEndTag { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (Condition)
            {
                //output.PreElement.SetHtmlContent(ParentElementStartTag);
                //output.PostElement.SetHtmlContent(ParentElementEndTag);
                output.Content.SetContent(ElementTag);
            }
        }
    }
}