using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;

namespace NemerleOrg.Code
{
  public sealed class BuildArtifactFile
  {
    public string Platform { get; private set; }
    public string Title { get; private set; }
    public Version Version { get; private set; }
    public string Extention { get; private set; }
    public string Name { get; private set; }
    public string FilePath { get; private set; }
    public string VirtualPath { get; private set; }

    public BuildArtifactFile(string filePath, string baseVirtualPath)
    {
      FilePath = filePath;
      Name = Path.GetFileName(filePath);
      VirtualPath = Path.Combine(baseVirtualPath, Name);
      Extention = Path.GetExtension(filePath);
      var nameWithoutExtention = Path.GetFileNameWithoutExtension(filePath);
      var matchResult = _pattern.Match(nameWithoutExtention);
      if (matchResult.Success)
      {
        Title = matchResult.Groups["Title"].Value;
        Platform = matchResult.Groups["Platform"].Value;
        Version version;
        if (System.Version.TryParse(matchResult.Groups["Version"].Value, out version))
          Version = version;
      }
    }

    private static readonly Regex _pattern = new Regex(@"(?<Title>.*?)\-(?<Platform>net-.*?)\-v(?<Version>\d+(\.\d+){3})", RegexOptions.Compiled);
  }
}
