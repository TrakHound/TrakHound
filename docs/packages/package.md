# TrakHound Package
Packages are used to distribute Apps, APIs, Services, Drivers, etc. so that multiple TrakHound Instances can share packages. A TrakHound Management Server can be used to download/upload Packages and can act as a central repository for Packages.

Packages can add other packages as dependencies which can be used to bundle multiple packages together.

Packages are often used to distribute Modules that can then be loaded into a TrakHound Instance. This separation allows the TrakHound Instance application to remain Open Source while individual Packages can be a mix of Open  Source and Licensed.

<ul class="toc">
    <li><a href="#contents">Contents</a></li>
    <li><a href="#package-json">package.json</a></li>
    <li><a href="#cli">CLI</a></li>
    <li><a href="#api">API</a></li>
</ul>

> [!NOTE]
> Think of TrakHound Packages as <strong>NPM</strong> or <strong>Nuget</strong> but for industrial applications

## Contents
The content can be anything but it does need to follow a certain structure:

> [!NOTE]
> A package always has a file extension of <strong>.thp</strong> and the contents of the file are the following contents in a zipped file.

<table>
    <thead>
        <tr>
            <th>Path</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td class="row-header">/package.json</td>
            <td>Information about the Package</td>
        </tr>
        <tr>
            <td class="row-header">/dist</td>
            <td>Directory to contain what the package is distributing</td>
        </tr>
        <tr>
            <td class="row-header">/src</td>
            <td>(Optional) Directory to contain the source files for the package</td>
        </tr>
        <tr>
            <td class="row-header">/Readme.md</td>
            <td>(Optional) A readme file to contain extended information</td>
        </tr>
    </tbody>
</table>

## package.json
A <strong>package.json</strong> file is required to provide information about the Package. The package.json file must be located in the root of the package contents.

### Example:
```json
{
  "uuid": "b1a7f411c841d5485e6c46a76036c76c",
  "id": "TrakHound.Mqtt.Drivers",
  "version": "1.0.18",
  "category": "driver",
  "buildDate": "2023-12-13T14:37:33.1015288Z",
  "hash": "79ccb37a77c01401a1a10edb590348f5",
  "metadata": {
    "publisher": "TrakHound Inc.",
    "description": "MQTT Drivers to implement Pub/Sub in the TrakHound Framework",
    ".bufferInterval": 100,
    ".bufferRetryInterval": 5000,
    ".bufferMaxItemsPerInterval": 1000,
    ".bufferQueuedItemLimit": 100000,
    ".fileBufferEnabled": false,
    ".fileBufferReadInterval": 0,
    ".bufferAcknowledgeSent": false
  },
  "dependencies": null
}
```

<table>
    <tbody>
        <tr>
            <td class="row-header">uuid</td>
            <td>Uniquely identifies the package and it's version using the format: <strong>MD5 Hash of [ID]:[VERSION]</strong></td>
        </tr>
        <tr>
            <td class="row-header">id</td>
            <td>Identifier for the package. This is considered the "name" of the package and is typically how a package is referenced.</td>
        </tr>
        <tr>
            <td class="row-header">version</td>
            <td>The Version of the package</td>
        </tr>
        <tr>
            <td class="row-header">category</td>
            <td>The category for the package. This is used to group similar packages togther. Examples of categories: app, api, service, driver, etc.</td>
        </tr>
        <tr>
            <td class="row-header">buildDate</td>
            <td>The ISO-8601 timestamp of when the package was built</td>
        </tr>
        <tr>
            <td class="row-header">hash</td>
            <td></td>
        </tr>
        <tr>
            <td class="row-header">metadata</td>
            <td></td>
        </tr>
        <tr>
            <td class="row-header">dependencies</td>
            <td></td>
        </tr>
    </tbody>
</table>

## CLI
> **Note:** Download and Learn more about the <a href="/docs/trakhound-cli/packages#top">TrakHound-CLI</a> application.

## API
> **Note:** Download and Learn more about the <a href="/docs/trakhound-management/packages#top">TrakHound-Management</a> application.