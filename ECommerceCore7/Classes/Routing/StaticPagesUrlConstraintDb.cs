using ECommerceCore7.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ECommerceCore7.Classes.Routing
{
    public class StaticPagesUrlConstraintDb : IRouteConstraint
    {
        public Func<ApplicationDbContext> createDbContext { get; set; }

        public StaticPagesUrlConstraintDb(Func<ApplicationDbContext> createDbContext)
        {
            this.createDbContext = createDbContext;
        }
        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            //var db = new ApplicationDbContext(null).GetDbContext();
            if (values[parameterName] != null)
            {
                var permalink = values[parameterName].ToString();
                using (var db = createDbContext())
                {
                    var page = db.Pages
                    .Include(x => x.PageGroup)
                    .FirstOrDefault(p => p.PageRoute == permalink);
                    if (page != null)
                    {
                        // httpContext.Current.Items.Add("CurrentStaticPage", page);
                        httpContext.Items.Add("CurrentStaticPage", page);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
    }
}