using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Functions.Model
{
  public class AzureArtifactsPackageResult
  {
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("versions")]
    public List<PackageVersion> Versions { get; set; }

    public class PackageVersion
    {
      [JsonPropertyName("id")]
      public string Id { get; set; }

      [JsonPropertyName("isLatest")]
      public bool IsLatest { get; set; }

      [JsonPropertyName("packageDescription")]
      public string PackageDescription { get; set; }

      [JsonPropertyName("publishDate")]
      public string PublishDate { get; set; }

      [JsonPropertyName("version")]
      public string Version { get; set; }
    }
  }
}