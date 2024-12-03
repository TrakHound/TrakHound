# TrakHound CLI
Command Line Interface application to perform tasks within the TrakHound Framework.

## Get Started

### Windows Installation

#### Install DotNet CLI
The TrakHound CLI application requires the [DotNet CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/) to build the package project files and must be installed.

#### Install TrakHound-CLI
Download and run the installer from the link below:

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
            <td>TrakHound-CLI</td>
            <td><img src="https://raw.githubusercontent.com/TrakHound/TrakHound/refs/heads/main/static/windows-logo.svg" style="height:20px;width:20px;vertical-align: middle;" /></td>
            <td>Installer</td>
            <td><a href="https://github.com/TrakHound/TrakHound/releases/latest">https://github.com/TrakHound/TrakHound/releases/latest</a></td>
        </tr>  
    </tbody>
</table>

### Linux Installation

#### Install DotNet CLI (Snap)
The TrakHound CLI application requires the [DotNet CLI](https://learn.microsoft.com/en-us/dotnet/core/install/linux-snap-sdk) to build the package project files and must be installed using Snap using the command below:
```
sudo snap install dotnet-sdk --classic
```

#### Install trakhound-cli (Snap)
Install the latest revision of the **[trakhound-cli](https://snapcraft.io/trakhound-cli)** Snap package using the command below:
```
sudo snap install trakhound-cli --devmode
```
> Note: Install using **devmode** as the trakhound-cli application requires access to the /usr/lib/dotnet-sdk/current directory. This may be changed in a later version to remove the devmode requirement.

Set the 'th' alias to point to trakhound-cli using the command below:
```
sudo snap alias trakhound-cli th
```

## Packages Commands

### List
List the installed packages in the Working Directory using the command below:
```
th packages list
```

### Install
Install the specified package in the Working Directory using the command below:
```
th packages install [PACKAGE_ID]
```

#### Example
Install the package **TrakHound.Entities.Api** using the command below:
```
th packages install TrakHound.Entities.Api
```
