using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Functions.Model
{
  public class PackageResult
  {
    [JsonPropertyName("objects")]
    public List<Object> Objects { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("time")]
    public string Time { get; set; }
    
    public class Links
    {
      [JsonPropertyName("npm")]
      public string Npm { get; set; }

      [JsonPropertyName("homepage")]
      public string Homepage { get; set; }

      [JsonPropertyName("repository")]
      public string Repository { get; set; }

      [JsonPropertyName("bugs")]
      public string Bugs { get; set; }
    }

    public class Publisher
    {
      [JsonPropertyName("username")]
      public string Username { get; set; }

      [JsonPropertyName("email")]
      public string Email { get; set; }
    }

    public class Maintainer
    {
      [JsonPropertyName("username")]
      public string Username { get; set; }

      [JsonPropertyName("email")]
      public string Email { get; set; }
    }

    public class Package
    {
      [JsonPropertyName("name")]
      public string Name { get; set; }

      [JsonPropertyName("version")]
      public string Version { get; set; }

      [JsonPropertyName("description")]
      public string Description { get; set; }

      [JsonPropertyName("keywords")]
      public List<string> Keywords { get; set; }

      [JsonPropertyName("date")]
      public DateTime Date { get; set; }

      [JsonPropertyName("links")]
      public Links Links { get; set; }

      [JsonPropertyName("publisher")]
      public Publisher Publisher { get; set; }

      [JsonPropertyName("maintainers")]
      public List<Maintainer> Maintainers { get; set; }
    }

    public class Detail
    {
      [JsonPropertyName("quality")]
      public double Quality { get; set; }

      [JsonPropertyName("popularity")]
      public double Popularity { get; set; }

      [JsonPropertyName("maintenance")]
      public double Maintenance { get; set; }
    }

    public class Score
    {
      [JsonPropertyName("final")]
      public double Final { get; set; }

      [JsonPropertyName("detail")]
      public Detail Detail { get; set; }
    }

    public class Object
    {
      [JsonPropertyName("package")]
      public Package Package { get; set; }

      [JsonPropertyName("score")]
      public Score Score { get; set; }

      [JsonPropertyName("searchScore")]
      public double SearchScore { get; set; }
    }
  }
}