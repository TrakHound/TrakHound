# TrakHound.Api.Template
Create a .NET Project template for a TrakHound API module. Api modules are used to create a custom programming interface in TrakHound.

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
            <td>Sets the Publisher metadata in the **trakhound.package.json** file</td>
        </tr>   
        <tr>
            <td>description</td>
            <td></td>
            <td>Sets the Description metadata in the **trakhound.package.json** file</td>
        </tr>   
        <tr>
            <td>defaultRoute</td>
            <td>[PROJECT_NAME]</td>
            <td>Sets the .defaultRoute metadata in the **trakhound.package.json** file</td>
        </tr>    
        <tr>
            <td>managementServer</td>
            <td></td>
            <td>Sets the TrakHound Management Server to publish to in the **trakhound.package.publish.json** file</td>
        </tr>       
        <tr>
            <td>organization</td>
            <td></td>
            <td>Sets the Organization to publish to in the **trakhound.package.publish.json** file</td>
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
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to create a TrakHound Package and publish it to the TrakHound Management Server specified in the **trakhound.package.publish.json** file:
```
publish-package.bat
```