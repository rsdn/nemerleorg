using System;
using System.Collections.Generic;
using System.IO;

namespace NemerleOrg.Code
{
  public sealed class BuildResult
  {
    public BuildResult(BuildConfiguration configuration, string path)
    {
      Configuration = configuration;
      Path = path;
      BuildId = System.IO.Path.GetFileName(path);
      Artifacts = GetArtifacts();
    }

    public BuildConfiguration Configuration { get; private set; }

    public string BuildId { get; private set; }

    public string Path { get; private set; }

    public IEnumerable<BuildArtifactFile> Artifacts { get; private set; }

    private BuildArtifactFile[] GetArtifacts()
    {
      if (!Directory.Exists(Path))
        return new BuildArtifactFile[0];

      var paths = Directory.GetFiles(Path);
      return Array.ConvertAll(paths, path => new BuildArtifactFile(this, path));
    }
  }
}
