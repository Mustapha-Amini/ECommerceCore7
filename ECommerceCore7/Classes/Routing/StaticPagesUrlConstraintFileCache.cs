using ECommerceCore7.Models.Entities;

namespace ECommerceCore7.Classes.Routing
{
    public class StaticPagesUrlConstraintFileCache : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            string cacheFileName = Path.Combine(Directory.GetCurrentDirectory(), "staticPages.json"); ;
            if (!System.IO.File.Exists(cacheFileName))
            {
                return false;
            }

            var cacheFileContents = System.IO.File.ReadAllText(cacheFileName);

            var pages =
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<Page>>(cacheFileContents);

            if (values[parameterName] != null)
            {
                var permalink = values[parameterName].ToString();

                var page = pages.FirstOrDefault(p => p.PageRoute == permalink);

                if (page != null)
                {
                    // httpContext.Current.Items.Add("CurrentStaticPage", page);
                    httpContext.Items.Add("CurrentStaticPage", page);
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}