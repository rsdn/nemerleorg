using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NemerleOrg.Code;

namespace NemerleOrg.Models
{
  public class DownloadsViewModel
  {
    public IEnumerable<BuildArtifactFile> RecommendedBuilds { get; set; }
    public IEnumerable<BuildArtifactFile> NightlyBuilds { get; set; }
  }
}
