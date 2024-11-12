// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrakHound.Deployments
{
    public class TrakHoundDeployment
    {
        public const string FileExtension = ".thd";
        public const string DeploymentDirectory = "deployments";
        public const string InformationFile = "information.json";
        public const string ManifestFile = "manifest.json";
        public const string InformationFileExtension = ".thdi";
        public const string DeploymentFile = "trakhound-deployment.json";

        private const string _configDirectory = "config";
        private const string _packagesDirectory = "packages";
        private const string _packagesFile = "trakhound-packages.json";

        public const string ReadMeFilename = "Readme";
        public static readonly IEnumerable<string> ReadMeFileExtensions = new string[] { ".md", ".txt" };


        [JsonPropertyName("id")]
        public string Id => GenerateId(this);

        [JsonPropertyName("profileId")]
        public string ProfileId { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("buildDate")]
        public DateTime BuildDate { get; set; }

        [JsonPropertyName("hash")]
        public string Hash => GenerateHash(this);

        private readonly Dictionary<string, object> _metadata = new Dictionary<string, object>();
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata
        {
            get => _metadata;
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    foreach (var entry in value)
                    {
                        _metadata.Remove(entry.Key);
                        _metadata.Add(entry.Key, entry.Value);
                    }
                }
            }
        }


        public static string GenerateId(TrakHoundDeployment package)
        {
            if (package != null)
            {
                return GenerateId(package.ProfileId, package.Version);
            }

            return null;
        }

        public static string GenerateId(string profileId, string version)
        {
            if (!string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(version))
            {
                return $"{profileId}:{version}".ToMD5Hash();
            }
            else
            {
                return $"{profileId}".ToMD5Hash();
            }
        }

        public static string GenerateHash(TrakHoundDeployment package)
        {
            if (package != null)
            {
                return $"{package.ProfileId}:{package.Id}:{package.Version}:{package.BuildDate.ToUnixTime()}".ToMD5Hash();
            }

            return null;
        }


        /// <summary>
        /// Create a TrakHoundDeployment from the source directory
        /// </summary>
        public TrakHoundCreateDeploymentResult Create(string sourcePath, DateTime? incrementalTimestamp = null)
        {
            bool success = false;
            TrakHoundDeploymentManifest manifest = null;
            byte[] content = null;
            var messages = new List<string>();
            var errors = new List<string>();


            if (!string.IsNullOrEmpty(sourcePath))
            {
                if (incrementalTimestamp.HasValue) messages.Add($"Creating Incremental Deployment from Source = {sourcePath}");
                else messages.Add($"Creating Clean Deployment from Source = {sourcePath}");

                // Create Manifest
                manifest = TrakHoundDeploymentManifest.Create(sourcePath);

                var localIncrementalTimestamp = incrementalTimestamp?.ToLocalTime();

                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                        {
                            // Create Information File Entry
                            var information = archive.CreateEntry(InformationFile);
                            using (var informationStream = information.Open())
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                };

                                var json = JsonSerializer.Serialize(this, jsonOptions);
                                var bytes = Encoding.ASCII.GetBytes(json);
                                using (var inputStream = new MemoryStream(bytes))
                                {
                                    inputStream.CopyTo(informationStream);
                                }

                                messages.Add($"Information File : Added");
                            }


                            // Create Manifest File Entry
                            var manifestEntry = archive.CreateEntry(ManifestFile);
                            using (var manifestStream = manifestEntry.Open())
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                };

                                var json = JsonSerializer.Serialize(manifest, jsonOptions);
                                var bytes = Encoding.ASCII.GetBytes(json);
                                using (var inputStream = new MemoryStream(bytes))
                                {
                                    inputStream.CopyTo(manifestStream);
                                }

                                messages.Add($"Manifest File : Added");
                            }


                            // Add Configuration Files
                            var configDir = Path.Combine(sourcePath, _configDirectory);
                            if (Directory.Exists(configDir))
                            {
                                var profileDirectories = Directory.GetDirectories(configDir);
                                if (!profileDirectories.IsNullOrEmpty())
                                {
                                    foreach (var profileDirectory in profileDirectories)
                                    {
                                        // Add Source files from Directory
                                        var files = Directory.GetFiles(profileDirectory, "*.*", SearchOption.AllDirectories);
                                        foreach (var file in files.Where(o => !o.EndsWith(FileExtension)))
                                        {
                                            var fileInfo = new FileInfo(file);
                                            if (!localIncrementalTimestamp.HasValue || fileInfo.LastWriteTime > localIncrementalTimestamp.Value)
                                            {
                                                // Get path Relative to Archive root
                                                var entry = Path.GetRelativePath(sourcePath, file);

                                                // Add Entry to Archive
                                                archive.CreateEntryFromFile(file, entry);

                                                messages.Add($"Configuration : Archive Entry Added : {entry}");
                                            }
                                        }
                                    }
                                }
                            }

                            // Add Package Files
                            var packagesDir = Path.Combine(sourcePath, _packagesDirectory);
                            if (Directory.Exists(packagesDir))
                            {
                                var categoryDirectories = Directory.GetDirectories(packagesDir);
                                foreach (var categoryDirectory in categoryDirectories)
                                {
                                    var packageDirectories = Directory.GetDirectories(categoryDirectory);
                                    foreach (var packageDirectory in packageDirectories)
                                    {
                                        var packageVersionDirectories = Directory.GetDirectories(packageDirectory);
                                        foreach (var packageVersionDirectory in packageVersionDirectories)
                                        {
                                            // Add Source files from Directory
                                            var files = Directory.GetFiles(packageVersionDirectory, "*.*", SearchOption.AllDirectories);
                                            foreach (var file in files.Where(o => !o.EndsWith(FileExtension)))
                                            {
                                                var fileInfo = new FileInfo(file);
                                                if (!localIncrementalTimestamp.HasValue || fileInfo.LastWriteTime > localIncrementalTimestamp.Value)
                                                {
                                                    // Get path Relative to Archive root
                                                    var entry = Path.GetRelativePath(sourcePath, file);

                                                    // Add Entry to Archive
                                                    archive.CreateEntryFromFile(file, entry);

                                                    messages.Add($"Packages : Archive Entry Added : {entry}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        success = true;
                        content = outputStream.ToArray();
                        messages.Add($"Deployment Created Successfully");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error : {ex.Message}");
                }
            }

            return new TrakHoundCreateDeploymentResult(success, manifest, content, messages.ToArray(), errors.ToArray());
        }

        /// <summary>
        /// Create a TrakHoundDeployment from the source directory
        /// </summary>
        public async Task<TrakHoundCreateDeploymentResult> CreateAsync(string sourcePath, DateTime? incrementalTimestamp = null)
        {
            bool success = false;
            TrakHoundDeploymentManifest manifest = null;
            byte[] content = null;
            var messages = new List<string>();
            var errors = new List<string>();


            if (!string.IsNullOrEmpty(sourcePath))
            {
                if (incrementalTimestamp.HasValue) messages.Add($"Creating Incremental Deployment from Source = {sourcePath}");
                else messages.Add($"Creating Clean Deployment from Source = {sourcePath}");

                // Create Manifest
                manifest = TrakHoundDeploymentManifest.Create(sourcePath);

                var localIncrementalTimestamp = incrementalTimestamp?.ToLocalTime();

                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                        {
                            // Create Information File Entry
                            var information = archive.CreateEntry(InformationFile);
                            using (var informationStream = information.Open())
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                };

                                var json = JsonSerializer.Serialize(this, jsonOptions);
                                var bytes = Encoding.ASCII.GetBytes(json);
                                using (var inputStream = new MemoryStream(bytes))
                                {
                                    await inputStream.CopyToAsync(informationStream);
                                }

                                messages.Add($"Information File : Added");
                            }


                            // Create Manifest File Entry
                            var manifestEntry = archive.CreateEntry(ManifestFile);
                            using (var manifestStream = manifestEntry.Open())
                            {
                                var jsonOptions = new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                };

                                var json = JsonSerializer.Serialize(manifest, jsonOptions);
                                var bytes = Encoding.ASCII.GetBytes(json);
                                using (var inputStream = new MemoryStream(bytes))
                                {
                                    await inputStream.CopyToAsync(manifestStream);
                                }

                                messages.Add($"Manifest File : Added");
                            }


                            // Add Configuration Files
                            var configDir = Path.Combine(sourcePath, _configDirectory);
                            if (Directory.Exists(configDir))
                            {
                                var profileDirectories = Directory.GetDirectories(configDir);
                                if (!profileDirectories.IsNullOrEmpty())
                                {
                                    foreach (var profileDirectory in profileDirectories)
                                    {
                                        // Add Source files from Directory
                                        var files = Directory.GetFiles(profileDirectory, "*.*", SearchOption.AllDirectories);
                                        foreach (var file in files.Where(o => !o.EndsWith(FileExtension)))
                                        {
                                            var fileInfo = new FileInfo(file);
                                            if (!localIncrementalTimestamp.HasValue || fileInfo.LastWriteTime > localIncrementalTimestamp.Value)
                                            {
                                                // Get path Relative to Archive root
                                                var entry = Path.GetRelativePath(sourcePath, file);

                                                // Add Entry to Archive
                                                archive.CreateEntryFromFile(file, entry);

                                                messages.Add($"Configuration : Archive Entry Added : {entry}");
                                            }
                                        }
                                    }
                                }
                            }

                            // Add Package Files
                            var packagesDir = Path.Combine(sourcePath, _packagesDirectory);
                            if (Directory.Exists(packagesDir))
                            {
                                var categoryDirectories = Directory.GetDirectories(packagesDir);
                                foreach (var categoryDirectory in categoryDirectories)
                                {
                                    var packageDirectories = Directory.GetDirectories(categoryDirectory);
                                    foreach (var packageDirectory in packageDirectories)
                                    {
                                        var packageVersionDirectories = Directory.GetDirectories(packageDirectory);
                                        foreach (var packageVersionDirectory in packageVersionDirectories)
                                        {
                                            var directoryInfo = new DirectoryInfo(packageVersionDirectory);

                                            // Add Source files from Directory
                                            var files = Directory.GetFiles(packageVersionDirectory, "*.*", SearchOption.AllDirectories);
                                            foreach (var file in files.Where(o => !o.EndsWith(FileExtension)))
                                            {
                                                var fileInfo = new FileInfo(file);
                                                if (!localIncrementalTimestamp.HasValue || directoryInfo.LastWriteTime > localIncrementalTimestamp.Value || fileInfo.LastWriteTime > localIncrementalTimestamp.Value)
                                                {
                                                    // Get path Relative to Archive root
                                                    var entry = Path.GetRelativePath(sourcePath, file);

                                                    // Add Entry to Archive
                                                    archive.CreateEntryFromFile(file, entry);

                                                    messages.Add($"Packages : Archive Entry Added : {entry}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        success = true;
                        content = outputStream.ToArray();
                        messages.Add($"Deployment Created Successfully");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Error : {ex.Message}");
                }
            }

            return new TrakHoundCreateDeploymentResult(success, manifest, content, messages.ToArray(), errors.ToArray());
        }


        public static TrakHoundInstallDeploymentResult Install(byte[] deploymentBytes, string destinationPath = null)
        {
            bool success = false;
            var messages = new List<string>();
            var errors = new List<string>();

            if (deploymentBytes != null && deploymentBytes.Length > 0)
            {
                messages.Add("Reading Deployment Information..");

                var information = ReadInformationFromDeployment(deploymentBytes);
                var manifest = ReadManifestFromDeployment(deploymentBytes);

                if (information != null && manifest != null)
                {
                    messages.Add($"Deployment Information Read : {information.ProfileId} : v{information.Version}");
                    messages.Add($"Deployment Manfiest Read");

                    var path = destinationPath;
                    if (string.IsNullOrEmpty(path)) path = AppDomain.CurrentDomain.BaseDirectory;
                    else if (!Path.IsPathRooted(path)) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                    messages.Add($"Installing Deployment to Destination = {path}");

                    try
                    {
                        var tempPath = TrakHoundTemp.CreateDirectory();
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        messages.Add("Cleaning Existing Configurations..");

                        var configDirectory = Path.Combine(path, "config");
                        if (Directory.Exists(configDirectory) && !manifest.Configurations.IsNullOrEmpty())
                        {
                            var profileDirectories = Directory.GetDirectories(configDirectory);
                            if (!profileDirectories.IsNullOrEmpty())
                            {
                                foreach (var profileDirectory in profileDirectories)
                                {
                                    var categoryDirectories = Directory.GetDirectories(profileDirectory);
                                    if (!categoryDirectories.IsNullOrEmpty())
                                    {
                                        foreach (var categoryDirectory in categoryDirectories)
                                        {
                                            var configurationCategory = Path.GetFileName(categoryDirectory);

                                            var configurationFiles = Directory.GetFiles(categoryDirectory, "*.*");
                                            if (!configurationFiles.IsNullOrEmpty())
                                            {
                                                foreach (var configurationFile in configurationFiles)
                                                {
                                                    var configurationId = Path.GetFileNameWithoutExtension(configurationFile);
                                                    if (!manifest.Configurations.Any(o => o.Category == configurationCategory && o.Id == configurationId))
                                                    {
                                                        File.Delete(configurationFile);
                                                    }
                                                }

                                                if (Directory.GetFiles(categoryDirectory).IsNullOrEmpty()) Directory.Delete(categoryDirectory);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Files.Clear(configDirectory);
                        }


                        messages.Add("Cleaning Existing Packages..");

                        var packagesDirectory = Path.Combine(path, "packages");
                        if (Directory.Exists(packagesDirectory) && !manifest.Packages.IsNullOrEmpty())
                        {
                            var categoryDirectories = Directory.GetDirectories(packagesDirectory);
                            if (!categoryDirectories.IsNullOrEmpty())
                            {
                                foreach (var categoryDirectory in categoryDirectories)
                                {
                                    var packageCategory = Path.GetFileName(categoryDirectory);

                                    var packageDirectories = Directory.GetDirectories(categoryDirectory);
                                    if (!packageDirectories.IsNullOrEmpty())
                                    {
                                        foreach (var packageDirectory in packageDirectories)
                                        {
                                            var packageId = Path.GetFileName(packageDirectory);

                                            var packageVersionDirectories = Directory.GetDirectories(packageDirectory);
                                            if (!packageVersionDirectories.IsNullOrEmpty())
                                            {
                                                foreach (var packageVersionDirectory in packageVersionDirectories)
                                                {
                                                    var packageVersion = Path.GetFileName(packageVersionDirectory);

                                                    if (!manifest.Packages.Any(o => o.Category == packageCategory && o.Id == packageId && o.Version == packageVersion))
                                                    {
                                                        Directory.Delete(packageVersionDirectory, true);
                                                    }
                                                }

                                                if (Directory.GetDirectories(packageDirectory).IsNullOrEmpty()) Directory.Delete(packageDirectory);
                                            }
                                        }

                                        if (Directory.GetDirectories(categoryDirectory).IsNullOrEmpty()) Directory.Delete(categoryDirectory);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Files.Clear(packagesDirectory);
                        }


                        messages.Add("Installing Deployment..");

                        using (var inputStream = new MemoryStream(deploymentBytes))
                        {
                            using (var archive = new ZipArchive(inputStream, ZipArchiveMode.Read))
                            {
                                archive.ExtractToDirectory(tempPath, true);
                            }
                        }

                        messages.Add("Moving Files from Temp Directory to Destination..");

                        // Move files from temp directory (helps avoid locked files with FileSystemWatcher)
                        Files.Move(tempPath, path);

                        messages.Add("Adding Deployment Information File to Destination..");

                        var informationPath = Path.Combine(path, information.Id + InformationFileExtension);
                        if (File.Exists(informationPath))
                        {
                            var newInformationPath = Path.Combine(path, DeploymentFile);
                            File.Move(informationPath, newInformationPath, true);
                        }

                        success = true;

                        messages.Add($"Deployment Installed Successfully");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error : {ex.Message}");
                    }
                }
            }

            return new TrakHoundInstallDeploymentResult(success, messages.ToArray(), errors.ToArray());
        }


        public static string GetInstalledHash(string directory = null)
        {
            var information = ReadInformation(directory);
            if (information != null)
            {
                return information.Hash;
            }

            return null;
        }


        #region "Information"

        public static TrakHoundDeployment ReadInformation(string directory = null)
        {
            var path = directory;
            if (string.IsNullOrEmpty(path)) path = AppDomain.CurrentDomain.BaseDirectory;
            else if (!Path.IsPathRooted(path)) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            path = Path.Combine(path, DeploymentFile);

            return ReadInformationFromFile(path);
        }

        public static TrakHoundDeployment ReadInformationFromFile(string packageInformationPath)
        {
            if (!string.IsNullOrEmpty(packageInformationPath))
            {
                try
                {
                    if (File.Exists(packageInformationPath))
                    {
                        // Read JSON Information File
                        var json = File.ReadAllText(packageInformationPath);

                        // Deserialize to TrakHoundDeployment
                        return JsonSerializer.Deserialize<TrakHoundDeployment>(json);
                    }
                }
                catch { }
            }

            return null;
        }

        public static TrakHoundDeployment ReadInformationFromDeployment(string packagePath)
        {
            if (!string.IsNullOrEmpty(packagePath))
            {
                try
                {
                    if (File.Exists(packagePath))
                    {
                        using (var fileStream = new FileStream(packagePath, FileMode.Open))
                        {
                            return ReadInformationFromStream(fileStream);
                        }
                    }
                }
                catch { }
            }

            return null;
        }


        public static TrakHoundDeployment ReadInformationFromDeployment(byte[] packageBytes)
        {
            if (packageBytes != null)
            {
                try
                {
                    using (var inputStream = new MemoryStream(packageBytes))
                    {
                        return ReadInformationFromStream(inputStream);
                    }
                }
                catch { }
            }

            return null;
        }

        private static TrakHoundDeployment ReadInformationFromStream(Stream stream)
        {
            if (stream != null)
            {
                try
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        if (archive.Entries != null && archive.Entries.Count > 0)
                        {
                            foreach (var entry in archive.Entries)
                            {
                                if (entry.Name == InformationFile)
                                {
                                    using (var outputStream = new MemoryStream())
                                    {
                                        var entryStream = entry.Open();
                                        entryStream.CopyTo(outputStream);
                                        var bytes = outputStream.ToArray();
                                        var json = Encoding.UTF8.GetString(bytes);
                                        return JsonSerializer.Deserialize<TrakHoundDeployment>(json);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }


        public static TrakHoundDeploymentManifest ReadManifestFromDeployment(byte[] packageBytes)
        {
            if (packageBytes != null)
            {
                try
                {
                    using (var inputStream = new MemoryStream(packageBytes))
                    {
                        return ReadManifestFromStream(inputStream);
                    }
                }
                catch { }
            }

            return null;
        }

        private static TrakHoundDeploymentManifest ReadManifestFromStream(Stream stream)
        {
            if (stream != null)
            {
                try
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        if (archive.Entries != null && archive.Entries.Count > 0)
                        {
                            foreach (var entry in archive.Entries)
                            {
                                if (entry.Name == ManifestFile)
                                {
                                    using (var outputStream = new MemoryStream())
                                    {
                                        var entryStream = entry.Open();
                                        entryStream.CopyTo(outputStream);
                                        var bytes = outputStream.ToArray();
                                        var json = Encoding.UTF8.GetString(bytes);
                                        return JsonSerializer.Deserialize<TrakHoundDeploymentManifest>(json);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion

    }
}
