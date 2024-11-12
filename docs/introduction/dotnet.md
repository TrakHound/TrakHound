# .NET
Although the TrakHound Framework is not specific to a particular programming language, the first implementation is designed using .NET and written in c#.

Even though TrakHound is designed for IIoT applications, any web application can be designed using the TrakHound Framework as its backend as it adds functionality to what a standard .NET application would have. The primary benefits are the abstraction between applications and infrastructure and the ability to use Packages to hot load/unload modules.

## Modules
In the TrakHound Framework, Apps, APIs, and Services are designed as individual modules and distributed as packages.

Using the latest version of .NET (.NET 8), TrakHound Instances allow for modules to be hot loaded and removed during runtime. This allows for updates to packages without needing to restart the entire application.

## API
The TrakHound Entities API and custom APIs use a slightly modified version of the new Api Controllers in ASP.NET Core. There are several TrakHound specific Attributes to use that mimic those used in standard ASP.NET Core Api Controllers.

An example of a custom API endpoint is below:
```c#
[TrakHoundApiQuery("status")]
public async Task<TrakHoundApiResponse> GetControllerStatus([FromQuery] string devicePath)
{
    if (!string.IsNullOrEmpty(devicePath))
    {
        var model = await Client.Entities.Get<DeviceModel>(devicePath);
        if (model != null)
        {
            return Ok(model);
        }
        else
        {
            return NotFound("Program Not Found");
        }
    }
    else
    {
        return BadRequest();
    }
}
```

### Query Endpoints
Query endpoints are used to read and return data and can use either the GET or POST Http methods.

#### Attribute
```c#
[TrakHoundApiQuery]

[TrakHoundApiQuery("[ROUTE]")]
```

### Subscribe Endpoints
Subscribe endpoints are used to establish a WebSocket connection in order to stream data to a client. All subscribe URLs will end with a "/subscribe" suffix and can use either the GET or POST Http methods.

#### Attribute
```c#
[TrakHoundApiSubscribe]

[TrakHoundApiSubscribe("[ROUTE]")]
```

### Publish Endpoints
Publish endpoints are used to write data and can use either the PUT or POST Http methods. All publish URLs will end with a "/publish" suffix.

#### Attribute
```c#
[TrakHoundApiPublish]

[TrakHoundApiPublish("[ROUTE]")]
```

### Delete Endpoints
Delete endpoints are used to delete data and can use either the DELETE or POST Http methods. All delete URLs will end with a "/delete" suffix.

#### Attribute
```c#
[TrakHoundApiDelete]

[TrakHoundApiDelete("[ROUTE]")]
```

## App
TrakHound Apps use .NET Blazor with a slightly modified version of the standard Blazor Page Router. Using Blazor allows for the use of any Blazor library or component and allows for SPA pages just like a standard Blazor Server app allows for. A modified Page Router is used in order to allow for hot loading of App modules.

An example of an App page is below:
```c#
@page "/dashboard"
@inherits TrakHoundAppBase
@using TrakHound.Apps
@using TrakHound.Blazor

<div class="panel-container">
    <div class="panel">
        <DashboardPanel BoothNumber="1" />
    </div>
    <div class="panel">
        <DashboardPanel BoothNumber="2" />
    </div>
    <div class="panel">
        <DashboardPanel BoothNumber="3" />
    </div>
</div>
```

The primary difference between a TrakHound App and a standard .NET Blazor application is that "@page" routes are relative to the package and that Dependency Injection uses a single Service class instead of injecting multiple services directly.

Services intended to be injected are designed similar to below:
```c#
public class DashboardService : ITrakHoundSingletonAppInjectionService
```

Notice that the service class implements the "ITrakHoundSingletonAppInjectionService" interface. This is used to tell TrakHound that it should be loaded as a dependency injected service as well as defines the service lifetime (Transient, Scoped, or Singleton):

<ul>
    <li>ITrakHoundSingletonAppInjectionService</li>
    <li>ITrakHoundTransientAppInjectionService</li>
    <li>ITrakHoundScopedAppInjectionService</li>
</ul>

This is slightly different than a standard Blazor application but allows the module to determine the lifetime that the service needs and allows for hot load/remove when packages are installed or uninstalled.

In Pages that inherit "TrakHoundAppBase", injected services are accessed by the "GetService&lt;TService&gt;()" method as shown below:
```c#
var service = GetService<DashboardService>();
```

The service returned follows the service lifetime defined in the service's class declaration.
