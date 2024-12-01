# TrakHound.Api.Template
Create a .NET Project template for a TrakHound API module. Api modules are used to create a custom programming interface in TrakHound.

<table>
    <thead>
        <tr>
            <td style="font-weight: bold;">Type</td>
            <td style="font-weight: bold;">Name</td>
            <td style="font-weight: bold;">Downloads</td>
            <td style="font-weight: bold;">Link</td>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Nuget</td>
            <td>TrakHound.Api.Template</td>
            <td><img src="https://img.shields.io/nuget/dt/TrakHound.Api.Template?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/TrakHound.Api.Template">https://www.nuget.org/packages/TrakHound.Api.Template</a></td>
        </tr>
    </tbody>
</table>



## Get Started

#### Install Template
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to install the TrakHound.Api.Template using the below command:
```
dotnet new install TrakHound.Api.Template
```

#### Create New Project
This will create a new project using the template in the current working directory
```
dotnet new trakhound.api
```

##### Parameters
<table>
    <thead>
        <tr>
            <th style="text-align: left;min-width: 100px;">Name</th>
            <th style="text-align: center;width: 20px;">Default Value</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>trakhoundVersion</td>
            <td>*</td>
            <td>Used to specify the version of the TrakHound Nuget packages to use</td>
        </tr>   
        <tr>
            <td>publisher</td>
            <td></td>
            <td>Sets the Publisher metadata in the <b>trakhound.package.json</b> file</td>
        </tr>   
        <tr>
            <td>description</td>
            <td></td>
            <td>Sets the Description metadata in the <b>trakhound.package.json</b> file</td>
        </tr>   
        <tr>
            <td>defaultRoute</td>
            <td>[PROJECT_NAME]</td>
            <td>Sets the .defaultRoute metadata in the <b>trakhound.package.json</b> file</td>
        </tr>    
        <tr>
            <td>managementServer</td>
            <td></td>
            <td>Sets the TrakHound Management Server to publish to in the <b>trakhound.package.publish.json</b> file</td>
        </tr>       
        <tr>
            <td>organization</td>
            <td></td>
            <td>Sets the Organization to publish to in the <b>trakhound.package.publish.json</b> file</td>
        </tr>   
    </tbody>
</table>

## Build & Develop

#### dotnet CLI
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to build the project using the below command:
```
dotnet build -c:Debug
```

#### Run
```
dotnet run -c:Debug
```

#### Open
Access the API using a web browser: http://localhost:5000


## Publish Package
Publishing packages can be done using the **[TrakHound-CLI](https://github.com/TrakHound/TrakHound/releases/latest)** tool. Included in this template is a **publish-package.bat** (or publish-package.sh) file to simplify publishing.

### Windows
```
publish-package.bat
```

### Linux
```
publish-package.sh
```

> [!NOTE]
> You must set the **managementServer** and **organization** in the "trakhound.package.publish.json" file. During the Beta phase, you may upload to the "https://www.trakhound.com/management" using an Organization but note that any package published to this server will technically be public. We will soon be implementing private User accounts with the option to make a package Public. You can host packages on a self-hosted server using the "TrakHound.Production.Plan", contact info@trakhound.com for more information.