using System;
using System.Collections.Generic;
using System.IO;

namespace NemerleOrg.Code
{
  public sealed class BuildConfiguration
  {
    public BuildConfiguration(string path)
    {
      Name = System.IO.Path.GetFileName(path);
      Path = path;
      Results = GetResults();
    }

    public string Name { get; private set; }

    public string Path { get; private set; }

    public IEnumerable<BuildResult> Results { get; private set; }

    private BuildResult[] GetResults()
    {
      if (!Directory.Exists(Path))
        return new BuildResult[0];

      var paths = Directory.GetDirectories(Path);
      return Array.ConvertAll(paths, path => new BuildResult(this, path));
    }
  }
}
