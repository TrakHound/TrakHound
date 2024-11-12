// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Configurations;
using TrakHound.Deployments;
using TrakHound.Packages;

namespace TrakHound.CLI.Deployments
{
    [CommandGroup("deployments")]
    internal static class Deployments
    {
        private const string _defaultOrganization = "default";
        private const string _defaultRevisionDatabase = "packages.db";


        [Command("install", "Install a Deployment from a local File")]
        public static async Task InstallLocal(
            [CommandParameter("The path of the Deployment file (.thd)")]
            string target,
            [CommandParameter("The path of the directory to save the downloaded file. If not specified, the current working directory is used")]
            string path = null
            )
        {
            var installPath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;
            if (!Path.IsPathRooted(installPath)) installPath = Path.Combine(Environment.CurrentDirectory, installPath);

            Console.WriteLine($"Install Local Deployment : {target} : to {installPath}");

            if (!string.IsNullOrEmpty(target) && File.Exists(target))
            {
                Console.Write("Reading Deployment Information from File..");

                var deploymentBytes = await File.ReadAllBytesAsync(target);
                if (deploymentBytes != null)
                {
                    var deployment = TrakHoundDeployment.ReadInformationFromDeployment(deploymentBytes);
                    if (deployment != null)
                    {
                        Console.WriteLine("Done");

                        Console.WriteLine();
                        Console.WriteLine(string.Format("{0,-12} : {1}", "ID", deployment.Id));
                        Console.WriteLine(string.Format("{0,-12} : {1}", "Profile ID", deployment.ProfileId));
                        Console.WriteLine(string.Format("{0,-12} : {1}", "Version", deployment.Version));
                        Console.WriteLine(string.Format("{0,-12} : {1}", "BuildDate", deployment.BuildDate));
                        Console.WriteLine(string.Format("{0,-12} : {1}", "Description", deployment.Description));
                        Console.WriteLine();

                        Console.Write("Installing Deployment..");

                        var installResult = TrakHoundDeployment.Install(deploymentBytes, installPath);
                        if (installResult.Success)
                        {
                            Console.WriteLine("Done");
                        }
                        else
                        {
                            Console.WriteLine("ERROR");
                            if (!installResult.Errors.IsNullOrEmpty())
                            {
                                foreach (var error in installResult.Errors)
                                {
                                    Console.WriteLine($"ERROR : {error}");
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("install-remote", "Download and Install the specified Deployment")]
        public static async Task Install(
            [CommandParameter($"The Profile ID of the Deployment to Install")]
            string profileId,
            [CommandParameter("The path of the directory to save the downloaded file. If not specified, the current working directory is used")]
            string path = null,
            [CommandOption("The Version of the Deployment to Install. If not specified, the latest version is installed")]
            string version = null,
            [CommandOption("The URL of the TrakHound Management Server to download the deployment from")]
            string server = null,
            [CommandOption("The Organization that the deployment is stored under")]
            string organization = null
            )
        {
            var installPath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;
            if (!Path.IsPathRooted(installPath)) installPath = Path.Combine(Environment.CurrentDirectory, installPath);

            Console.WriteLine($"Install Deployment : {profileId} : version = {version} : to {installPath}");

            var managementClient = await ManagementClient.Get(server, organization);
            if (managementClient != null)
            {
                Console.Write("Retrieving Deployment Information..");

                var deployment = await managementClient.Deployments.GetDeployment(profileId, version);
                if (deployment != null)
                {
                    Console.WriteLine("Done");

                    Console.WriteLine();
                    Console.WriteLine(string.Format("{0,-12} : {1}", "ID", deployment.Id));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "Profile ID", deployment.ProfileId));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "Version", deployment.Version));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "BuildDate", deployment.BuildDate));
                    Console.WriteLine(string.Format("{0,-12} : {1}", "Description", deployment.Description));
                    Console.WriteLine();

                    Console.Write("Downloading Deployment..");

                    var deploymentBytes = await managementClient.Deployments.DownloadDeployment(deployment.Id);
                    if (!deploymentBytes.IsNullOrEmpty())
                    {
                        Console.WriteLine("Done");

                        Console.Write("Installing Deployment..");

                        var installResult = TrakHoundDeployment.Install(deploymentBytes, installPath);
                        if (installResult.Success)
                        {
                            Console.WriteLine("Done");

                            Console.WriteLine("Restoring Packaging..");

                            await Packages.Packages.Install(path: installPath, server: server, organization: organization);
                        }
                        else
                        {
                            Console.WriteLine("ERROR");
                            if (!installResult.Errors.IsNullOrEmpty())
                            {
                                foreach (var error in installResult.Errors)
                                {
                                    Console.WriteLine($"ERROR : {error}");
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("create", "Create a new Deployment")]
        public static async Task Create(
            [CommandParameter($"The Profile ID of the Deployment to Install")]
            string profileId,
            [CommandParameter("The path of the root directory to create the Deployment from. If not specified, the current working directory is used")]
            string path = null,
            [CommandOption("The Version of the Deployment to Install. If not specified, the latest version is installed")]
            string version = null,
            [CommandOption("The path of the directory to write the created Deployment to. If not specified, the current working directory is used")]
            string output = null
            )
        {
            var sourcePath = !string.IsNullOrEmpty(path) ? path.Trim('"') : Environment.CurrentDirectory;
            if (!Path.IsPathRooted(sourcePath)) sourcePath = Path.Combine(Environment.CurrentDirectory, sourcePath);

            var outputPath = !string.IsNullOrEmpty(output) ? output.Trim('"') : Environment.CurrentDirectory;
            if (!Path.IsPathRooted(outputPath)) outputPath = Path.Combine(Environment.CurrentDirectory, outputPath);

            Console.WriteLine($"Create Deployment : {profileId} : version = {version} : from {sourcePath}");

            var deployment = new TrakHoundDeployment();
            deployment.ProfileId = profileId;
            deployment.Version = version;
            deployment.BuildDate = DateTime.UtcNow;

            Console.Write("Creating Deployment..");

            var deploymentResult = deployment.Create(sourcePath);
            if (deploymentResult.Success)
            {
                Console.WriteLine("Done");

                string outputFilename;
                if (!string.IsNullOrEmpty(version)) outputFilename = $"{profileId}-{version}{TrakHoundDeployment.FileExtension}";
                else outputFilename = $"{profileId}{TrakHoundDeployment.FileExtension}";

                var writePath = Path.Combine(outputPath, outputFilename);
                Console.Write($"Writing Deployment to ({writePath})..");

                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
                await File.WriteAllBytesAsync(writePath, deploymentResult.Content);
                Console.WriteLine("Done");
            }
            else
            {
                Console.WriteLine("ERROR");
            }
        }

        [Command("clean")]
        public static void Clean(
            [CommandParameter("The path of the directory to clean. If not specified, the current working directory is used")]
            string path = null
            )
        {
            var installPath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;
            if (!Path.IsPathRooted(installPath)) installPath = Path.Combine(Environment.CurrentDirectory, installPath);

            Console.WriteLine($"Cleaning Deployment from {installPath}");

            Console.Write("Removing Packages..");
            var packagesDir = TrakHoundPackage.GetDirectory(installPath);
            if (Directory.Exists(packagesDir)) Directory.Delete(packagesDir, true);
            Console.WriteLine("Done");

            Console.Write("Removing Configurations..");
            var configurationsDir = Path.Combine(installPath, TrakHoundConfigurations.Directory);
            if (Directory.Exists(configurationsDir)) Directory.Delete(configurationsDir, true);
            Console.WriteLine("Done");

            Console.Write("Removing Packages File..");
            var packagesFile = Path.Combine(installPath, TrakHoundPackagesFile.Filename);
            if (File.Exists(packagesFile)) File.Delete(packagesFile);
            Console.WriteLine("Done");

            Console.Write("Removing Deployment File..");
            var deploymentFile = Path.Combine(installPath, TrakHoundDeployment.DeploymentFile);
            if (File.Exists(deploymentFile)) File.Delete(deploymentFile);
            Console.WriteLine("Done");
        }

        //[Command("download")]
        //public static async Task Download(
        //    [CommandParameter($"The Profile ID of the Deployment to Download")]
        //    string profileId,
        //    [CommandOption("The Version of the Deployment to Download. If not specified, the latest version is downloaded")]
        //    string version = null,
        //    [CommandOption("The path of the directory to save the downloaded file. If not specified, the current working directory is used")]
        //    string path = null,
        //    [CommandOption("The URL of the TrakHound Management Server to download the deployment from")]
        //    string server = null,
        //    [CommandOption("The Organization that the deployment is stored under")]
        //    string organization = _defaultOrganization
        //    )
        //{
        //    Console.WriteLine($"Download Deployment : {profileId} : version = {version}");

        //    var savePath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

        //    var managementClient = await ManagementClient.Get(server);
        //    if (managementClient != null)
        //    {
        //        Console.Write("Retrieving Deployment Information..");

        //        var deployment = await managementClient.Deployments.GetDeployment(organization, profileId, version);
        //        if (deployment != null)
        //        {
        //            Console.WriteLine("Done");

        //            Console.Write("Downloading Deployment..");

        //            var deploymentBytes = await managementClient.Deployments.DownloadDeployment(organization, deployment.Id);
        //            if (!deploymentBytes.IsNullOrEmpty())
        //            {
        //                if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
        //                var filePath = Path.Combine(savePath, deployment.Filename);
        //                File.WriteAllBytes(filePath, deploymentBytes);

        //                Console.WriteLine("Done");

        //                Console.WriteLine($"Path : {filePath}");
        //            }
        //        }
        //    }
        //}
    }
}
