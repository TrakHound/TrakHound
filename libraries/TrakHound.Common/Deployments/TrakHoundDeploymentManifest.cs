// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace TrakHound.Deployments
{
    public class TrakHoundDeploymentManifest
    {
        private const string _configDirectory = "config";
        private const string _packagesDirectory = "packages";


        [JsonPropertyName("configurations")]
        public IEnumerable<TrakHoundDeploymentConfigurationManifestItem> Configurations { get; set; }

        [JsonPropertyName("packages")]
        public IEnumerable<TrakHoundDeploymentPackageManifestItem> Packages { get; set; }



        public static TrakHoundDeploymentManifest Create(string sourcePath)
        {
            var manifestConfigurations = new List<TrakHoundDeploymentConfigurationManifestItem>();
            var manifestPackages = new List<TrakHoundDeploymentPackageManifestItem>();

            if (!string.IsNullOrEmpty(sourcePath))
            {
                try
                {
                    // Add Configuration Files
                    var configDir = Path.Combine(sourcePath, _configDirectory);
                    if (Directory.Exists(configDir))
                    {
                        var profileDirectories = Directory.GetDirectories(configDir);
                        if (!profileDirectories.IsNullOrEmpty())
                        {
                            foreach (var profileDirectory in profileDirectories)
                            {
                                var files = Directory.GetFiles(profileDirectory, "*.*", SearchOption.AllDirectories);
                                foreach (var file in files)
                                {
                                    var fileInfo = new FileInfo(file);

                                    // Add to Manifest
                                    var manifestItem = new TrakHoundDeploymentConfigurationManifestItem();
                                    manifestItem.Category = Path.GetFileName(Path.GetDirectoryName(file));
                                    manifestItem.Id = Path.GetFileNameWithoutExtension(file);
                                    manifestItem.LastUpdated = fileInfo.LastWriteTime.ToUnixTime();
                                    manifestConfigurations.Add(manifestItem);
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
                                    // Add to Manifest
                                    var manifestItem = new TrakHoundDeploymentPackageManifestItem();
                                    manifestItem.Category = Path.GetFileName(categoryDirectory);
                                    manifestItem.Id = Path.GetFileName(packageDirectory);
                                    manifestItem.Version = Path.GetFileName(packageVersionDirectory);
                                    manifestPackages.Add(manifestItem);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            var manifest = new TrakHoundDeploymentManifest();
            manifest.Configurations = manifestConfigurations;
            manifest.Packages = manifestPackages;
            return manifest;
        }



        public static TrakHoundDeploymentManifest Create(TrakHoundDeploymentManifest baseManifest, TrakHoundDeploymentManifest compareManifest)
        {
            if (baseManifest != null && compareManifest != null)
            {
                var outputConfigurations = new List<TrakHoundDeploymentConfigurationManifestItem>();

                if (!baseManifest.Configurations.IsNullOrEmpty())
                {
                    if (!compareManifest.Configurations.IsNullOrEmpty())
                    {
                        foreach (var item in compareManifest.Configurations)
                        {
                            var baseItem = baseManifest.Configurations.FirstOrDefault(o => o.Category == item.Category && o.Id == item.Id);
                            if (baseItem != null)
                            {
                                if (item.LastUpdated > baseItem.LastUpdated)
                                {
                                    var outputItem = new TrakHoundDeploymentConfigurationManifestItem();
                                    //outputItem.ChangeType = TrakHoundDeploymentChangeType.Modified;
                                    outputItem.Category = item.Category;
                                    outputItem.Id = item.Id;
                                    outputItem.LastUpdated = item.LastUpdated;
                                    outputConfigurations.Add(outputItem);
                                }
                            }
                            else
                            {
                                var outputItem = new TrakHoundDeploymentConfigurationManifestItem();
                                //outputItem.ChangeType = TrakHoundDeploymentChangeType.Added;
                                outputItem.Category = item.Category;
                                outputItem.Id = item.Id;
                                outputItem.LastUpdated = item.LastUpdated;
                                outputConfigurations.Add(outputItem);
                            }
                        }

                        // Detect Deleted Configurations
                        foreach (var item in baseManifest.Configurations)
                        {
                            if (!compareManifest.Configurations.Any(o => o.Category == item.Category && o.Id == item.Id))
                            {
                                var outputItem = new TrakHoundDeploymentConfigurationManifestItem();
                                //outputItem.ChangeType = TrakHoundDeploymentChangeType.Removed;
                                outputItem.Category = item.Category;
                                outputItem.Id = item.Id;
                                outputItem.LastUpdated = item.LastUpdated;
                                outputConfigurations.Add(outputItem);
                            }
                        }
                    }
                    else
                    {
                        outputConfigurations.AddRange(baseManifest.Configurations);
                    }
                }
                else if (!compareManifest.Configurations.IsNullOrEmpty())
                {
                    outputConfigurations.AddRange(compareManifest.Configurations);
                }

                var outputManifest = new TrakHoundDeploymentManifest();
                outputManifest.Configurations = outputConfigurations;
                return outputManifest;
            }

            return null;
        }
    }
}
