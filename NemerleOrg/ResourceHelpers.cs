using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace NemerleOrg
{
  public static class ResourceHelpers
  {
    public static T SelectResource<T>(this ViewContext context, string desiredController, string desiredAction, T trueResult, T falseResult)
    {
      if ((string)context.RouteData.Values["controller"] == desiredController
        && (string)context.RouteData.Values["action"] == desiredAction)
        return trueResult;
      else
        return falseResult;
    }
  }
}
