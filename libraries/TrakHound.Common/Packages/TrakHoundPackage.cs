// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace TrakHound.Packages
{
    public sealed class TrakHoundPackage
    {
        public const string FileExtension = ".thp";
        public const string InformationFilename = "package.json";
        public const string PackageDirectory = "packages";
        public const string DistributableDirectory = "dist";
        public const string SourceDirectory = "src";
        public const string PackageConfigurationFilename = "trakhound.package.json";
        public const string WilcardVersion = "*";

        public const string ReadMeFilename = "Readme";
        public const string LicenseFilename = "License";
        public static readonly IEnumerable<string> ReadMeFileExtensions = new string[] { ".md", ".txt" };

        public const string DescriptionName = "description";
        public const string PublisherName = "publisher";
        public const string ProjectName = "project";
        public const string RepositoryName = "repository";
        public const string RepositoryDirectoryName = "repositoryDirectory";
        public const string RepositoryBranchName = "repositoryBranch";
        public const string RepositoryCommitName = "repositoryCommit";
        public const string TrakHoundVersionName = "trakhoundVersion";
        public const string ImageName = ".image";


        public string Uuid => GenerateUuid(this);

        public string Id { get; }

        public string Version { get; }

        public string Category { get; }

        public DateTime BuildDate { get; }

        public string Location { get; internal set; }

        public string Hash => GenerateHash(this);

        public Dictionary<string, object> Metadata { get; }

        public IEnumerable<ITrakHoundPackageDependency> Dependencies { get; }


        internal TrakHoundPackage(TrakHoundPackageBuilder builder)
        {
            if (builder != null)
            {
                Category = builder.Category;
                Id = builder.Id;
                Version = builder.Version;
                Location = builder.Location;
                BuildDate = builder.BuildDate;
                Metadata = builder.Metadata?.OrderBy(o => o.Key).ToDictionary();
                Dependencies = builder.Dependencies;
            }
        }

        public override string ToString()
        {
            return $"{Category} : {Id} : v{Version}";
        }

        public static string GenerateUuid(TrakHoundPackage package)
        {
            if (package != null)
            {
                return GenerateUuid(package.Id, package.Version);
            }

            return null;
        }

        public static string GenerateUuid(string packageId, string packageVersion)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(packageVersion))
            {
                return $"{packageId}:{packageVersion}".ToMD5Hash();
            }

            return null;
        }

        public static string GenerateHash(TrakHoundPackage package)
        {
            if (package != null)
            {
                return $"{package.Category}:{package.Id}:{package.Version}:{package.BuildDate.ToUnixTime()}".ToMD5Hash();
            }

            return null;
        }


        public bool MetadataExists(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Metadata.IsNullOrEmpty())
            {
                return Metadata.ContainsKey(name);
            }

            return false;
        }

        public string GetMetadata(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Metadata.IsNullOrEmpty())
            {
                var value = Metadata.GetValueOrDefault(name);
                if (value != null)
                {
                    return value.ToString();
                }
            }

            return null;
        }

        public T GetMetadata<T>(string name)
        {
            if (!string.IsNullOrEmpty(name) && !Metadata.IsNullOrEmpty())
            {
                var value = Metadata.GetValueOrDefault(name);
                if (value != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch { }
                }
            }

            return default;
        }


        /// <summary>
        /// Create a TrakHoundPackage from the source directory
        /// </summary>
        public byte[] Package(string sourcePath)
        {
            if (!string.IsNullOrEmpty(sourcePath))
            {
                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                        {
                            // Create Information File Entry
                            var information = archive.CreateEntry(InformationFilename);
                            using (var informationStream = information.Open())
                            {
                                var jsonPackage = new TrakHoundPackageJson(this);
                                jsonPackage.BuildDate = DateTime.UtcNow;

                                var jsonOptions = new JsonSerializerOptions
                                {
                                    WriteIndented = true
                                };

                                var json = JsonSerializer.Serialize(jsonPackage, jsonOptions);
                                var bytes = Encoding.ASCII.GetBytes(json);
                                using (var inputStream = new MemoryStream(bytes))
                                {
                                    inputStream.CopyTo(informationStream);
                                }
                            }

                            // Add Source files from Directory
                            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                            foreach (var file in files.Where(o => !o.EndsWith(FileExtension)))
                            {
                                // Get path Relative to Archive root
                                var entry = Path.GetRelativePath(sourcePath, file);

                                // Add Entry to Archive
                                archive.CreateEntryFromFile(file, entry);
                            }
                        }

                        return outputStream.ToArray();
                    }
                }
                catch { }
            }

            return null;
        }


        #region "Directories"

        public static string GetDirectory(string path = null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return Path.Combine(path, PackageDirectory);
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PackageDirectory);
            }
        }


        public static string GetCategoryDirectory(string category)
        {
            return Path.Combine(GetDirectory(), category);
        }

        public static string GetCategoryDirectory(string installPath, string category)
        {
            return Path.Combine(GetDirectory(installPath), category);
        }


        public static string GetPackageDirectory(string category, string packageId)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(packageId))
            {
                return Path.Combine(GetCategoryDirectory(category), packageId);
            }

            return null;
        }

        public static string GetPackageDirectory(string installPath, string category, string packageId)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(packageId))
            {
                return Path.Combine(GetCategoryDirectory(installPath, category), packageId);
            }

            return null;
        }

        public static string GetPackageVersionDirectory(string category, string packageId, string version)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(version))
            {
                if (version.Contains('*'))
                {
                    try
                    {
                        var matchedFiles = Directory.GetDirectories(Path.Combine(GetCategoryDirectory(category), packageId), version);
                        if (!matchedFiles.IsNullOrEmpty())
                        {
                            return matchedFiles.OrderByDescending(o => Path.GetFileName(o).ToVersion()).FirstOrDefault();
                        }
                    }
                    catch { }
                }
                else
                {
                    return Path.Combine(GetCategoryDirectory(category), packageId, version);
                }
            }

            return null;
        }

        public static string GetPackageVersionDirectory(string installPath, string category, string packageId, string version)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(version))
            {
                if (version.Contains('*'))
                {
                    try
                    {
                        var matchedFiles = Directory.GetDirectories(Path.Combine(GetCategoryDirectory(installPath, category), packageId), version);
                        if (!matchedFiles.IsNullOrEmpty())
                        {
                            return matchedFiles.OrderByDescending(o => Path.GetFileName(o).ToVersion()).FirstOrDefault();
                        }
                    }
                    catch { }
                }
                else
                {
                    return Path.Combine(GetCategoryDirectory(installPath, category), packageId, version);
                }
            }

            return null;
        }


        public static string GetLatestPackageDirectory(string category, string packageId)
        {
            var dir = GetCategoryDirectory(category);
            dir = Path.Combine(dir, packageId);
            if (Directory.Exists(dir))
            {
                var versionDirectories = Directory.GetDirectories(dir);
                if (versionDirectories != null && versionDirectories.Length > 0)
                {
                    var versions = new List<VersionDirectory>();
                    foreach (var versionDirectory in versionDirectories)
                    {
                        var directoryName = Path.GetFileName(versionDirectory);
                        if (System.Version.TryParse(directoryName, out var version))
                        {
                            versions.Add(new VersionDirectory(version, versionDirectory));
                        }
                    }

                    // Get the latest version
                    return versions.OrderByDescending(o => o.Version).FirstOrDefault().Path;
                }
            }

            return null;
        }

        public static string GetLatestPackageDirectory(string installPath, string category, string packageId)
        {
            var dir = GetCategoryDirectory(installPath, category);
            dir = Path.Combine(dir, packageId);
            if (Directory.Exists(dir))
            {
                var versionDirectories = Directory.GetDirectories(dir);
                if (versionDirectories != null && versionDirectories.Length > 0)
                {
                    var versions = new List<VersionDirectory>();
                    foreach (var versionDirectory in versionDirectories)
                    {
                        var directoryName = Path.GetFileName(versionDirectory);
                        if (System.Version.TryParse(directoryName, out var version))
                        {
                            versions.Add(new VersionDirectory(version, versionDirectory));
                        }
                    }

                    // Get the latest version
                    return versions.OrderByDescending(o => o.Version).FirstOrDefault().Path;
                }
            }

            return null;
        }



        public static string GetPackageVersionDistributableDirectory(string category, string packageId, string version)
            => GetSubDirectory(GetPackageVersionDirectory(category, packageId, version), DistributableDirectory);

        public static string GetPackageVersionDistributableDirectory(string installPath, string category, string packageId, string version) 
            => GetSubDirectory(GetPackageVersionDirectory(installPath, category, packageId, version), DistributableDirectory);

        public static string GetLatestPackageDistributableDirectory(string category, string packageId) 
            => GetSubDirectory(GetLatestPackageDirectory(category, packageId), DistributableDirectory);

        public static string GetLatestPackageDistributableDirectory(string installPath, string category, string packageId)
            => GetSubDirectory(GetLatestPackageDirectory(installPath, category, packageId), DistributableDirectory);



        public static string GetPackageVersionSourceDirectory(string category, string packageId, string version)
            => GetSubDirectory(GetPackageVersionSourceDirectory(category, packageId, version), SourceDirectory);

        public static string GetPackageVersionSourceDirectory(string installPath, string category, string packageId, string version)
            => GetSubDirectory(GetPackageVersionDirectory(installPath, category, packageId, version), SourceDirectory);

        public static string GetLatestPackageSourceDirectory(string category, string packageId)
            => GetSubDirectory(GetLatestPackageDirectory(category, packageId), SourceDirectory);

        public static string GetLatestPackageSourceDirectory(string installPath, string category, string packageId)
            => GetSubDirectory(GetLatestPackageDirectory(installPath, category, packageId), SourceDirectory);


        private static string GetSubDirectory(string path, string subDirectoryName)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(subDirectoryName))
            {
                return Path.Combine(path, subDirectoryName);
            }

            return null;
        }

        #endregion

        #region "Information"

        public static TrakHoundPackage ReadInformation(string category, string packageId, string packageVersion = null, string installPath = null)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                try
                {
                    string packageDirectory;

                    if (!string.IsNullOrEmpty(packageVersion))
                    {
                        if (!string.IsNullOrEmpty(installPath)) packageDirectory = GetPackageVersionDirectory(installPath, category, packageId, packageVersion);
                        else packageDirectory = GetPackageVersionDirectory(category, packageId, packageVersion);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(installPath)) packageDirectory = GetLatestPackageDirectory(installPath, category, packageId);
                        else packageDirectory = GetLatestPackageDirectory(category, packageId);
                    }

                    if (Directory.Exists(packageDirectory))
                    {
                        var informationPath = Path.Combine(packageDirectory, InformationFilename);
                        if (File.Exists(informationPath))
                        {
                            // Read JSON Information File
                            var json = File.ReadAllText(informationPath);

                            // Deserialize to TrakHoundPackage
                            var jsonPackage = JsonSerializer.Deserialize<TrakHoundPackageJson>(json);
                            if (jsonPackage != null)
                            {
                                var package = jsonPackage.ToPackage();
                                if (package != null)
                                {
                                    package.Location = packageDirectory;

                                    return package;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static TrakHoundPackage ReadInformation(string packageDirectory)
        {
            if (!string.IsNullOrEmpty(packageDirectory))
            {
                try
                {
                    if (Directory.Exists(packageDirectory))
                    {
                        // Get Package ID
                        var packageId = Path.GetFileName(Path.GetDirectoryName(packageDirectory));

                        var informationPath = Path.Combine(packageDirectory, InformationFilename);
                        if (File.Exists(informationPath))
                        {
                            // Read JSON Information File
                            var json = File.ReadAllText(informationPath);

                            // Deserialize to TrakHoundPackage
                            var jsonPackage = JsonSerializer.Deserialize<TrakHoundPackageJson>(json);
                            if (jsonPackage != null)
                            {
                                var package = jsonPackage.ToPackage();
                                if (package != null)
                                {
                                    package.Location = packageDirectory;

                                    return package;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static TrakHoundPackage ReadInformationFromFile(string packageInformationPath)
        {
            if (!string.IsNullOrEmpty(packageInformationPath))
            {
                try
                {
                    if (File.Exists(packageInformationPath))
                    {
                        // Read JSON Information File
                        var json = File.ReadAllText(packageInformationPath);

                        // Deserialize to TrakHoundPackage
                        var jsonPackage = JsonSerializer.Deserialize<TrakHoundPackageJson>(json);
                        if (jsonPackage != null)
                        {
                            var package = jsonPackage.ToPackage();
                            if (package != null)
                            {
                                package.Location = Path.GetDirectoryName(packageInformationPath);

                                return package;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static TrakHoundPackage ReadInformationFromPackage(string packagePath)
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

        public static TrakHoundPackage ReadInformationFromPackage(byte[] packageBytes)
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

        private static TrakHoundPackage ReadInformationFromStream(Stream stream)
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
                                if (entry.Name == InformationFilename)
                                {
                                    using (var outputStream = new MemoryStream())
                                    {
                                        var entryStream = entry.Open();
                                        entryStream.CopyTo(outputStream);
                                        var bytes = outputStream.ToArray();
                                        var json = Encoding.ASCII.GetString(bytes);

                                        // Deserialize to TrakHoundPackage
                                        var jsonPackage = JsonSerializer.Deserialize<TrakHoundPackageJson>(json);
                                        if (jsonPackage != null) return jsonPackage.ToPackage();
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

        #region "Get"

        public static IEnumerable<TrakHoundPackage> GetInstalled(string installPath = null)
        {
            var installedPackages = new List<TrakHoundPackage>();

            var packagePath = GetDirectory(installPath);
            if (!string.IsNullOrEmpty(packagePath))
            {
                try
                {
                    if (Directory.Exists(packagePath))
                    {
                        // Read Categories
                        var categoryDirectories = Directory.GetDirectories(packagePath);
                        if (categoryDirectories != null && categoryDirectories.Length > 0)
                        {
                            foreach (var categoryDirectory in categoryDirectories)
                            {
                                // Read Packages (Package ID)
                                var packageDirectories = Directory.GetDirectories(categoryDirectory);
                                if (packageDirectories != null && packageDirectories.Length > 0)
                                {
                                    foreach (var packageDirectory in packageDirectories)
                                    {
                                        var packageId = Path.GetFileName(packageDirectory);

                                        // Read Versions
                                        var versionDirectories = Directory.GetDirectories(packageDirectory);
                                        if (versionDirectories != null && versionDirectories.Length > 0)
                                        {
                                            foreach (var versionDirectory in versionDirectories)
                                            {
                                                // Read Package Information
                                                var packageInformation = ReadInformation(versionDirectory);
                                                if (packageInformation != null)
                                                {
                                                    installedPackages.Add(packageInformation);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        } 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return installedPackages;
        }

        public static IEnumerable<string> GetInstalledIds()
        {
            var installedPackageIds = new List<string>();

            var packagePath = GetDirectory();
            if (!string.IsNullOrEmpty(packagePath))
            {
                try
                {
                    if (Directory.Exists(packagePath))
                    {
                        // Read Categories
                        var categoryDirectories = Directory.GetDirectories(packagePath);
                        if (categoryDirectories != null && categoryDirectories.Length > 0)
                        {
                            foreach (var categoryDirectory in categoryDirectories)
                            {
                                // Read Packages (Package ID)
                                var packageDirectories = Directory.GetDirectories(categoryDirectory);
                                if (packageDirectories != null && packageDirectories.Length > 0)
                                {
                                    foreach (var packageDirectory in packageDirectories)
                                    {
                                        // Read Package ID
                                        var packageId = Path.GetFileName(packageDirectory);

                                        installedPackageIds.Add(packageId);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return installedPackageIds;
        }

        public static string GetLatestVersion(string category, string packageId)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(packageId))
            {
                var dir = GetLatestPackageDirectory(category, packageId);
                if (dir != null)
                {
                    var info = ReadInformation(dir);
                    if (info != null)
                    {
                        return info.Version;
                    }
                }
            }

            return null;
        }

        public static IEnumerable<TrakHoundPackage> GetInstalledVersions(string category, string packageId)
        {
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(packageId))
            {
                var dir = GetPackageDirectory(category, packageId);
                if (dir != null)
                {
                    var versionDirs = Directory.GetDirectories(dir);
                    if (!versionDirs.IsNullOrEmpty())
                    {
                        var packages = new List<TrakHoundPackage>();

                        foreach (var versionDir in versionDirs)
                        {
                            var package = ReadInformation(versionDir);
                            if (package != null) packages.Add(package);
                        }

                        return packages;
                    }
                }
            }

            return null;
        }

        public static string GetReadMe(string category, string packageId, string packageVersion = null, string installPath = null)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                try
                {
                    string packageDirectory;
                    if (!string.IsNullOrEmpty(installPath)) packageDirectory = GetPackageVersionDistributableDirectory(installPath, category, packageId, packageVersion);
                    else packageDirectory = GetPackageVersionDistributableDirectory(category, packageId, packageVersion);

                    if (Directory.Exists(packageDirectory))
                    {
                        foreach (var fileExtension in ReadMeFileExtensions)
                        {
                            // Set Filename
                            var filename = ReadMeFilename + fileExtension;

                            var filePath = Path.Combine(packageDirectory, filename);
                            if (File.Exists(filePath))
                            {
                                // Read JSON Information File
                                var content = File.ReadAllText(filePath);
                                if (!string.IsNullOrEmpty(content)) return content;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        public static string GetLicense(string category, string packageId, string packageVersion = null, string installPath = null)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                try
                {
                    string packageDirectory;
                    if (!string.IsNullOrEmpty(installPath)) packageDirectory = GetPackageVersionDistributableDirectory(installPath, category, packageId, packageVersion);
                    else packageDirectory = GetPackageVersionDistributableDirectory(category, packageId, packageVersion);

                    if (Directory.Exists(packageDirectory))
                    {
                        var filePath = Path.Combine(packageDirectory, LicenseFilename);
                        if (File.Exists(filePath))
                        {
                            // Read License Text File
                            var content = File.ReadAllText(filePath);
                            if (!string.IsNullOrEmpty(content)) return content;
                        }
                    }
                }
                catch { }
            }

            return null;
        }

        #endregion

        #region "Install / Uninstall"

        public static bool Install(byte[] packageBytes, string installPath = null)
        {
            if (packageBytes != null && packageBytes.Length > 0)
            {
                var information = ReadInformationFromPackage(packageBytes);
                if (information != null)
                {
                    var packageDirectory = GetDirectory(installPath);
                    var packagePath = Path.Combine(packageDirectory, information.Category, information.Id, information.Version);

                    try
                    {
                        if (!Directory.Exists(packagePath)) Directory.CreateDirectory(packagePath);

                        using (var inputStream = new MemoryStream(packageBytes))
                        {
                            using (var archive = new ZipArchive(inputStream, ZipArchiveMode.Read))
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    if (entry.FullName != InformationFilename)
                                    {
                                        var entryPath = Path.Combine(packagePath, entry.FullName);

                                        if (Path.DirectorySeparatorChar == '/') entryPath = entryPath.Replace('\\', '/');
                                        else entryPath = entryPath.Replace('/', '\\');

                                        var directoryPath = Path.GetDirectoryName(entryPath);
                                        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                                        entry.ExtractToFile(entryPath, true);
                                    }
                                }
                            }
                        }

                        // Write Information file
                        // Note: Do this last so that all files will be copied before notifying the FileSystemWatcher
                        var jsonInformation = new TrakHoundPackageJson(information);
                        Json.Write(jsonInformation, Path.Combine(packagePath, InformationFilename));

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return false;
        }

        public static bool Uninstall(string packageId, string packageVersion = null, string installPath = null)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                try
                {
                    var packagesInformation = TrakHoundPackagesFile.Read(installPath);
                    if (packagesInformation != null)
                    {
                        TrakHoundPackagesFileItem packageInformation;

                        if (!string.IsNullOrEmpty(packageVersion)) packageInformation = packagesInformation.Packages.FirstOrDefault(o => o.Id.ToLower() == packageId.ToLower() && StringFunctions.MatchVersion(packageVersion, o.Version));
                        else packageInformation = packagesInformation.Packages.FirstOrDefault(o => o.Id.ToLower() == packageId.ToLower());

                        if (packageInformation != null)
                        {
                            var packageDirectory = GetPackageDirectory(installPath, packageInformation.Category, packageId);
                            if (!string.IsNullOrEmpty(packageDirectory))
                            {
                                if (Directory.Exists(packageDirectory))
                                {
                                    if (!string.IsNullOrEmpty(packageVersion))
                                    {
                                        var versionDirectory = GetPackageVersionDirectory(installPath, packageInformation.Category, packageId, packageVersion);
                                        return RemovePackageVersion(versionDirectory);
                                    }
                                    else
                                    {
                                        return RemovePackage(packageDirectory);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            return false;
        }

        public static bool RemovePackageVersion(string packageVersionDirectory)
        {
            if (!string.IsNullOrEmpty(packageVersionDirectory))
            {
                try
                {
                    if (Directory.Exists(packageVersionDirectory))
                    {
                        // Delete the Package Directory for the specified Version
                        Directory.Delete(packageVersionDirectory, true);

                        // Get Parent Directory (Package Directory)
                        var packageDirectory = Path.GetDirectoryName(packageVersionDirectory);

                        // Check if Package Directory is empty
                        var remainingDirectories = Directory.GetDirectories(packageDirectory);
                        if (remainingDirectories == null || remainingDirectories.Length < 1)
                        {
                            // Delete the empty Package Directory
                            Directory.Delete(packageDirectory, true);
                        }

                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        public static bool RemovePackage(string packageDirectory)
        {
            if (!string.IsNullOrEmpty(packageDirectory))
            {
                try
                {
                    if (Directory.Exists(packageDirectory))
                    {
                        // Delete the entire Package Directory
                        Directory.Delete(packageDirectory, true);
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }

        #endregion

    }
}
