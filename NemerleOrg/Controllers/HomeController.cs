using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NemerleOrg.Models;
using System.IO;
using NemerleOrg.Code;

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
      return View();
    }

    [HttpGet]
    public ActionResult Downloads()
    {
      var artifacts = GetArtifacts(_artifactsStoragePath)
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
      var artifacts = GetArtifacts(_artifactsStoragePath)
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
      var physicalPath = this.HttpContext.Server.MapPath(virtualPath);
      return Directory.GetFiles(physicalPath)
        .Select(filePath => new BuildArtifactFile(filePath, virtualPath))
        .Where(b => !string.IsNullOrEmpty(b.Title) && b.Version != null)
        .ToArray();
    }

    private const string _artifactsStoragePath = "~/Static/Downloads";
  }
}
