using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Microsoft.AspNetCore.Mvc
{
    public class IconAndTitleActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var methodInfo = controllerActionDescriptor.MethodInfo;

            var controllerAttributes = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(true);
            var methodAttributes = methodInfo.GetCustomAttributes(true);

            var actionIconAttribute = (IconAttribute)methodAttributes.FirstOrDefault(ma => ma is IconAttribute);
            var actionTitleAttribute = (TitleAttribute)methodAttributes.FirstOrDefault(ma => ma is TitleAttribute);
            var controllerTitleAttribute = (ControllerTitleAttribute)controllerAttributes.FirstOrDefault(ma => ma is ControllerTitleAttribute);

            if (actionIconAttribute != null)
            {
                controller.ViewBag.Icon = actionIconAttribute.DisplayIcon;
            }

            if (actionTitleAttribute != null)
            {
                controller.ViewBag.Title = actionTitleAttribute.DisplayTitle;
            }

            if (controllerTitleAttribute != null)
            {
                controller.ViewBag.ControllerPluralTitle = controllerTitleAttribute.PluralTitle;
                controller.ViewBag.ControllerSingularTitle = controllerTitleAttribute.SingularTitle;
            }
        }
    }
}
