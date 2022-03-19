using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Functions.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions
{
  public static class Api
  {
    const string AUTH_HEADER = "Authorization";
    public static HttpClient httpClient = new HttpClient();

    [FunctionName("Search")]
    public static async Task<IActionResult> Search(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{organization}/{project}/{feedId}/search")] HttpRequest req,
        string organization,
        string project,
        string feedId,
        ILogger log)
    {
      string debugAuth = null;

      // Authorization header is expected in the following format: 'Basic base64Encode(<user>:<PAT>)'
      // The header is automatically set by the npm command line if the .npmrc file(s) are configured
      // - <user> can be any string
      // - <PAT> required permissions: Packaging Read & write
      if(!req.Headers.ContainsKey(AUTH_HEADER) && string.IsNullOrEmpty(debugAuth))
      {
        return new UnauthorizedResult();
      }

      string packageNameQuery = req.Query["text"];
      string top = req.Query["size"];
      string skip = req.Query["from"];

      // Documented at: https://docs.microsoft.com/en-us/rest/api/azure/devops/artifacts/artifact-details/get-packages?view=azure-devops-rest-7.1
      var packageSearchUrl = $"https://feeds.dev.azure.com/{organization}/{project}/_apis/packaging/Feeds/{feedId}/packages?protocolType=npm&packageNameQuery={packageNameQuery ?? string.Empty}&includeUrls=true&includeAllVersions=false&getTopPackageVersions=false&includeDescription=true&$top={top ?? "20"}&$skip={skip ?? "0"}&api-version=7.1-preview.1";
      
      var packageRequest = new HttpRequestMessage(HttpMethod.Get, packageSearchUrl);

      string authHeader = req.Headers[AUTH_HEADER];
      authHeader = authHeader ?? debugAuth;

      packageRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader.Replace("Basic ", string.Empty, true, CultureInfo.InvariantCulture));
      
      var response = await httpClient.SendAsync(packageRequest);
      var rawResult = await response.Content.ReadAsAsync<dynamic>();

      AzureArtifactsPackageResult[] result = JsonConvert.DeserializeObject<AzureArtifactsPackageResult[]>(rawResult.value.ToString());

      var npmSearchResult = new PackageResult
      {
        Objects = new List<PackageResult.Object>(result.Length),
        Total = result.Length,
        Time = DateTime.UtcNow.ToString("r")
      };

      foreach (var package in result)
      {
        npmSearchResult.Objects.AddRange(
            package.Versions.Where(v => v.IsLatest).Select(v => new PackageResult.Object 
            {
              Package = new PackageResult.Package
              {
                Name = package.Name,
                Date = DateTime.Parse(v.PublishDate),
                Description = v.PackageDescription,
                Version = v.Version
              }
            }
          )
        );
      }

      return new OkObjectResult(npmSearchResult);
    }
  }
}
