using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PropertyTycoon
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var json = config.Formatters.JsonFormatter;

            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "GetActivePlayerRoute",
                routeTemplate: "api/{controller}/{id}/GetActivePlayer",
                defaults: new { controller = "Game", action = "GetActivePlayer" }
            );

            config.Routes.MapHttpRoute(
                name: "GetBoardGameUsersRoute",
                routeTemplate: "api/{controller}/{id}/BoardUsers",
                defaults: new { controller = "Game", action = "GetBoardGameUsers" }
            );

            config.Routes.MapHttpRoute(
                name: "GetMovesListRoute",
                routeTemplate: "api/{controller}/{id}/GetMovesList",
                defaults: new { controller = "Game", action = "GetMovesList" }
            );

            config.Routes.MapHttpRoute(
                name: "CreateMovesRoute",
                routeTemplate: "api/{controller}/{id}/CreateMove/{userId}",
                defaults: new { controller = "Game", action = "GetMovesList" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
