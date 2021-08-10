# DotNet-Throttling-Example
This repository contains a DotNet Core 3.1 based example project which use following NuGet packages:
- Our extended platform client [DocuWare.Platform.ServerClient.Extensions](https://www.nuget.org/packages/DocuWare.Platform.ServerClient.Extensions/)
- The resilience and transient-fault-handling library [Polly](https://www.nuget.org/packages/Polly/)
- [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/)
- [Microsoft.Extensions.Configuration.Binder](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/)
- [Microsoft.Extensions.Configuration.CommandLine](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.CommandLine/)
- [Microsoft.Extensions.Configuration.EnvironmentVariables](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.EnvironmentVariables/)
- [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/)
  
It shows a solution how to handle throttled platform requests.
## How to use
In the configuration file appsettings.json it is possible to include the settings shown in the sample below.
With these settings the behavior of the policy is defined which is handling the case of a custom exception from type HttpClientRequestException with the http status code 429 to many requests.
You are able to define for each platform endpoint and http method a time to wait in seconds that will be converted into a TimeSpan.
``` json
{
    "DocuWarePlatform": {
    "ThrottlingDefaultRetryAfterSeconds": 300, 
    "Throttling": [
      {
        "Endpoint": "/DocuWare/Platform/Account/Logon",
        "Method": "POST",
        "SecondsToWait": 60
      },
      {
        "Endpoint": "/DocuWare/Platform/FileCabinets/Index",
        "Method": "GET",
        "SecondsToWait": 60 
      }
    ]
  } 
}
```