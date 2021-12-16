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
Requestdropper tries to bring some peace of mind to the developer who wants to get rid of certain requests by returning a 429 or a 302 status code.

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
    "Limit": 60, // The amount of points required before a request is dropped.
    "Period": "00:01:30", // How long a client will be blocked.
    "ExcludedPaths": [ "/excluded-global" ], // Paths you wish to exclude from being handled by the request dropper middleware.
    "Rules": {
      "404": {              // How to handle requests that resulted in a 404 status code.
        "Weight": 20,       // Give them a 20 point penalty.
        "ExcludedPaths": [ // Except on the following paths.
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

### Specifications

#### Overwriting default behaviour.
By default requests are tried together using the IP Address
This can be changed by overridding the `Key` method on the RequestHandle

```C#
public class CustomHandler : NativeRequestHandler
{
    public CustomHandler(IStore<DropCounter?> store, IOptions<DropperSettings> dropperSettings)
        : base(store, dropperSettings)
    {
    }

    #region Overrides of RequestHandler<DropCounter?>

    public override string Key(HttpContext context)
    {
        return CustomRequestIdentity; // e.g. UserId...
    }

    #endregion
}
```
#### TODO

- [ ] TODO: Whitelist IP Addresses as a setting.
- [ ] TODO: Separate `Key` logic into it's own class.
- [ ] TODO: Hash default keys.
- [ ] TODO: X-Forwarded-For or X-Real-IP headers for default keys.
- [ ] TODO: Some sort of Mediator logic so block events can be listened to.
- [ ] TODO: Improved docs.

## Licensing

RequestDropper is licensed under the MIT license. It is free to use in personal and commercial projects.
