// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Diagnostics;

namespace TrakHound.CLI.Applications
{
    [CommandGroup("applications", "Download and Install TrakHound Applications that are configured for an Organization")]
    internal static class Applications
    {
        private const string _defaultRevisionDatabase = "packages.db";


        [Command("install", "Download and Install the specified Application")]
        public static async Task Install(
            [CommandParameter($"The Profile ID of the Application to Install")]
            string profileId,
            [CommandOption("The Version of the Application to Install. If not specified, the latest version is installed")]
            string version = null,
            [CommandOption("The URL of the TrakHound Management Server to download the application from")]
            string server = null,
            [CommandOption("The Organization that the application is stored under")]
            string organization = null
            )
        {
            Console.WriteLine($"Install Application : {profileId} : version = {version}");

            var managementClient = await ManagementClient.Get(server, organization);
            if (managementClient != null)
            {
                Console.Write("Retrieving Application Information..");

                var application = await managementClient.Applications.GetApplication(profileId, version);
                if (application != null)
                {
                    Console.WriteLine("Done");

                    Console.Write("Downloading Application..");

                    var applicationBytes = await managementClient.Applications.DownloadApplication(application.Id);
                    if (!applicationBytes.IsNullOrEmpty())
                    {
                        var tempDir = TrakHoundTemp.CreateDirectory();
                        var filePath = Path.Combine(tempDir, application.Filename);
                        File.WriteAllBytes(filePath, applicationBytes);

                        Console.WriteLine("Done");

                        if (File.Exists(filePath))
                        {
                            Console.Write("Installing Application..");

                            var process = Process.Start(filePath);
                            process.WaitForExit();

                            if (process.ExitCode >= 0)
                            {
                                Console.WriteLine("Done");
                            }
                            else
                            {
                                Console.WriteLine("Canceled");
                            }
                        }
                    }
                }
            }
        }

        [Command("download")]
        public static async Task Download(
            [CommandParameter($"The Profile ID of the Application to Download")]
            string profileId,
            [CommandOption("The Version of the Application to Download. If not specified, the latest version is downloaded")]
            string version = null,
            [CommandOption("The path of the directory to save the downloaded file. If not specified, the current working directory is used")]
            string path = null,
            [CommandOption("The URL of the TrakHound Management Server to download the application from")]
            string server = null,
            [CommandOption("The Organization that the application is stored under")]
            string organization = null
            )
        {
            Console.WriteLine($"Download Application : {profileId} : version = {version}");

            var savePath = !string.IsNullOrEmpty(path) ? path : Environment.CurrentDirectory;

            var managementClient = await ManagementClient.Get(server, organization);
            if (managementClient != null)
            {
                Console.Write("Retrieving Application Information..");

                var application = await managementClient.Applications.GetApplication(profileId, version);
                if (application != null)
                {
                    Console.WriteLine("Done");

                    Console.Write("Downloading Application..");

                    var applicationBytes = await managementClient.Applications.DownloadApplication(application.Id);
                    if (!applicationBytes.IsNullOrEmpty())
                    {
                        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
                        var filePath = Path.Combine(savePath, application.Filename);
                        File.WriteAllBytes(filePath, applicationBytes);

                        Console.WriteLine("Done");

                        Console.WriteLine($"Path : {filePath}");
                    }
                }
            }
        }
    }
}
