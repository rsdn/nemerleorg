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
    public ActionResult Index()
    {
      return RedirectToAction("About");
    }

    public ActionResult About()
    {
      return View();
    }

    public ActionResult Downloads()
    {
      return View(new DownloadsViewModel
      {
        RecommendedBuilds = GetArtifacts("~/Static/Downloads/Recommended"),
        NightlyBuilds = GetArtifacts("~/Static/Downloads/Nightly"),
      });
    }

    private BuildArtifactFile[] GetArtifacts(string path)
    {
      var physicalPath = this.HttpContext.Server.MapPath(path);
      return Directory.GetFiles(physicalPath)
        .Select(filePath => new BuildArtifactFile(filePath))
        .Where(b => ! string.IsNullOrEmpty(b.Title))
        .ToArray();
    }
  }
}
