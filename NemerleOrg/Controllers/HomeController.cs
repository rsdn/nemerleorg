using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using NemerleOrg.Code;
using NemerleOrg.Models;
using System.Net.Mime;

namespace NemerleOrg.Controllers
{
  public class HomeController : Controller
  {
    [HttpGet]
    public ActionResult Index()
    {
      return RedirectToAction("About");
    }

    [HttpGet]
    public ActionResult About()
    {
      var artifacts = GetArtifacts();
      BuildArtifactFile defaultDownload = null;
      foreach (var b in artifacts)
      {
        if (b.Platform == "net-4.0" && b.Title.Contains("Setup"))
        {
          if (defaultDownload == null || defaultDownload.Version < b.Version)
            defaultDownload = b;
        }
      }
      return View(new AboutViewModel
      {
        DefaultDownload = defaultDownload
      });
    }

    [HttpGet]
    public ActionResult Downloads()
    {
      var artifacts = GetArtifacts()
        .GroupBy(b => Tuple.Create(b.Version.Major, b.Version.Minor))
        .SelectMany(g =>
          {
            var top = new List<BuildArtifactFile>();
            foreach (var b in g)
            {
              if (top.Count == 0 || top[0].Version == b.Version)
                top.Add(b);
              else if (top[0].Version < b.Version)
              {
                top.Clear();
                top.Add(b);
              }
            }
            return top;
          })
        .ToArray();
      return View(new DownloadsViewModel
      {
        Artifacts = artifacts,
      });
    }

    [HttpGet]
    public ActionResult Download(string buildConfiguration, string buildId, string name)
    {
      if (MvcApplication.TeamCityBuildConfigurations.Any(_ => _ == buildConfiguration))
      {
        var bc = GetBuildConfiguration(buildConfiguration);
        var r = bc.Results.FirstOrDefault(_ => _.BuildId == buildId);
        if (r != null)
        {
          var a = r.Artifacts.FirstOrDefault(_ => _.Name == name);
          return File(a.FilePath, MediaTypeNames.Application.Octet);
        }
      }
      return HttpNotFound();
    }

    [HttpGet]
    public ActionResult BuildHistory(int major, int minor)
    {
      var artifacts = GetArtifacts()
        .Where(b => b.Version.Major == major && b.Version.Minor == minor)
        .ToArray();
      if (artifacts.Length > 0)
        return View(new BuildHistoryViewModel
        {
          Major = major,
          Minor = minor,
          Artifacts = artifacts
        });
      return RedirectToAction("Downloads");
    }

    private BuildArtifactFile[] GetArtifacts()
    {
      var cacheKey = "HomeController.GetArtifacts";
      var cachedResult = HttpContext.Cache.Get(cacheKey);
      if (cachedResult != null)
        return (BuildArtifactFile[])cachedResult;

      var artifacts = new List<BuildArtifactFile>();
      foreach (var name in MvcApplication.TeamCityBuildConfigurations)
      {
        var buildConfiguration = GetBuildConfiguration(name);
        foreach (var buildResult in buildConfiguration.Results)
          foreach (var artifact in buildResult.Artifacts)
            if (!string.IsNullOrEmpty(artifact.Title) && artifact.Version != null)
              artifacts.Add(artifact);
      }

      var result = artifacts.ToArray();
      HttpContext.Cache.Insert(cacheKey, result, new CacheDependency(MvcApplication.TeamCityProjectPath));
      return result;
    }

    private BuildConfiguration GetBuildConfiguration(string name)
    {
      var cacheKey = "HomeController.GetBuildConfiguration " + name;
      var cachedResult = HttpContext.Cache.Get(cacheKey);
      if (cachedResult != null)
        return (BuildConfiguration)cachedResult;

      var result = new BuildConfiguration(Path.Combine(MvcApplication.TeamCityProjectPath, name));
      HttpContext.Cache.Insert(cacheKey, result, new CacheDependency(result.Path));
      return result;
    }

    public const string ArtifactsStoragePath = "~/Static/Downloads";
  }
}
