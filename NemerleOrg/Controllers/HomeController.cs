using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
      return View();
    }
  }
}
