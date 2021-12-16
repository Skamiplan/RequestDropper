<div align="center">

![Icon](images/icon.png)
# RequestDropper
A request-dropping extension for ASP.NET Core
    
![Build](https://img.shields.io/github/workflow/status/Skamiplan/RequestDropper/Build)
[![NuGet](https://img.shields.io/nuget/v/RequestDropper.svg)](https://www.nuget.org/packages/RequestDropper/)
</div>

# RequestDropper for ASP.NET Core
This repository contains the C# source code for RequestDropper. You should [get it on NuGet](https://www.nuget.org/packages/RequestDropper/):

    PM> Install-Package RequestDropper

## Overview
Requestdropper tries to bring some peace of mind to the developer who wants to get rid of certain requests.

Some of the major features include:
- High performance
- Tiny footprint
- Easy to configure
- Extensive customization
- Flexible extensibility

## Basic usage

```C#
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    //...

    // Add the request dropper logic and chain a store.
    // Default configuration section.
    services.AddRequestDropper(hostBuilder.Configuration).AddMemoryCacheStore();

    //...
  }
  
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    //...
    
    // Add the requestdropper middleware 
    app.UseRequestDropperMiddleware(); 
    
    //...
  }
}
```

## Configuration

Where your `appsettings.json` (or other configuration source) contains a configuration like this:
```json
{
  "DropperSettings": {
    "Limit": 60,
    "Period": "00:01:30",
    "ExcludedPaths": [ "/excluded-global" ],
    "Rules": {
      "404": {
        "Weight": 20,
        "ExcludedPaths": [
          "/excluded-404"
        ]
      },
      "500": {
        "Weight": 5,
        "ExcludedPaths": [
          "/excluded-error"
        ]
      },
      "401": {
        "Weight": 15,
        "ExcludedPaths": [
          "/useless-login"
        ]
      }
    }
  }
}
```

## Licensing

RequestDropper is licensed under the MIT license. It is free to use in personal and commercial projects.
