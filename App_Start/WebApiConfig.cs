using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace MortgageCalulator
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{action}",
            //    defaults: new { controller = "Quick", action = "GetBalance" },
            //    constraints: new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            //    );

        }
    }
}
