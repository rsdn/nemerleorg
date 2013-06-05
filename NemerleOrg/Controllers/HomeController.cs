using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using NemerleOrg.Code;
using NemerleOrg.Models;

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
      var artifacts = GetArtifacts(ArtifactsStoragePath);
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
      var artifacts = GetArtifacts(ArtifactsStoragePath)
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
    public ActionResult BuildHistory(int major, int minor)
    {
      var artifacts = GetArtifacts(ArtifactsStoragePath)
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

    private BuildArtifactFile[] GetArtifacts(string virtualPath)
    {
      var cacheKey = "HomeController.GetArtifacts " + virtualPath;
      var cachedResult = HttpContext.Cache.Get(cacheKey);
      if (cachedResult != null)
        return (BuildArtifactFile[])cachedResult;

      var physicalPath = this.HttpContext.Server.MapPath(virtualPath);
      var result = Directory.GetFiles(physicalPath)
        .Select(filePath => new BuildArtifactFile(filePath, virtualPath))
        .Where(b => !string.IsNullOrEmpty(b.Title) && b.Version != null)
        .ToArray();

      HttpContext.Cache.Insert(cacheKey, result, new CacheDependency(physicalPath));

      return result;
    }

    public const string ArtifactsStoragePath = "~/Static/Downloads";
  }
}
