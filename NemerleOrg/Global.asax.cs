using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using NemerleOrg.Controllers;
using System.IO;
using System.Configuration;

namespace NemerleOrg
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      var bannerCacheDirectory = Server.MapPath(BannersController.BannerCacheDirectory);
      if (!Directory.Exists(bannerCacheDirectory))
        Directory.CreateDirectory(bannerCacheDirectory);

      TeamCityProjectPath = GetApplicationSetting("TeamCityProjectPath");
      TeamCityBuildConfigurations = GetApplicationSetting("TeamCityBuildConfigurations").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
    }

    private string GetApplicationSetting(string key)
    {
      var value = ConfigurationManager.AppSettings[key];
      if (string.IsNullOrEmpty(value))
        throw new InvalidOperationException(string.Format("Application setting '{0}' is required", key));
      return value;
    }

    public static string TeamCityProjectPath { get; private set; }
    public static string[] TeamCityBuildConfigurations { get; private set; }
  }
}