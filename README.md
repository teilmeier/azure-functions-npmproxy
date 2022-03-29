# Azure Artifacts npm Proxy

This Azure Function extends Azure Artifacts' npm API by the [*search*](https://github.com/npm/registry/blob/master/docs/REGISTRY-API.md#get-v1search) and [*all*](https://github.com/npm/registry/blob/master/docs/REGISTRY-API.md#get-all) endpoints.
It acts as a proxy that handles all search/all requests internally in a function and routes all other requests to Azure Artifacts using Azure Functions Proxies.

## Use cases

- Search packages using [npm-search](https://docs.npmjs.com/cli/v8/commands/npm-search)
- Use Azure Artifacts in [Unity Package Manager](https://docs.unity3d.com/Manual/Packages.html)

## Deployment and configuration

- Deploy to a Function App. I tested with an Azure Function in the `Consumption` tier.
- Use Azure Function Runtime 3. Functions Runtime V4 is not supported.
- All required configuration details are passed to the Azure Function in the function call (HTTP request)
- No appSettings need to be changed

Follow the [documented](https://docs.microsoft.com/azure/devops/artifacts/npm/npmrc?view=azure-devops&tabs=linux%2Cyaml) steps to connect to your Azure Artifacts npm feed. Replace all occurrences of `pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm` with `{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}` in the `.npmrc` or other client's configuration files. That's it.

### Before

`.npmrc` in your project directory:

    registry=https://pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/registry/

    always-auth=true

`.npmrc` in your home directory:

    ; begin auth token
    //pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/registry/:username=somevalue
    //pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/registry/:_password=[BASE64_ENCODED_PAT]
    //pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/registry/:email=someemail
    //pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/:username=somevalue
    //pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/:_password=[BASE64_ENCODED_PAT]
    //pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/:email=someemail
    ; end auth token

### After

`.npmrc` in your project directory:

    registry=https://{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/registry/

    always-auth=true

`.npmrc` in your home directory:

    ; begin auth token
    //{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/registry/:username=somevalue
    //{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/registry/:_password=[BASE64_ENCODED_PAT]
    //{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/registry/:email=some@e.mail
    //{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/:username=somevalue
    //{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/:_password=[BASE64_ENCODED_PAT]
    //{functionName}.azurewebsites.net/npm/{organization}/{project}/{feedId}/:email=some@e.mail
    ; end auth token

## Limits

- npm-cli has a maximum package size of ~1 GB when publishing packages
- Azure Artifacts has a [maximum package size](https://docs.microsoft.com/azure/devops/artifacts/reference/limits?view=azure-devops#size-limits) of ~ 500 MB
- Azure Functions have a [maximum **request** size](https://docs.microsoft.com/azure/azure-functions/functions-scale#service-limits) of 100 MB

## Notes and ideas

Package downloads are automatically served by Azure Artifacts' underlying blob storage. So typically only metadata and package uploads are passing the Azure Function. This important for cost and performance reasons. To change the behavior of the Function Proxy and further decrease the amount of data passing the Function you could also implement a redirect instead of defining a backendUri. The downside is that this behavior has to be supported by the client. For example `npm publish` does not work with the following proxy definition:

```javascript
"azureDevOpsRedirect": {
  "desc": [ "307 Redirect to Azure Artifacts instead of passing traffic through Function App" ],
  "matchCondition": {
    "methods": [
      "GET",
      "POST",
      "DELETE",
      "HEAD",
      "PATCH",
      "PUT",
      "OPTIONS",
      "TRACE"
    ],
    "route": "/npm/{organization}/{project}/{feedId}/{*restOfPath}"
  },
  "responseOverrides": {
    "response.headers.Location": "<https://pkgs.dev.azure.com/{organization}/{project}/_packaging/{feedId}/npm/{restOfPath>}",
    "response.statusCode": "307"
  }
}


