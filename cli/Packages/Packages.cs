// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Management;
using TrakHound.Packages;

namespace TrakHound.CLI.Packages
{
    [CommandGroup("packages")]
    internal static class Packages
    {
        private const string _defaultOrganization = "default";


        [Command("list", "List the Installed Packages")]
        public static void List(
            [CommandParameter("The path to the directory where the Packages are installed")]
            string path = null,
            [CommandOption("The Category of the Packages to list")]
            string category = null
            )
        {
            Console.WriteLine($"Get Packages : path = {path} : category = {category}");

            var packages = TrakHoundPackage.GetInstalled(path);

            if (!string.IsNullOrEmpty(category))
            {
                packages = packages?.Where(x => x.Category == category);
            }

            if (packages != null)
            {
                foreach (var package in packages)
                {
                    Console.WriteLine($"{package.Category} : {package.Id} : {package.Version}");
                }
            }
            else
            {
                Console.WriteLine($"No Packages Found : {path} : {category}");
            }
        }


        [Command("install", "Download and Install the specified Package")]
        public static async Task Install(
            [CommandParameter($"The ID of the Package to Install. If not specified, the {TrakHoundPackage.PackageConfigurationFilename} file is processed")]
            string packageId = null,
            [CommandOption("The Version of the Package to Install. If not specified, the latest package is installed")]
            string version = null,
            [CommandOption("The path to the directory to install the package to")]
            string path = null,
            [CommandOption("The URL of the TrakHound Management Server to download the package from")]
            string server = null,
            [CommandOption("The Organization that the package is stored under")]
            string organization = null
            )
        {
            Console.WriteLine($"Install Package : {packageId} : version = {version} : path = {path}");

            var rootPath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;
            var installPackages = new List<TrakHoundPackageDependency>();

            // Read TrakHound Packages File
            var packagesFile = TrakHoundPackagesFile.Read(rootPath);

            if (!string.IsNullOrEmpty(packageId))
            {
                installPackages.Add(new TrakHoundPackageDependency(packageId, version));
            }
            else
            {
                if (packagesFile != null && !packagesFile.Packages.IsNullOrEmpty())
                {
                    foreach (var filePackage in packagesFile.Packages)
                    {
                        installPackages.Add(new TrakHoundPackageDependency(filePackage.Id, filePackage.Version));
                    }
                }
            }

            Console.WriteLine($"Installing Packages to ({rootPath})");

            foreach (var installPackage in installPackages)
            {
                await InstallPackage(installPackage, organization, server, rootPath);
            }

            Console.Write("Updating Packages File...");

            var installedPackages = TrakHoundPackage.GetInstalled(rootPath);
            if (!installedPackages.IsNullOrEmpty())
            {
                var autoUpdates = packagesFile != null ? packagesFile.AutoUpdates : null;

                var fileItems = new List<TrakHoundPackagesFileItem>();
                foreach (var installedPackage in installedPackages)
                {
                    fileItems.Add(new TrakHoundPackagesFileItem(installedPackage.Category, installedPackage.Id, installedPackage.Version));
                }

                var file = new TrakHoundPackagesFile();
                file.Packages = fileItems;
                file.AutoUpdates = autoUpdates;

                TrakHoundPackagesFile.Write(fileItems, autoUpdates, rootPath);

                Console.WriteLine("DONE");
            }
        }


        [Command("uninstall", "Uninstall the specified Package")]
        public static void Uninstall(
            [CommandParameter] string packageId,
            [CommandOption] string version = null,
            [CommandOption] string path = null
            )
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                var rootPath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

                Console.Write($"Uninstalling Package ({packageId})...");

                if (TrakHoundPackage.Uninstall(packageId, version, rootPath))
                {
                    Console.WriteLine("DONE");
                    Console.WriteLine("Package Uninstalled : " + packageId + " => " + version);
                }
                else
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine("Error Uninstalling Package. Please try again");
                }
            }
        }


        [Command("pack", "Create a new Package from the specified directory (path)")]
        public static void Pack(
            [CommandParameter] string path,
            [CommandOption] string configuration = null,
            [CommandOption] string category = null,
            [CommandOption] string id = null,
            [CommandOption] string version = null,
            [CommandOption] string description = null,
            [CommandOption] string publisher = null,
            [CommandOption] string output = null
            )
        {
            var sourcePath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

            var configurationPath = !string.IsNullOrEmpty(configuration) ? configuration : sourcePath;
            if (!File.Exists(configurationPath)) configurationPath = Path.Combine(configurationPath, TrakHoundPackage.PackageConfigurationFilename);

            var outputPath = !string.IsNullOrEmpty(output) ? output : sourcePath;

            var packageBytes = CreatePackage(sourcePath, configurationPath, category, id, version, description, publisher);
            if (packageBytes != null && packageBytes.Length > 0)
            {
                var information = TrakHoundPackage.ReadInformationFromPackage(packageBytes);
                if (information != null)
                {
                    // Set filename ([PackageId]-[Version].thp)
                    var filename = information.Id + "-" + information.Version + TrakHoundPackage.FileExtension;

                    // Set destination path
                    var destinationPath = Path.Combine(outputPath, filename);

                    Console.Write($"Writing Package File...");

                    // Write bytes to file
                    File.WriteAllBytes(destinationPath, packageBytes);

                    Console.WriteLine("Done");
                    Console.WriteLine($"TrakHound Package ({filename}) Created Successfully:");
                    Console.WriteLine(destinationPath);
                }
            }
        }


        [Command("publish")]
        public static async Task Publish(
            [CommandParameter] string path = null,
            [CommandOption] string configuration = null,
            [CommandOption] string category = null,
            [CommandOption] string id = null,
            [CommandOption] string version = null,
            [CommandOption] string description = null,
            [CommandOption] string publisher = null,
            [CommandOption] string server = null,
            [CommandOption] string organization = null
            )
        {
            var sourcePath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

            var configurationPath = !string.IsNullOrEmpty(configuration) ? configuration : sourcePath;
            if (!File.Exists(configurationPath)) configurationPath = Path.Combine(configurationPath, TrakHoundPackage.PackageConfigurationFilename);

            var packageBytes = CreatePackage(sourcePath, configurationPath, category, id, version, description, publisher);
            if (packageBytes != null && packageBytes.Length > 0)
            {
                // Read Publish Profile File
                var publishProfilePath = Path.Combine(sourcePath, TrakHoundPackagePublishProfile.Filename);
                var publishProfile = Json.Read<TrakHoundPackagePublishProfile>(publishProfilePath);
                if (publishProfile != null && !publishProfile.Destinations.IsNullOrEmpty())                  
                {
                    Console.WriteLine($"Package Publish Profile Read : ({publishProfilePath})");

                    foreach (var publishDestination in publishProfile.Destinations)
                    {
                        Console.WriteLine($"Publishing Package to Destination : ({publishDestination.ManagementServer} | {publishDestination.Organization})");

                        await PublishPackage(packageBytes, publishDestination.Organization, publishDestination.ManagementServer);
                    }
                }
                else
                {
                    await PublishPackage(packageBytes, organization, server);
                }
            }
        }


        [Command("push")]
        public static async Task Push(
            [CommandParameter] string path,
            [CommandOption] string server = null,
            [CommandOption] string organization = null
            )
        {
            if (!string.IsNullOrEmpty(path))
            {
                Console.Write($"Uploading Package File ({path})...");

                try
                {
                    var packageBytes = File.ReadAllBytes(path);

                    var managementClient = await ManagementClient.Get(server, organization);

                    var package = await managementClient.Packages.Upload(packageBytes);
                    if (package != null)
                    {
                        Console.WriteLine("Done");
                        Console.WriteLine("Package Uploaded Successfully");
                    }
                    else
                    {
                        Console.WriteLine("ERROR Uploading Package");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR");
                    Console.WriteLine(ex.Message);
                }
            }
        }


        [Command("dotnet")]
        public static async Task DotNet(
            [CommandParameter] string path = null,
            [CommandOption] string output = null,
            [CommandOption] string projectConfiguration = "Release",
            [CommandOption] bool includeSource = false,
            [CommandOption] bool publish = false,
            [CommandOption] string server = null,
            [CommandOption] string organization = null
            )
        {
            if (!publish && output == null) output = Environment.CurrentDirectory;

            // Set Package Source (directory to read from)
            var packageSource = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

            // Add Working directory (if path is not rooted)
            var rootPath = packageSource;
            if (!Path.IsPathRooted(rootPath))
            {
                rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, packageSource);
            }

            // Add Working directory (if output is not rooted)
            var outputPath = output;
            if (outputPath != null && !Path.IsPathRooted(outputPath))
            {
                outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputPath);
            }


            string version = null;
            var informationPath = Path.Combine(rootPath, TrakHoundPackage.PackageConfigurationFilename);
            var information = TrakHoundPackage.ReadInformationFromFile(informationPath);
            if (information != null)
            {
                Console.WriteLine($"Package Information Read : ({informationPath})");

                // Read Publish Profile File
                var publishProfilePath = Path.Combine(rootPath, TrakHoundPackagePublishProfile.Filename);
                var publishProfile = Json.Read<TrakHoundPackagePublishProfile>(publishProfilePath);
                if (publishProfile != null)
                {
                    Console.WriteLine($"Package Publish Profile Read : ({publishProfilePath})");

                    // Increment Version
                    var latestVersion = publishProfile.Version.ToVersion();
                    if (latestVersion != null)
                    {
                        version = new Version(latestVersion.Major, latestVersion.Minor, latestVersion.Build + 1).ToString();
                    }

                    //if (string.IsNullOrEmpty(server)) server = publishProfile.ManagementServer;
                    //if (string.IsNullOrEmpty(organization)) organization = publishProfile.Organization;
                }

                Console.Write($"Packing DotNet Project...");

                var dotnetSettings = new TrakHoundDotNetPackageSettings();
                dotnetSettings.Version = version;
                dotnetSettings.Configuration = projectConfiguration;
                dotnetSettings.IncludeDebugSymbols = projectConfiguration == "Debug";
                dotnetSettings.IncludeSource = includeSource;

                // Set Git Parameters
                dotnetSettings.GitBranch = GitCommands.GetCurrentBranch(rootPath);
                dotnetSettings.GitCommit = GitCommands.GetLatestCommit(rootPath);

                var packageBytes = TrakHoundDotNetPackage.Create(rootPath, dotnetSettings);
                Console.WriteLine("Done");

                if (packageBytes != null && packageBytes.Length > 0)
                {
                    information = TrakHoundPackage.ReadInformationFromPackage(packageBytes);
                    if (information != null)
                    {
                        Console.WriteLine($"TrakHound Package ({information.Id} : v{information.Version}) Created Successfully:");

                        // Update Publish Profile File
                        if (publishProfile != null)
                        {
                            publishProfile.Version = version;
                            Json.Write(publishProfile, publishProfilePath);
                        }

                        if (outputPath != null)
                        {
                            // Set filename ([PackageId]-[Version].thp)
                            var filename = information.Id + "-" + information.Version + TrakHoundPackage.FileExtension;

                            // Set destination path
                            var destinationPath = Path.Combine(outputPath, filename);

                            // Create Output Directory (if not exists)
                            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

                            Console.Write($"Writing Package File...");

                            // Write bytes to file
                            File.WriteAllBytes(destinationPath, packageBytes);

                            Console.WriteLine("Done");
                            Console.WriteLine($"TrakHound Package ({filename}) File Created Successfully:");
                            Console.WriteLine(destinationPath);
                        }

                        if (publish)
                        {
                            if (publishProfile != null && !publishProfile.Destinations.IsNullOrEmpty())
                            {
                                foreach (var publishDestination in publishProfile.Destinations)
                                {
                                    Console.WriteLine($"Publishing Package to Destination : ({publishDestination.ManagementServer} | {publishDestination.Organization})");

                                    await PublishPackage(packageBytes, publishDestination.Organization, publishDestination.ManagementServer);
                                }
                            }
                            else
                            {
                                await PublishPackage(packageBytes, organization, server);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error : Package Information File not Found : ({informationPath})");
            }
        }


        private static byte[] CreatePackage(
            string path,
            string configurationPath = null,
            string category = null,
            string id = null,
            string version = null,
            string description = null,
            string publisher = null
            )
        {
            // Set Package Category
            string packageCategory = category;

            // Set Package ID
            string packageId = id;

            // Set Package Source (directory to read from)
            var packageSource = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

            // Add Working directory (if path is not rooted)
            var rootPath = packageSource;
            if (!Path.IsPathRooted(rootPath))
            {
                rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, packageSource);
            }

            Console.WriteLine($"Package Source = \"{rootPath}\"");
            Console.Write($"Packing Files...");

            if (Directory.Exists(rootPath))
            {
                Dictionary<string, object> metadata = null;
                IEnumerable<TrakHoundPackageDependency> dependencies = null;

                // Read Configuration File
                if (!string.IsNullOrEmpty(configurationPath))
                {
                    var configuration = Json.Read<TrakHoundPackageJson>(configurationPath);
                    if (configuration != null)
                    {
                        if (string.IsNullOrEmpty(packageCategory)) packageCategory = configuration.Category;
                        if (string.IsNullOrEmpty(packageId)) packageId = configuration.Id;
                        if (string.IsNullOrEmpty(version)) version = configuration.Version;

                        if (configuration.Metadata != null)
                        {
                            //if (string.IsNullOrEmpty(name) && configuration.Metadata.ContainsKey("name")) name = configuration.Metadata["name"].ToString();
                            if (string.IsNullOrEmpty(description) && configuration.Metadata.ContainsKey("description")) description = configuration.Metadata["description"].ToString();
                            if (string.IsNullOrEmpty(publisher) && configuration.Metadata.ContainsKey("publisher")) publisher = configuration.Metadata["publisher"].ToString();
                        }

                        metadata = configuration.Metadata;
                        dependencies = configuration.Dependencies;
                    }
                }

                // Read Publish Profile File
                var publishProfilePath = Path.Combine(rootPath, TrakHoundPackagePublishProfile.Filename);
                var publishProfile = Json.Read<TrakHoundPackagePublishProfile>(publishProfilePath);
                if (publishProfile != null)
                {
                    Console.WriteLine($"Package Publish Profile Read : ({publishProfilePath})");

                    // Increment Version
                    var latestVersion = publishProfile.Version.ToVersion();
                    if (latestVersion != null)
                    {
                        version = new Version(latestVersion.Major, latestVersion.Minor, latestVersion.Build + 1).ToString();
                    }
                }


                // Create Package Model
                var packageBuilder = new TrakHoundPackageBuilder();
                packageBuilder.Category = packageCategory;
                packageBuilder.Id = packageId;
                packageBuilder.Version = version;
                packageBuilder.BuildDate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(description)) packageBuilder.Metadata["description"] = description;
                if (!string.IsNullOrEmpty(publisher)) packageBuilder.Metadata["publisher"] = publisher;

                if (!metadata.IsNullOrEmpty())
                {
                    foreach (var o in metadata)
                    {
                        if (!string.IsNullOrEmpty(o.Key) && !packageBuilder.Metadata.ContainsKey(o.Key))
                        {
                            packageBuilder.Metadata.Add(o.Key, o.Value);
                        }
                    }
                }

                packageBuilder.Dependencies = dependencies;

                var package = packageBuilder.Build();

                // Create a new Package (ZipFile) from the directory passed as argument
                var packageBytes = package.Package(rootPath);


                // Update Publish Profile File
                if (publishProfile != null)
                {
                    publishProfile.Version = version;
                    Json.Write(publishProfile, publishProfilePath);
                }


                Console.WriteLine("Done");
                Console.WriteLine($"TrakHound Package ({package.Id} v{package.Version}) Created Successfully:");

                return packageBytes;
            }

            return null;
        }

        private static async Task InstallPackage(ITrakHoundPackageDependency package, string organization, string server, string rootPath)
        {
            try
            {
                Console.WriteLine("-----");
                Console.Write("Downloading Package Information : " + server + "...");

                var managementClient = await ManagementClient.Get(server, organization);

                // Get Package from Server
                TrakHoundPackage serverPackage;
                if (!string.IsNullOrEmpty(package.Version))
                {
                    serverPackage = await managementClient.Packages.Get(package.Id, package.Version);
                }
                else
                {
                    serverPackage = await managementClient.Packages.GetLatest(package.Id);
                }

                if (serverPackage != null)
                {
                    Console.WriteLine("DONE");

                    Console.WriteLine();
                    Console.WriteLine(string.Format("{0,-12} : {1}", "UUID", serverPackage.Uuid));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "ID", serverPackage.Id));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "Version", serverPackage.Version));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "BuildDate", serverPackage.BuildDate));

                    if (serverPackage.Metadata.IsNullOrEmpty() && serverPackage.Metadata.ContainsKey("Description"))
                    {
                        Console.WriteLine(string.Format("{0,-12} : {1}", "Description", serverPackage.Metadata["Description"]));
                    }
                    Console.WriteLine();

                    Console.WriteLine("Package Information Read : " + serverPackage.Id + " => " + serverPackage.Version + " : " + serverPackage.Category);
                    if (!serverPackage.Dependencies.IsNullOrEmpty())
                    {
                        Console.WriteLine("Reading Package Dependencies...");

                        //var packageQueries = new List<TrakHoundPackageQuery>();
                        //foreach (var dependency in serverPackage.Dependencies)
                        //{
                        //    packageQueries.Add(new TrakHoundPackageQuery(dependency.Id, dependency.Version));
                        //}

                        //var packagesArchive = managementClient.Packages.DownloadArchive(organization, packageQueries);
                        //if (packagesArchive != null)
                        //{

                        //}

                        foreach (var dependency in serverPackage.Dependencies)
                        {
                            await InstallPackage(dependency, organization, server, rootPath);
                        }
                    }
                    var install = true;
                    var installedPackageInformation = TrakHoundPackage.ReadInformation(serverPackage.Category, serverPackage.Id, serverPackage.Version, rootPath);
                    if (installedPackageInformation != null)
                    {
                        Console.WriteLine("Installed Package Information Read : " + serverPackage.Id + " => " + serverPackage.Version + " : " + serverPackage.Category);
                        install = serverPackage.Hash != installedPackageInformation.Hash;
                    }

                    if (install)
                    {
                        Console.WriteLine("-----");
                        Console.Write("Downloading Package Contents : " + server + "...");

                        var packageBytes = await managementClient.Packages.Download(serverPackage.Uuid);
                        if (packageBytes != null)
                        {
                            Console.WriteLine("DONE");

                            Console.Write($"Installing Package ({serverPackage.Id})...");

                            if (TrakHoundPackage.Install(packageBytes, rootPath))
                            {
                                Console.WriteLine("DONE");
                                Console.WriteLine("Package Installed : " + serverPackage.Id + " => " + serverPackage.Version + " : " + serverPackage.Category);
                            }
                            else
                            {
                                Console.WriteLine("ERROR");
                                Console.WriteLine("Error Installing Package. Please try again");
                            }
                        }
                        else
                        {
                            Console.WriteLine("ERROR");
                            Console.WriteLine("Error Downloading Package. Please try again");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Package Already Installed");
                    }
                }
            }
            catch { }
        }

        private static async Task PublishPackage(byte[] packageBytes, string organization = null, string server = null)
        {
            if (packageBytes != null && packageBytes.Length > 0)
            {
                var managementClient = new TrakHoundManagementClient(server, organization);
                Console.Write($"Uploading Package File ({managementClient.BaseUrl} | {managementClient.Organization})...");

                var package = await managementClient.Packages.Upload(packageBytes);
                if (package != null)
                {
                    Console.WriteLine("Done");
                    Console.WriteLine("Package Uploaded Successfully");
                }
                else
                {
                    Console.WriteLine("ERROR Uploading Package");
                }
            }
        }
    }
}
