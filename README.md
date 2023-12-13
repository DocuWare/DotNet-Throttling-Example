# DotNet-Throttling-Example
This repository contains a DotNet 8.0 based example project which use following NuGet packages:
- Our extended platform client [DocuWare.Platform.ServerClient.Extensions](https://www.nuget.org/packages/DocuWare.Platform.ServerClient.Extensions/)
- The resilience and transient-fault-handling library [Polly](https://www.nuget.org/packages/Polly/)
  
It shows a solution how to handle throttled platform requests with the DocuWare Exception DocuWare.Platform.ServerClient.Exceptions.ClientThrottleException and it's RetryAfterInterval.
