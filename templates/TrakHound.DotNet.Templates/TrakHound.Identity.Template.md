# TrakHound.Identity.Template
Create a .NET Project template for a TrakHound Identity module. Identity modules are used to provide Authentication and Authorization for other TrakHound modules.

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
            <td>TrakHound.DotNet.Templates</td>
            <td><img src="https://img.shields.io/nuget/dt/TrakHound.DotNet.Templates?style=for-the-badge&logo=nuget&label=%20&color=%23333"/></td>
            <td><a href="https://www.nuget.org/packages/TrakHound.DotNet.Templates">https://www.nuget.org/packages/TrakHound.DotNet.Templates</a></td>
        </tr>
    </tbody>
</table>



## Get Started

#### Install Template
Use the **[dotnet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)** to install the TrakHound.DotNet.Templates using the below command:
```
dotnet new install TrakHound.DotNet.Templates
```

#### Create New Project
This will create a new project using the template in the current working directory
```
dotnet new trakhound.identity
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
