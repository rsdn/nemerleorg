using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using NemerleOrg.Controllers;
using System.IO;

namespace NemerleOrg
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      var artifactStorageDirectory = Server.MapPath(HomeController.ArtifactsStoragePath);
      if (!Directory.Exists(artifactStorageDirectory))
        Directory.CreateDirectory(artifactStorageDirectory);

      var bannerCacheDirectory = Server.MapPath(BannersController.BannerCacheDirectory);
      if (!Directory.Exists(bannerCacheDirectory))
        Directory.CreateDirectory(bannerCacheDirectory);


      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }
  }
}