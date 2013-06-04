using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NemerleOrg.Code;

namespace NemerleOrg.Models
{
  public class BuildHistoryViewModel
  {
    public int Major { get; set; }
    public int Minor { get; set; }
    public BuildArtifactFile[] Artifacts { get; set; }
  }
}