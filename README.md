![trakhound-logo](https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/trakhound-logo-v5-100px.png)

# TrakHound
Open Source Application Framework for Industrial Enterprise & IoT Applications. Provides Data Interoperability, Flexible Architecture (Cloud, On-Premise, or Hybrid), Package Based Deployments. Built on .NET 8 and Blazor. Use cases include custom MES, ERP, CMMS, QMS applications as well as many other IIoT related solutions. The primary purpose of the framework is to allow multiple systems to work together in a seamless way.

## Demo
View a Live Demo using the link below:
https://www.trakhound.com/demo/_admin

## Download
<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">Name</th>
            <th style="text-align: center;width: 20px;"></th>
            <th style="text-align: left;min-width: 100px;">Type</th>
            <th style="text-align: left;">Link</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Instance</td>
            <td><img src="https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/windows-logo.svg" style="height:20px;width:20px;vertical-align: middle;" /></td>
            <td>Installer</td>
            <td><a href="https://github.com/TrakHound/TrakHound/releases/latest">https://github.com/TrakHound/TrakHound/releases/latest</a></td>
        </tr>        
        <tr>
            <td>Instance</td>
            <td><img src="https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/docker-logo.svg" style="height:20px;width:20px;vertical-align: middle;" /></td>
            <td>Docker</td>
            <td><a href="https://hub.docker.com/repository/docker/trakhound/instance">https://hub.docker.com/repository/docker/trakhound/instance</a></td>
        </tr>
        <tr>
            <td>SDK</td>
            <td><img src="https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/nuget-logo.svg" style="height:20px;width:20px;vertical-align: middle;" /></td>
            <td>Nuget</td>
            <td><a href="https://www.nuget.org/profiles/TrakHound">https://www.nuget.org/profiles/TrakHound</a></td>
        </tr>
        <tr>
            <td>CLI</td>
            <td><img src="https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/windows-logo.svg" style="height:20px;width:20px;vertical-align: middle;" /></td>
            <td>Installer</td>
            <td><a href="https://github.com/TrakHound/TrakHound/releases/latest">https://github.com/TrakHound/TrakHound/releases/latest</a></td>
        </tr>  
    </tbody>
</table>

## Release Notes
11/12/2024 : This is the initial beta release of TrakHound and is intended to provide an example of the functionality of the framework. Note that there may be some areas that are not finished yet. if you see anything that is not completed, please feel free to create an Issue or Discussion.

We are looking for feedback and recommendations so please feel free to create a Discussion on GitHub or reach out to us directly at info@trakhound.com. 

We love talking about software and manufacturing so don't hesitate to reach out!

> [!NOTE]
> The TrakHound Framework and some of the base modules are Free are completely Open Source while some modules, such as Drivers, are not open source and may require a license. Most licenses are available with a 2 Hour renewable demo and full licenses can be purchased directly through the Package Manager UI.

## Documentation
Learn more using the link below:
https://github.com/TrakHound/TrakHound/tree/dev/docs

> Note that Documentation is still a work in progress. This will be a focus during the initial beta.

## Getting Started

### Windows
- Download and Install TrakHound Instance
- Browse to http://localhost:8472/_admin
- (If First Open) Create an Admin username and password and click **Setup**
- Install the **TrakHound.Core.Bundle** package using the Admin UI

### Docker
```
docker pull trakhound/instance
docker container run -p 8472:8080 trakhound/instance
```
After Docker container is running, browse to http://localhost:8472/_admin/packages and install the **TrakHound.Core.Bundle** package.

## Overview
Simplify application development and utilize modern software tools and concepts to develop industrial resource tracking systems such as MES, ERP, etc. Although TrakHound is designed with industrial applications in mind, as they require many very small custom applications, any application can be developed with the framework.

- Develop applications using Visual Studio or Visual Studio Code
- Build on the .NET framework using Nuget for external libraries
- Compatible with Git for version control and project management
- Package modules for distribution

<table>
    <tbody>
        <tr>
            <td style="vertical-align: top;">Apps</td>
            <td>Build and Deploy user interfaces that can be accessed through a Web Browser using the latest web technologies</td>
        </tr>        
        <tr>
            <td style="vertical-align: top;">API</td>
            <td>Custom interfaces for application specific endpoints</td>
        </tr>
        <tr>
            <td style="vertical-align: top;">Functions</td>
            <td>Short Lived processes for performing tasks</td>
        </tr>
        <tr>
            <td style="vertical-align: top;">Services</td>
            <td>Long lived processes for maintaining external connections</td>
        </tr>  
        <tr>
            <td style="vertical-align: top;">Entities</td>
            <td>Data from multiple sources often have their own schema and format that is specific for its use case. TrakHound decomposes complex models into simple Entities so that they can all interact together regardless of the overall schema.</td>
        </tr> 
    </tbody>
</table>

## Admin Interface
The Admin UI is used to install/manage TrakHound Packages, to explore App, Api, Function, and Service modules, and to view/manage Entities. 

> Note that this UI can be protected through Authentication or can be disabled entirely.

### Explorer (Entities)
![Explorer-Entities-Screenshot](https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/explorer-entities.png)

### Packages
![Packages-Screenshot](https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/packages.png)

### Configuration
![Configuration-Screenshot](https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/configuration.png)

## Architecture
Configurable architecture that creates an abstraction layer between applications and infrastructure. Routing can be configured as needed whether that is to implement more of a monolith or more of a distributed architecture.

![Routing-Screenshot](https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/routing.png)

## Support
TrakHound provides commercial **Enterprise Support Plans** to maintain uptime and provide professional consulting and setup assistance. Please feel free to contact us for more information at info@trakhound.com.

## Sponsor
If you have found TrakHound helpful or are interested in helping, please consider sponsoring this project. We welcome your support whether you are a manufacturer, an integrator, an educational institute, or an individual.

[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&style=for-the-badge&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/TrakHound)

https://github.com/sponsors/TrakHound

## Contribution / Feedback
- Please use the [Issues](https://github.com/TrakHound/TrakHound/issues) tab to create issues for specific problems that you may encounter 
- Please feel free to use the [Pull Requests](https://github.com/TrakHound/TrakHound/pulls) tab for any suggested improvements to the source code
- For any other questions or feedback, please contact TrakHound directly at **info@trakhound.com**.

## License
This application and it's source code is licensed under the [MIT License](https://choosealicense.com/licenses/mit/) and is free to use and distribute.
