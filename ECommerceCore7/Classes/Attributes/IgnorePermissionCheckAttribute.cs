using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class IgnorePermissionCheckAttribute : Attribute
    {
    }
}