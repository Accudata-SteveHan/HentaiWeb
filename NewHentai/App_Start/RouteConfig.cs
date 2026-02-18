using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NewHentai
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region HomePage
          
  routes.MapRoute(
                name: "DEFAULT",
                url: "{key}",
                defaults:
                    new
                    {
                        controller = "Home",
                        action = "Index",
                        key = UrlParameter.Optional

                    }
            );

            #endregion HomePage

            #region ADMIN
            routes.MapRoute(
                name: "ADMIN",
                url: "ADMIN/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "ADMIN",
                        param = UrlParameter.Optional

                    }
            );

            #endregion ADMIN

            #region HOME
            routes.MapRoute(
                name: "HOME",
                url: "HOME/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "HOME",
                        param = UrlParameter.Optional

                    }
            );

            #endregion HOME

            #region CRAW
            routes.MapRoute(
                name: "CRAW",
                url: "CRAW/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "Craw",
                        param = UrlParameter.Optional

                    }
            );

            #endregion CRAW

            #region CRAWMANGA
            routes.MapRoute(
                name: "CRAWMANGA",
                url: "CRAWMANGA/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "CrawManga",
                        param = UrlParameter.Optional

                    }
            );
            #endregion CRAWMANGA

            #region CRAWDOUJINSHI
            routes.MapRoute(
                name: "CRAWDOUJINSHI",
                url: "CRAWDOUJINSHI/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "CrawDoujinshi",
                        param = UrlParameter.Optional

                    }
            );

            #endregion CRAWDOUJINSHI

            #region CRAWNONH
            routes.MapRoute(
                name: "CRAWNONH",
                url: "CRAWNONH/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "CrawNonH",
                        param = UrlParameter.Optional

                    }
            );

            #endregion CRAWNONH

            #region CRAWMELONBOOK
            routes.MapRoute(
                name: "CRAWMELONBOOK",
                url: "CRAWMELONBOOK/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "CrawMelonBook",
                        param = UrlParameter.Optional

                    }
            );

            #endregion CRAWMELONBOOK

            #region CRAWFEATURE
            routes.MapRoute(
                name: "CRAWFEATURE",
                url: "CRAWFEATURE/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "CrawFeature",
                        param = UrlParameter.Optional

                    }
            );

            #endregion CRAWMELONBOOK

            #region GALLERY
            routes.MapRoute(
                name: "GALLERY",
                url: "GALLERY/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "Gallery",
                        param = UrlParameter.Optional

                    }
            );

            #endregion GALLERY

            #region BROWSE
            routes.MapRoute(
                name: "BROWSE",
                url: "BROWSE/{action}/{type}/{lang}/{page}/{param}",
                defaults:
                    new
                    {
                        controller = "Browse",
                        page = "ALL",
                        param = ""

                    }
            );
            routes.MapRoute(
                name: "BROWSE_2",
                url: "MELON/{action}/{tag}/{type}/{lang}/{page}/{param}",
                defaults:
                    new
                    {
                        controller = "Browse",
                        page = "ALL",
                        param = ""

                    }
            );

            #endregion BROWSE

            #region SHOW
            routes.MapRoute(
                name: "BROWSE_SHOW",
                url: "SHOW/{action}/{key}/{type}/{lang}/{page}/{param}",
                defaults:
                    new
                    {
                        controller = "Show",
                        page = "ALL",
                        param = ""

                    }
            );

            routes.MapRoute(
                name: "BROWSE_SHOW_2",
                url: "MELONSHOW/{action}/{key}/{tag}/{type}/{lang}/{page}/{param}",
                defaults:
                    new
                    {
                        controller = "Show",
                        page = "ALL",
                        param = ""

                    }
            );
            #endregion SHOW

            #region TALK
            routes.MapRoute(
                name: "TALK",
                url: "TALK/{action}/{param}",
                defaults:
                    new
                    {
                        controller = "Telegram",
                        param = UrlParameter.Optional

                    }
            );

            #endregion TALK

        }

    }
}
