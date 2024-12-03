// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace TrakHound.Packages
{
    public class TrakHoundDotNetPackage
    {
        private static IEnumerable<string> _sourceIgnorePatterns = new string[]
        {
            "[Bb]in\\\\",
            "[Oo]bj\\\\",
            "\\.vs"
        };


        public static byte[] Create(string projectPath, TrakHoundDotNetPackageSettings settings)
        {
            if (!string.IsNullOrEmpty(projectPath))
            {
                try
                {
                    var path = projectPath;
                    if (!Path.IsPathRooted(path)) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);                  

                    if (Path.HasExtension(path) && Path.GetExtension(path) == ".csproj")
                    {
                        if (File.Exists(projectPath))
                        {
                            return CreateFromProjectFile(projectPath, settings);
                        }
                    }
                    else
                    {
                        if (Directory.Exists(path))
                        {
                            var information = TrakHoundPackage.ReadInformationFromFile(Path.Combine(path, TrakHoundPackage.PackageConfigurationFilename));
                            if (information != null)
                            {
                                var projectFile = Path.Combine(path, $"{information.Id}.csproj");
                                projectFile = Path.GetRelativePath(Environment.CurrentDirectory, projectFile);

                                return CreateFromProjectFile(projectFile, settings);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("TrakHoundDotNetPackage.Create() : " + ex.Message);
                }
            }

            return null;
        }

        private static byte[] CreateFromProjectFile(string projectFilePath, TrakHoundDotNetPackageSettings settings)
        {
            if (!string.IsNullOrEmpty(projectFilePath))
            {
                try
                {
                    TrakHoundTemp.Clear();

                    byte[] packageBytes = null;

                    var projectOutputDir = TrakHoundTemp.CreateDirectory();
                    var projectDir = Path.GetDirectoryName(projectFilePath);
                    var projectFilename = Path.GetFileName(projectFilePath);

                    var tempProjectFilePath = Path.ChangeExtension(projectFilePath, ".tmp");
                    File.Move(projectFilePath, tempProjectFilePath);

                    var packageConfiguration = TrakHoundPackage.ReadInformationFromFile(Path.Combine(projectDir, TrakHoundPackage.PackageConfigurationFilename));
                    if (packageConfiguration != null)
                    {
                        var packageBuilder = new TrakHoundPackageBuilder(packageConfiguration);

                        // Override Asembly/Package Version
                        if (!string.IsNullOrEmpty(settings.Version)) packageBuilder.Version = settings.Version;

                        // Set TrakHound Version (read TrakHound.Common Assembly Version)
                        var trakHoundVersion = GetTrakHoundVersion(projectDir);
                        if (!string.IsNullOrEmpty(trakHoundVersion))
                        {
                            packageBuilder.Metadata.Add(TrakHoundPackage.TrakHoundVersionName, trakHoundVersion);
                        }

                        // Add Git Branch and Commit SHA
                        var repository = packageConfiguration.GetMetadata(TrakHoundPackage.RepositoryName);
                        if (!string.IsNullOrEmpty(repository))
                        {
                            if (!string.IsNullOrEmpty(settings.GitBranch)) packageBuilder.Metadata.Add(TrakHoundPackage.RepositoryBranchName, settings.GitBranch);
                            if (!string.IsNullOrEmpty(settings.GitCommit)) packageBuilder.Metadata.Add(TrakHoundPackage.RepositoryCommitName, settings.GitCommit);
                        }

                        //var newProjectTempPath = Path.Combine(projectDir, $"{packageConfiguration.Id}-{packageConfiguration.Version}.tmp");
                        var newProjectTempPath = Path.Combine(projectDir, $"{packageBuilder.Id}-{packageBuilder.Version}.csproj");

                        // Copy and Convert Project file to add category specific configuration
                        ConvertProjectFile(trakHoundVersion, packageBuilder.Category, packageBuilder.Id, tempProjectFilePath, newProjectTempPath);

                        // Clear out the 'bin' and 'obj' directories
                        // -- This mainly caused issues with Blazor/ Razor components using scoped css.For some reason this resets the scope ID in the files.
                        // -- Try twice since if Visual Studio is running, it tries to rebuild the 'obj' directory
                        try
                        {
                            if (Directory.Exists(Path.Combine(projectDir, "obj"))) Directory.Delete(Path.Combine(projectDir, "obj"), true);
                        }
                        catch { }
                        Thread.Sleep(3000);
                        try
                        {
                            if (Directory.Exists(Path.Combine(projectDir, "obj"))) Directory.Delete(Path.Combine(projectDir, "obj"), true);
                        }
                        catch { }

                        // Restore New Project
                        if (!RestoreProject(settings, projectFilePath))
                        //if (!RestoreProject(newProjectTempPath))
                        {
                            Console.WriteLine("Restore Error");
                        }

                        //// Build New Project
                        //if (!BuildProject(settings, projectFilePath))
                        ////if (!BuildProject(settings, newProjectTempPath))
                        //{
                        //    Console.WriteLine("Build Error");
                        //}

                        //// Set TrakHound Version (read TrakHound.Common Assembly Version)
                        //var trakHoundVersion = GetTrakHoundVersion(projectDir);
                        //if (!string.IsNullOrEmpty(trakHoundVersion))
                        //{
                        //    packageBuilder.Metadata.Add(TrakHoundPackage.TrakHoundVersionName, trakHoundVersion);
                        //}

                        var package = packageBuilder.Build();

                        if (PublishProject(package, settings, newProjectTempPath, projectOutputDir))
                        {
                            var packageTempDir = TrakHoundTemp.CreateDirectory();

                            // Copy Built files to "dist" directory
                            var distDir = Path.Combine(packageTempDir, TrakHoundPackage.DistributableDirectory);
                            Directory.CreateDirectory(distDir);
                            Files.Copy(projectOutputDir, distDir);

                            // Exclude "_content" because csproj ExcludeAssets isn't working
                            if (Directory.Exists(Path.Combine(distDir, "wwwroot/_content/TrakHound.Blazor")))
                            {
                                var trakhoundBlazorDir = Directory.GetDirectories(distDir, "wwwroot/_content/TrakHound.Blazor")?.FirstOrDefault();
                                if (trakhoundBlazorDir != null) Directory.Delete(trakhoundBlazorDir, true);
                            }
                            if (Directory.Exists(Path.Combine(distDir, "wwwroot/_content/Radzen.Blazor")))
                            {
                                var radzenBlazorDir = Directory.GetDirectories(distDir, "wwwroot/_content/Radzen.Blazor")?.FirstOrDefault();
                                if (radzenBlazorDir != null) Directory.Delete(radzenBlazorDir, true);
                            }

                            if (settings.IncludeSource)
                            {
                                // Copy Source files and compress to "src.zip
                                var srcZipTemp = TrakHoundTemp.CreateDirectory();
                                Files.Copy(projectDir, srcZipTemp, _sourceIgnorePatterns);
                                Files.Zip(srcZipTemp, Path.Combine(packageTempDir, $"{TrakHoundPackage.SourceDirectory}.zip"));
                                Directory.Delete(srcZipTemp, true);
                            }


                            //var package = CreatePackageFromProject(packageConfiguration.Category, projectTempPath);

                            //if (packageConfiguration != null)
                            //{
                            //    // Override Package ID
                            //    if (!string.IsNullOrEmpty(packageConfiguration.Id)) package.Id = packageConfiguration.Id;

                            //    // Override Package Version
                            //    if (!string.IsNullOrEmpty(packageConfiguration.Version)) package.Version = packageConfiguration.Version;

                            //    // Override Description
                            //    if (packageConfiguration.Metadata.ContainsKey("description")) package.Metadata["description"] = packageConfiguration.Metadata["description"];

                            //    // Override Publisher
                            //    if (packageConfiguration.Metadata.ContainsKey("publisher")) package.Metadata["publisher"] = packageConfiguration.Metadata["publisher"];

                            //    // Set Package Dependencies
                            //    if (!packageConfiguration.Dependencies.IsNullOrEmpty())
                            //    {
                            //        package.Dependencies = packageConfiguration.Dependencies;
                            //    }
                            //}


                            packageBytes = package.Package(packageTempDir);

                            Directory.Delete(packageTempDir, true);
                        }

                        File.Delete(newProjectTempPath);
                        Directory.Delete(projectOutputDir, true);

                        File.Move(tempProjectFilePath, projectFilePath);

                        TrakHoundTemp.Clear();

                        // Restore original Project (helps with working in Visual Studio)
                        if (!RestoreProject(settings, projectFilePath))
                        {
                            Console.WriteLine("Restore Error");
                        }

                        return packageBytes;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("TrakHoundDotNetPackage.CreateFromProjectFile() : " + projectFilePath + " : " + ex.Message + " : " + ex.StackTrace);
                }
            }

            return null;
        }

        private static TrakHoundPackage CreatePackageFromProject(string packageCategory, string projectFilePath)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(projectFilePath);

            var doc = XDocument.Load(projectFilePath);

            // Get Assembly Name
            var projectAssemblyName = doc.Descendants("AssemblyName")?.FirstOrDefault()?.Value;

            // Get Package Version
            var projectVersion = doc.Descendants("PackageVersion")?.FirstOrDefault()?.Value;

            // Get Description
            var projectDescription = doc.Descendants("Description")?.FirstOrDefault()?.Value;

            var packageBuilder = new TrakHoundPackageBuilder();
            packageBuilder.Category = packageCategory;
            packageBuilder.Id = !string.IsNullOrEmpty(projectAssemblyName) ? projectAssemblyName : assemblyName;
            packageBuilder.Version = !string.IsNullOrEmpty(projectVersion) ? projectVersion : "1.0.0";
            packageBuilder.BuildDate = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(projectDescription)) packageBuilder.Metadata["description"] = projectDescription;

            return packageBuilder.Build();
        }

        private static bool CleanProject(TrakHoundPackage packageConfiguration, TrakHoundDotNetPackageSettings settings, string projectFilePath, string outputPath)
        {
            if (packageConfiguration != null && settings != null && !string.IsNullOrEmpty(settings.Configuration) && !string.IsNullOrEmpty(projectFilePath))
            {
                var cmd = $"dotnet clean \"{projectFilePath}\" ";
                var arguments = new List<string>();
                arguments.Add($"-c:{settings.Configuration}");
                cmd += string.Join(' ', arguments);

                return RunCommand(projectFilePath, cmd);
            }

            return false;
        }

        private static bool RestoreProject(TrakHoundDotNetPackageSettings settings, string projectFilePath)
        {
            if (!string.IsNullOrEmpty(projectFilePath))
            {
                var cmd = $"dotnet restore";
                if (settings != null && !string.IsNullOrEmpty(settings.DotnetSdkLocation)) cmd = Path.Combine(settings.DotnetSdkLocation, cmd);

                return RunCommand(cmd);
            }

            return false;
        }

        private static bool BuildProject(TrakHoundDotNetPackageSettings settings, string projectFilePath)
        {
            if (settings != null && !string.IsNullOrEmpty(settings.Configuration) && !string.IsNullOrEmpty(projectFilePath))
            {
                var cmd = "dotnet build ";
                var arguments = new List<string>();
                arguments.Add($"-c:{settings.Configuration}");

                if (!settings.IncludeDebugSymbols)
                {
                    arguments.Add("/p:DebugType=None");
                    arguments.Add("/p:DebugSymbols=false");
                }

                cmd += string.Join(' ', arguments);

                return RunCommand(projectFilePath, cmd);
            }

            return false;
        }

        private static bool PublishProject(TrakHoundPackage packageConfiguration, TrakHoundDotNetPackageSettings settings, string projectFilePath, string outputPath)
        {
            if (packageConfiguration != null && settings != null && !string.IsNullOrEmpty(settings.Configuration) && !string.IsNullOrEmpty(projectFilePath))
            {
                var cmd = $"dotnet publish \"{projectFilePath}\" ";
                if (settings != null && !string.IsNullOrEmpty(settings.DotnetSdkLocation)) cmd = Path.Combine(settings.DotnetSdkLocation, cmd);

                var arguments = new List<string>();
                arguments.Add($"-c:{settings.Configuration}");

                if (!settings.IncludeDebugSymbols)
                {
                    arguments.Add("/p:DebugType=None");
                    arguments.Add("/p:DebugSymbols=false");
                }

                arguments.Add($"-o:\"{outputPath}\"");
                cmd += string.Join(' ', arguments);

                return RunCommand(cmd);
            }

            return false;
        }


        private static void ConvertProjectFile(string trakhoundVersion, string packageCategory, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(packageCategory) && !string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                switch (packageCategory.ToLower())
                {
                    case "api": ConvertApiProjectFile(trakhoundVersion, packageId, inputProjectFilePath, outputProjectFilePath); break;
                    case "app": ConvertAppProjectFile(trakhoundVersion, packageId, inputProjectFilePath, outputProjectFilePath); break;
                    case "function": ConvertFunctionProjectFile(trakhoundVersion, packageId, inputProjectFilePath, outputProjectFilePath); break;
                    case "service": ConvertServiceProjectFile(trakhoundVersion, packageId, inputProjectFilePath, outputProjectFilePath); break;
                    case "driver": ConvertDriverProjectFile(trakhoundVersion, packageId, inputProjectFilePath, outputProjectFilePath); break;
                    case "identity": ConvertIdentityProjectFile(trakhoundVersion, packageId, inputProjectFilePath, outputProjectFilePath); break;
                }
            }
        }

        private static void ConvertApiProjectFile(string trakhoundVersion, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                var xml = new XmlDocument();
                xml.Load(inputProjectFilePath);

                var projectNode = xml.SelectSingleNode("/Project");
                if (projectNode != null)
                {
                    ClearProjectSdk(xml, projectNode);
                    RemoveImportSdk(xml);
                    RemoveDebugFiles(xml);
                    RemoveDebugProperties(xml);
                    RemoveDebugReferences(xml);
                    RemoveTrakHoundReferences(xml);

                    AddImportSdkProps(xml, projectNode);
                    AddImportSdkTargets(xml, projectNode);

                    RemoveDebugOutputProperties(xml);
                    AddOutputProperties(xml, projectNode, packageId);

                    AddApiProperties(xml, projectNode);
                    AddApiReferences(xml, projectNode, trakhoundVersion);

                    IgnoreFile(xml, projectNode, "Program.cs", "Compile");

                    xml.Save(outputProjectFilePath);
                }
            }
        }

        private static void ConvertAppProjectFile(string trakhoundVersion, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                var xml = new XmlDocument();
                xml.Load(inputProjectFilePath);

                var projectNode = xml.SelectSingleNode("/Project");
                if (projectNode != null)
                {
                    ClearProjectSdk(xml, projectNode);
                    RemoveImportSdk(xml);
                    RemoveDebugFiles(xml);
                    RemoveDebugProperties(xml);
                    RemoveDebugReferences(xml);
                    RemoveTrakHoundReferences(xml);

                    AddImportSdkRazorProps(xml, projectNode);
                    AddImportSdkRazorTargets(xml, projectNode);

                    RemoveDebugOutputProperties(xml);
                    AddOutputProperties(xml, projectNode, packageId);

                    AddAppProperties(xml, projectNode);
                    AddAppItems(xml, projectNode);
                    AddAppReferences(xml, projectNode, trakhoundVersion);

                    IgnoreFile(xml, projectNode, "Program.cs", "Compile");
                    IgnoreFile(xml, projectNode, "Debug.razor", "Content");
                    IgnoreFile(xml, projectNode, "DebugRoutes.razor", "Content");

                    xml.Save(outputProjectFilePath);
                }
            }
        }

        private static void ConvertFunctionProjectFile(string trakhoundVersion, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                var xml = new XmlDocument();
                xml.Load(inputProjectFilePath);

                var projectNode = xml.SelectSingleNode("/Project");
                if (projectNode != null)
                {
                    ClearProjectSdk(xml, projectNode);
                    RemoveImportSdk(xml);
                    RemoveDebugFiles(xml);
                    RemoveDebugProperties(xml);
                    RemoveDebugReferences(xml);
                    RemoveTrakHoundReferences(xml);

                    AddImportSdkProps(xml, projectNode);
                    AddImportSdkTargets(xml, projectNode);

                    RemoveDebugOutputProperties(xml);
                    AddOutputProperties(xml, projectNode, packageId);

                    AddFunctionProperties(xml, projectNode);
                    AddFunctionReferences(xml, projectNode, trakhoundVersion);

                    IgnoreFile(xml, projectNode, "Program.cs", "Compile");

                    xml.Save(outputProjectFilePath);
                }
            }
        }

        private static void ConvertServiceProjectFile(string trakhoundVersion, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                var xml = new XmlDocument();
                xml.Load(inputProjectFilePath);

                var projectNode = xml.SelectSingleNode("/Project");
                if (projectNode != null)
                {
                    ClearProjectSdk(xml, projectNode);
                    RemoveImportSdk(xml);
                    RemoveDebugFiles(xml);
                    RemoveDebugProperties(xml);
                    RemoveDebugReferences(xml);
                    RemoveTrakHoundReferences(xml);

                    AddImportSdkProps(xml, projectNode);
                    AddImportSdkTargets(xml, projectNode);

                    RemoveDebugOutputProperties(xml);
                    AddOutputProperties(xml, projectNode, packageId);

                    AddServiceProperties(xml, projectNode);
                    AddServiceReferences(xml, projectNode, trakhoundVersion);

                    IgnoreFile(xml, projectNode, "Program.cs", "Compile");

                    xml.Save(outputProjectFilePath);
                }
            }
        }

        private static void ConvertDriverProjectFile(string trakhoundVersion, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                var xml = new XmlDocument();
                xml.Load(inputProjectFilePath);

                var projectNode = xml.SelectSingleNode("/Project");
                if (projectNode != null)
                {
                    ClearProjectSdk(xml, projectNode);
                    RemoveImportSdk(xml);
                    RemoveDebugFiles(xml);
                    RemoveDebugProperties(xml);
                    RemoveDebugReferences(xml);
                    RemoveTrakHoundReferences(xml);

                    AddImportSdkProps(xml, projectNode);
                    AddImportSdkTargets(xml, projectNode);

                    RemoveDebugOutputProperties(xml);
                    AddOutputProperties(xml, projectNode, packageId);

                    AddDriverProperties(xml, projectNode);
                    AddDriverReferences(xml, projectNode, trakhoundVersion);

                    xml.Save(outputProjectFilePath);
                }
            }
        }

        private static void ConvertIdentityProjectFile(string trakhoundVersion, string packageId, string inputProjectFilePath, string outputProjectFilePath)
        {
            if (!string.IsNullOrEmpty(inputProjectFilePath) && !string.IsNullOrEmpty(outputProjectFilePath))
            {
                var xml = new XmlDocument();
                xml.Load(inputProjectFilePath);

                var projectNode = xml.SelectSingleNode("/Project");
                if (projectNode != null)
                {
                    ClearProjectSdk(xml, projectNode);
                    RemoveImportSdk(xml);
                    RemoveDebugFiles(xml);
                    RemoveDebugProperties(xml);
                    RemoveDebugReferences(xml);
                    RemoveTrakHoundReferences(xml);

                    AddImportSdkProps(xml, projectNode);
                    AddImportSdkTargets(xml, projectNode);

                    RemoveDebugOutputProperties(xml);
                    AddOutputProperties(xml, projectNode, packageId);

                    AddIdentityProperties(xml, projectNode);
                    AddIdentityReferences(xml, projectNode, trakhoundVersion);

                    xml.Save(outputProjectFilePath);
                }
            }
        }


        private static void ClearProjectSdk(XmlDocument xml, XmlNode projectNode)
        {
            projectNode.Attributes.RemoveNamedItem("Sdk");
        }

        private static void RemoveImportSdk(XmlDocument xml)
        {
            // Remove Web SDK props (Web Api Project)
            var nodes = xml.SelectNodes("//Import[@Project='Sdk.props']");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes) x.ParentNode.RemoveChild(x);
            }

            // Remove Web SDK props (Web Api Project)
            nodes = xml.SelectNodes("//Import[@Project='Sdk.targets']");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes) x.ParentNode.RemoveChild(x);
            }
        }

        private static void RemoveDebugFiles(XmlDocument xml)
        {
            var nodes = xml.SelectNodes("//ItemGroup[@Label='files-trakhound-debug']");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes) x.ParentNode.RemoveChild(x);
            }
        }

        private static void RemoveDebugProperties(XmlDocument xml)
        {
            RemoveProperty(xml, "OutputType");
        }

        private static void RemoveDebugReferences(XmlDocument xml)
        {
            var nodes = xml.SelectNodes("//ItemGroup[@Label='references-trakhound-debug']");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes) x.ParentNode.RemoveChild(x);
            }
        }

        private static void RemoveTrakHoundReferences(XmlDocument xml)
        {
            RemoveReference(xml, "TrakHound.Blazor");
            RemoveReference(xml, "TrakHound.Common");
            RemoveReference(xml, "TrakHound.Clients");
            RemoveReference(xml, "TrakHound.Debug.Apps");
            RemoveReference(xml, "TrakHound.Debug.AspNetCore");
            RemoveReference(xml, "TrakHound.Drivers");
        }


        private static void AddImportSdkProps(XmlDocument xml, XmlNode projectNode)
        {
            var importNode = AddImport(xml, projectNode, "Sdk.props", "Microsoft.NET.Sdk");
            projectNode.InsertBefore(importNode, projectNode.FirstChild);
        }

        private static void AddImportSdkTargets(XmlDocument xml, XmlNode projectNode)
        {
            var importNode = AddImport(xml, projectNode, "Sdk.targets", "Microsoft.NET.Sdk");
            projectNode.InsertAfter(importNode, projectNode.LastChild);
        }

        private static void AddImportSdkRazorProps(XmlDocument xml, XmlNode projectNode)
        {
            var importNode = AddImport(xml, projectNode, "Sdk.props", "Microsoft.NET.Sdk.Razor");
            projectNode.InsertBefore(importNode, projectNode.FirstChild);
        }

        private static void AddImportSdkRazorTargets(XmlDocument xml, XmlNode projectNode)
        {
            var importNode = AddImport(xml, projectNode, "Sdk.targets", "Microsoft.NET.Sdk.Razor");
            projectNode.InsertAfter(importNode, projectNode.LastChild);
        }


        private static void AddOutputProperties(XmlDocument xml, XmlNode projectNode, string packageId)
        {
            var tempDir = TrakHoundTemp.CreateDirectory();
            var tempBin = Path.Combine(tempDir, "bin");
            var tempObj = Path.Combine(tempDir, "obj");

            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-output-properties");

            var rootNamespace = GetProperty(xml, "RootNamespace");
            if (string.IsNullOrEmpty(rootNamespace))
            {
                AddProperty(xml, propertyGroupNode, "RootNamespace", packageId);
            }

            AddProperty(xml, propertyGroupNode, "OutputPath", tempBin);
            AddProperty(xml, propertyGroupNode, "BaseOutputPath", tempBin);
            AddProperty(xml, propertyGroupNode, "MSBuildProjectExtensionsPath", tempObj);
            AddProperty(xml, propertyGroupNode, "BaseIntermediateOutputPath", tempObj);
            AddProperty(xml, propertyGroupNode, "IntermediateOutputPath", tempObj);

            projectNode.InsertBefore(propertyGroupNode, projectNode.FirstChild);
        }

        private static void RemoveDebugOutputProperties(XmlDocument xml)
        {
            var nodes = xml.SelectNodes("//PropertyGroup[@Label='trakhound-debug-output-properties']");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes)
                {
                    if (x != null && x.ParentNode != null)
                    {
                        var parentNode = x.ParentNode;
                        parentNode.RemoveChild(x);
                        if (!parentNode.HasChildNodes) parentNode.ParentNode.RemoveChild(parentNode);
                    }
                }
            }
        }



        private static void AddApiProperties(XmlDocument xml, XmlNode projectNode)
        {
            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-properties");

            AddProperty(xml, propertyGroupNode, "OutputType", "Library");
            AddProperty(xml, propertyGroupNode, "EnableDynamicLoading", "true");
            AddProperty(xml, propertyGroupNode, "GenerateAssemblyInfo", "false");

            projectNode.InsertBefore(propertyGroupNode, projectNode.LastChild);
        }

        private static void AddApiReferences(XmlDocument xml, XmlNode projectNode, string trakhoundVersion)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-references");

            AddPackageReference(xml, itemGroupNode, "TrakHound.Common", trakhoundVersion, true);

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static void AddAppProperties(XmlDocument xml, XmlNode projectNode)
        {
            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-properties");

            AddProperty(xml, propertyGroupNode, "EnableDynamicLoading", "true");
            AddProperty(xml, propertyGroupNode, "GenerateAssemblyInfo", "false");

            projectNode.InsertBefore(propertyGroupNode, projectNode.LastChild);
        }

        private static void AddAppItems(XmlDocument xml, XmlNode projectNode)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-items");

            AddItem(xml, itemGroupNode, "FrameworkReference", "Microsoft.AspNetCore.App");
            AddItem(xml, itemGroupNode, "SupportedPlatform", "browser");

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }

        private static void AddAppReferences(XmlDocument xml, XmlNode projectNode, string trakhoundVersion)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-references");

            AddPackageReference(xml, itemGroupNode, "TrakHound.Blazor", trakhoundVersion, true);

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static void AddFunctionProperties(XmlDocument xml, XmlNode projectNode)
        {
            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-properties");

            AddProperty(xml, propertyGroupNode, "OutputType", "Library");
            AddProperty(xml, propertyGroupNode, "EnableDynamicLoading", "true");
            AddProperty(xml, propertyGroupNode, "GenerateAssemblyInfo", "false");

            projectNode.InsertBefore(propertyGroupNode, projectNode.LastChild);
        }

        private static void AddFunctionReferences(XmlDocument xml, XmlNode projectNode, string trakhoundVersion)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-references");

            AddPackageReference(xml, itemGroupNode, "TrakHound.Common", trakhoundVersion, true);

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static void AddServiceProperties(XmlDocument xml, XmlNode projectNode)
        {
            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-properties");

            AddProperty(xml, propertyGroupNode, "OutputType", "Library");
            AddProperty(xml, propertyGroupNode, "EnableDynamicLoading", "true");
            AddProperty(xml, propertyGroupNode, "GenerateAssemblyInfo", "false");

            projectNode.InsertBefore(propertyGroupNode, projectNode.LastChild);
        }

        private static void AddServiceReferences(XmlDocument xml, XmlNode projectNode, string trakhoundVersion)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-references");

            AddPackageReference(xml, itemGroupNode, "TrakHound.Common", trakhoundVersion, true);

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static void AddDriverProperties(XmlDocument xml, XmlNode projectNode)
        {
            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-properties");

            AddProperty(xml, propertyGroupNode, "OutputType", "Library");
            AddProperty(xml, propertyGroupNode, "EnableDynamicLoading", "true");
            //AddProperty(xml, propertyGroupNode, "GenerateAssemblyInfo", "false");

            projectNode.InsertBefore(propertyGroupNode, projectNode.LastChild);
        }

        private static void AddDriverReferences(XmlDocument xml, XmlNode projectNode, string trakhoundVersion)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-references");

            AddPackageReference(xml, itemGroupNode, "TrakHound.Drivers", trakhoundVersion, true);

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static void AddIdentityProperties(XmlDocument xml, XmlNode projectNode)
        {
            var propertyGroupNode = AddPropertyGroup(xml, projectNode, "trakhound-package-properties");

            AddProperty(xml, propertyGroupNode, "OutputType", "Library");
            AddProperty(xml, propertyGroupNode, "EnableDynamicLoading", "true");
            AddProperty(xml, propertyGroupNode, "GenerateAssemblyInfo", "false");

            projectNode.InsertBefore(propertyGroupNode, projectNode.LastChild);
        }

        private static void AddIdentityReferences(XmlDocument xml, XmlNode projectNode, string trakhoundVersion)
        {
            var itemGroupNode = AddItemGroup(xml, projectNode, "trakhound-package-references");

            AddPackageReference(xml, itemGroupNode, "TrakHound.Common", trakhoundVersion, true);

            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static XmlNode AddImport(XmlDocument xml, XmlNode projectNode, string project, string sdk)
        {
            var importNode = xml.CreateNode(XmlNodeType.Element, "Import", null);

            var projectAttribute = xml.CreateAttribute("Project");
            projectAttribute.Value = project;
            importNode.Attributes.Append(projectAttribute);

            var sdkAttribute = xml.CreateAttribute("Sdk");
            sdkAttribute.Value = sdk;
            importNode.Attributes.Append(sdkAttribute);

            return importNode;
        }

        private static XmlNode AddPropertyGroup(XmlDocument xml, XmlNode projectNode, string label = null, Dictionary<string, string> attributes = null)
        {
            var groupNode = xml.CreateNode(XmlNodeType.Element, "PropertyGroup", null);

            if (!string.IsNullOrEmpty(label))
            {
                var xmlAttribute = xml.CreateAttribute("Label");
                xmlAttribute.Value = label;
                groupNode.Attributes.Append(xmlAttribute);
            }

            if (!attributes.IsNullOrEmpty())
            {
                foreach (var attribute in attributes)
                {
                    var xmlAttribute = xml.CreateAttribute(attribute.Key);
                    xmlAttribute.Value = attribute.Value;
                    groupNode.Attributes.Append(xmlAttribute);
                }
            }

            return groupNode;
        }

        private static void AddProperty(XmlDocument xml, XmlNode propertyGroupNode, string propertyName, string propertyValue)
        {
            var node = xml.CreateNode(XmlNodeType.Element, propertyName, null);
            node.InnerText = propertyValue;
            propertyGroupNode.AppendChild(node);
        }

        private static string GetProperty(XmlDocument xml, string propertyName)
        {
            var node = xml.SelectSingleNode($"//PropertyGroup/{propertyName}");
            if (node != null)
            {
                return node.InnerText;
            }

            return null;
        }

        private static void RemoveProperty(XmlDocument xml, string propertyName)
        {
            var nodes = xml.SelectNodes($"//PropertyGroup/{propertyName}");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes)
                {
                    if (x != null && x.ParentNode != null)
                    {
                        var parentNode = x.ParentNode;
                        parentNode.RemoveChild(x);
                        if (!parentNode.HasChildNodes) parentNode.ParentNode.RemoveChild(parentNode);
                    }
                }
            }
        }


        private static XmlNode AddItemGroup(XmlDocument xml, XmlNode projectNode, string label = null, Dictionary<string, string> attributes = null)
        {
            var groupNode = xml.CreateNode(XmlNodeType.Element, "ItemGroup", null);

            if (!string.IsNullOrEmpty(label))
            {
                var xmlAttribute = xml.CreateAttribute("Label");
                xmlAttribute.Value = label;
                groupNode.Attributes.Append(xmlAttribute);
            }

            if (!attributes.IsNullOrEmpty())
            {
                foreach (var attribute in attributes)
                {
                    var xmlAttribute = xml.CreateAttribute(attribute.Key);
                    xmlAttribute.Value = attribute.Value;
                    groupNode.Attributes.Append(xmlAttribute);
                }
            }

            return groupNode;
        }

        private static void AddItem(XmlDocument xml, XmlNode itemGroupNode, string itemName, string include)
        {
            var node = xml.CreateNode(XmlNodeType.Element, itemName, null);

            var includeAttribute = xml.CreateAttribute("Include");
            includeAttribute.Value = include;
            node.Attributes.Append(includeAttribute);

            itemGroupNode.AppendChild(node);
        }


        private static void AddPackageReference(XmlDocument xml, XmlNode itemGroupNode, string packageId, string version = "*", bool privateAssets = false)
        {
            var referenceNode = xml.CreateNode(XmlNodeType.Element, "PackageReference", null);

            var includeAttribute = xml.CreateAttribute("Include");
            includeAttribute.Value = packageId;
            referenceNode.Attributes.Append(includeAttribute);

            var versionAttribute = xml.CreateAttribute("Version");
            versionAttribute.Value = !string.IsNullOrEmpty(version) ? version : "*";
            referenceNode.Attributes.Append(versionAttribute);

            var privateNode = xml.CreateNode(XmlNodeType.Element, "Private", null);
            privateNode.InnerText = "false";
            referenceNode.AppendChild(privateNode);

            var excludeAssetsNode = xml.CreateNode(XmlNodeType.Element, "ExcludeAssets", null);
            excludeAssetsNode.InnerText = "runtime";
            referenceNode.AppendChild(excludeAssetsNode);

            itemGroupNode.AppendChild(referenceNode);
        }

        private static void RemoveReference(XmlDocument xml, string packageName)
        {
            // Remove Package References
            var nodes = xml.SelectNodes($"//PackageReference[@Include='{packageName}']");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes)
                {
                    if (x != null && x.ParentNode != null)
                    {
                        var parentNode = x.ParentNode;
                        parentNode.RemoveChild(x);
                        if (!parentNode.HasChildNodes) parentNode.ParentNode.RemoveChild(parentNode);
                    }
                }
            }

            // Remove Project References
            nodes = xml.SelectNodes($"//ProjectReference[contains(@Include, '\\{packageName}.csproj')]");
            if (nodes != null)
            {
                foreach (XmlNode x in nodes)
                {
                    if (x != null && x.ParentNode != null)
                    {
                        var parentNode = x.ParentNode;
                        parentNode.RemoveChild(x);
                        if (!parentNode.HasChildNodes) parentNode.ParentNode.RemoveChild(parentNode);
                    }
                }
            }
        }


        private static void IgnoreFile(XmlDocument xml, XmlNode projectNode, string filename, string buildType)
        {
            var itemGroupNode = xml.CreateNode(XmlNodeType.Element, "ItemGroup", null);

            var itemGroupLabelAttribute = xml.CreateAttribute("Label");
            itemGroupLabelAttribute.Value = "trakhound-package-ignore-files";
            itemGroupNode.Attributes.Append(itemGroupLabelAttribute);


            var compileNode = xml.CreateNode(XmlNodeType.Element, buildType, null);

            var removeAttribute = xml.CreateAttribute("Remove");
            removeAttribute.Value = filename;
            compileNode.Attributes.Append(removeAttribute);

            itemGroupNode.AppendChild(compileNode);


            var noneNode = xml.CreateNode(XmlNodeType.Element, "None", null);

            var includeAttribute = xml.CreateAttribute("Include");
            includeAttribute.Value = filename;
            noneNode.Attributes.Append(includeAttribute);

            itemGroupNode.AppendChild(noneNode);


            projectNode.InsertBefore(itemGroupNode, projectNode.LastChild);
        }


        private static bool RunCommand(string command)
        {
            return RunCommand(null, command);
        }

        private static bool RunCommand(string workingDirectory, string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                try
                {
                    var commandParts = command.Split(' ');
                    var executable = commandParts[0];
                    var arguments = string.Join(' ', commandParts.Skip(1));

                    var startInfo = new ProcessStartInfo(executable, arguments);
                    //var startInfo = new ProcessStartInfo("cmd", "/c " + command);
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    if (!string.IsNullOrEmpty(workingDirectory)) startInfo.WorkingDirectory = Path.GetDirectoryName(workingDirectory);

                    var outputBuilder = new StringBuilder();

                    using (var outputStream = new MemoryStream())
                    using (var outputWriter = new StreamWriter(outputStream))
                    using (var process = new Process())
                    {
                        process.StartInfo = startInfo;
                        process.Start();

                        using (var stream = process.StandardOutput)
                        {
                            while (!process.StandardOutput.EndOfStream) outputBuilder.AppendLine(process.StandardOutput.ReadLine());
                        }

                        using (var stream = process.StandardError)
                        {
                            while (!process.StandardError.EndOfStream) outputBuilder.AppendLine(process.StandardError.ReadLine());
                        }

                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            Console.WriteLine(outputBuilder.ToString());
                        }

                        return process.ExitCode == 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("TrakHoundDotNetPackage.RunCommand() : Command = " + command + " : WorkingDirectory = " + workingDirectory + " : " + ex.Message);
                }
            }

            return false;
        }


        class ProjectAssetsFile
        {
            [JsonPropertyName("targets")]
            public Dictionary<string, Dictionary<string, object>> Targets { get; set; }
        }

        class ProjectAssetsFileTarget
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("path")]
            public string Path { get; set; }
        }


        private static string GetTrakHoundVersion(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var assetsPath = Path.Combine(path, "obj", "project.assets.json");
                    var assets = Json.Read<ProjectAssetsFile>(assetsPath);
                    if (assets != null)
                    {
                        foreach (var target in assets.Targets.Values)
                        {
                            foreach (var key in target.Keys)
                            {
                                var keyParts = key.Split('/');
                                var packageName = keyParts[0];
                                var packageVersion = keyParts[1];

                                if (packageName == "TrakHound.Common")
                                {
                                    var keyTarget = Json.ConvertObject<ProjectAssetsFileTarget>(target.GetValueOrDefault(key));
                                    if (keyTarget != null)
                                    {
                                        switch (keyTarget.Type)
                                        {
                                            case "package": return packageVersion;

                                            case "project": return "*";
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

            return null;
        }
    }
}
