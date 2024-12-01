# Packages
TrakHound Packages are used to distribute Apps, APIs, Services, Drivers, etc. along with other files and data. Packages can be used to distribute modules across multiple sites or from a public server. This can be used so that other modules can reference the package as a dependency and utilize what is already in another package.  Packages are also versioned so that a specific version can be referenced as a dependency or so that updates can be rolled back as needed.

## Package Installation
TrakHound Packages can be installed using the <a href="/docs/trakhound-cli/packages#top">TrakHound CLI</a> packages commands. This downloads the package from the TrakHound Management Server and installs it in the current directory's <strong>packages</strong> directory.
```
th packages install demo.app
```

## Creating Packages
TrakHound Packages can be created using the <a href="/docs/trakhound-cli/packages#top">TrakHound CLI</a> packages commands. This packages the current directory using the configuration set in a <strong>packages.json</strong> file.
```
th packages pack
```
