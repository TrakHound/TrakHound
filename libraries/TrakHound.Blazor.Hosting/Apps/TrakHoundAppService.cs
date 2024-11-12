// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Text.RegularExpressions;
using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Blazor.Routing;
using TrakHound.Clients;
using TrakHound.Configurations;
using TrakHound.Instances;
using TrakHound.Modules;
using TrakHound.Packages;
using TrakHound.Requests;

namespace TrakHound.Blazor.Apps
{
    public class TrakHoundAppService : ITrakHoundAppProvider
    {
        private const string _packageCategory = "app";
        private const string _packagesDirectory = "_packages";

        private const string _routeParameterRegexPattern = @"(\{(.*?)\})";
        private static readonly Regex _routeParameterRegex = new Regex(_routeParameterRegexPattern, RegexOptions.Compiled);

        private readonly ITrakHoundInstance _instance;
        private readonly ITrakHoundConfigurationProfile _configurationProfile;
        private readonly ITrakHoundPageRouteManager _routeManager;
        private readonly ITrakHoundPackageManager _packageManager;
        private readonly ITrakHoundModuleManager _moduleManager;
        private readonly string _baseUrl;
        private readonly Dictionary<string, TrakHoundAppInformation> _appInformations = new Dictionary<string, TrakHoundAppInformation>();
        private readonly Dictionary<string, TrakHoundPackage> _appPackages = new Dictionary<string, TrakHoundPackage>();
        private readonly Dictionary<string, ModuleInfo> _moduleInfos = new Dictionary<string, ModuleInfo>();
        private readonly ListDictionary<string, string> _packageStylesheets = new ListDictionary<string, string>();
        private readonly ListDictionary<string, string> _packageScripts = new ListDictionary<string, string>();
        private readonly Dictionary<string, string> _installedConfigurationHashes = new Dictionary<string, string>(); // Configuration.Id => Configuration.Hash
        private readonly Dictionary<string, string> _installedModuleHashes = new Dictionary<string, string>(); // Configuration.Id => Module.Hash
        private AppInfo[] _routes;
        private readonly CacheDictionary<string, bool?> _routeCache = new CacheDictionary<string, bool?>();
        private readonly DelayEvent _delayLoadEvent = new DelayEvent(2000);
        private readonly object _lock = new object();


        public IEnumerable<string> PackageStylesheets => _packageStylesheets.Values;

        public IEnumerable<string> PackageScripts => _packageScripts.Values;


        public event TrakHoundAppLogHandler AppLogUpdated;


        public bool IsRouteValid(string route)
        {
            if (route != null)
            {
                _routeManager.Initialise();
                var match = _routeManager.Match(route);
                return match.IsMatch;
            }

            return false;
        }


        public TrakHoundAppService(
            ITrakHoundInstance instance,
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundPageRouteManager routeManager,
            ITrakHoundPackageManager packageManager,
            ITrakHoundModuleProvider moduleProvider
            )
        {
            _instance = instance;
            _delayLoadEvent.Elapsed += UpdateDelayElapsed;

            // Set Base URL (HTTP Address)
            if (_instance != null)
            {
                _baseUrl = _instance.Information.GetInterface("HTTP")?.GetBaseUrl();
            }

            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ConfigurationUpdated;
            _configurationProfile.ConfigurationRemoved += ConfigurationUpdated;

            _routeManager = routeManager;
            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ComponentBase>(_packageCategory);

            Load();
        }

        public TrakHoundAppService(
            ITrakHoundInstance instance,
            ITrakHoundConfigurationProfile configurationProfile,
            ITrakHoundPageRouteManager routeManager,
            ITrakHoundPackageManager packageManager,
            ITrakHoundModuleProvider moduleProvider,
            TrakHoundModuleContext context
            )
        {
            _instance = instance;
            _delayLoadEvent.Elapsed += UpdateDelayElapsed;

            _configurationProfile = configurationProfile;
            _configurationProfile.ConfigurationAdded += ConfigurationUpdated;
            _configurationProfile.ConfigurationRemoved += ConfigurationUpdated;

            _routeManager = routeManager;
            _packageManager = packageManager;
            _packageManager.PackageAdded += PackageAdded;
            _packageManager.PackageRemoved += PackageRemoved;

            _moduleManager = moduleProvider.Get<ComponentBase>(_packageCategory, context);

            Load();
        }


        private void PackageAdded(object sender, TrakHoundPackage package)
        {
            if (package != null && package.Category == _packageCategory)
            {
                _delayLoadEvent.Set();
            }
        }

        private void PackageRemoved(object sender, TrakHoundPackageChangedArgs args)
        {
            if (args.PackageCategory == _packageCategory)
            {
                _delayLoadEvent.Set();
            }
        }


        private void ConfigurationUpdated(object sender, ITrakHoundConfiguration configuration)
        {
            if (configuration != null && configuration.Category == TrakHoundAppConfiguration.ConfigurationCategory)
            {
                _delayLoadEvent.Set();
            }
        }


        public IEnumerable<TrakHoundAppInformation> GetInformation()
        {
            var informations = new List<TrakHoundAppInformation>();

            lock (_lock)
            {
                if (!_appInformations.IsNullOrEmpty())
                {
                    foreach (var appInformation in _appInformations)
                    {
                        informations.Add(appInformation.Value);
                    }
                }
            }

            return informations;
        }

        public TrakHoundAppInformation GetInformation(string appId)
        {
            var informations = new List<TrakHoundAppInformation>();

            lock (_lock)
            {
                if (!_appInformations.IsNullOrEmpty())
                {
                    var appInformation = _appInformations.GetValueOrDefault(appId);
                    if (appInformation != null)
                    {
                        return appInformation;
                    }
                }
            }

            return null;
        }

        public TrakHoundAppPageInformation GetPageInformation(string route)
        {
            var appInformations = GetInformation();
            if (!appInformations.IsNullOrEmpty())
            {
                var fMatchRoute = route?.TrimStart('/');

                foreach (var appInformation in appInformations)
                {
                    if (!appInformation.Pages.IsNullOrEmpty())
                    {
                        foreach (var pageInformation in appInformation.Pages)
                        {
                            var pageRoute = Url.Combine(appInformation.Route, pageInformation.Route);
                            var fPageRoute = pageRoute?.TrimStart('/');

                            if (IsRouteMatch(fMatchRoute, fPageRoute))
                            {
                                pageInformation.App = appInformation;
                                return pageInformation;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private bool IsRouteMatch(string url, string route)
        {
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(route)) return true;

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(route))
            {
                var match = false;

                var cacheKey = $"{url}:{route}";
                var cachedMatch = _routeCache.Get(cacheKey);
                if (cachedMatch != null) match = cachedMatch.Value;
                if (cachedMatch == null)
                {
                    var routeRegex = CreateRouteRegexPattern(route);

                    var urlRegex = new Regex(routeRegex);
                    if (urlRegex.IsMatch(url))
                    {
                        var urlParts = url.Split('/');
                        var routeParts = route.Split('/');

                        match = urlParts != null && routeParts != null && urlParts.Length == routeParts.Length;
                    }

                    _routeCache.Add(cacheKey, match);
                }

                return match;
            }

            return false;
        }

        private static string CreateRouteRegexPattern(string route)
        {
            var pattern = route;

            if (!string.IsNullOrEmpty(route))
            {
                if (_routeParameterRegex.IsMatch(route))
                {
                    var matches = _routeParameterRegex.Matches(route);
                    if (!matches.IsNullOrEmpty())
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Groups?.Count > 1)
                            {
                                var matchText = match.Groups[1].ToString();

                                pattern = pattern.Replace(matchText, ".*");
                            }
                        }
                    }
                }
            }

            return $"^{pattern}$";
        }

        public Type GetModuleType(string packageId, string packageVersion = null)
        {
            if (!string.IsNullOrEmpty(packageId))
            {
                if (!string.IsNullOrEmpty(packageVersion))
                {
                    var key = $"{packageId}:{packageVersion}";
                    lock (_lock) return _moduleInfos.GetValueOrDefault(key).Type;
                }
                else
                {
                    lock (_lock)
                    {
                        var values = _moduleInfos.Values;
                        return values.Where(o => o.PackageId == packageId)?.OrderByDescending(o => o.PackageVersion.ToVersion()).FirstOrDefault().Type;
                    }
                }
            }


            return null;
        }

        public IEnumerable<string> GetAppStylesheets(string appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                lock (_lock)
                {
                    return _packageStylesheets.Get(appId);
                }
            }

            return null;
        }

        public IEnumerable<string> GetAppScripts(string appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                lock (_lock)
                {
                    return _packageScripts.Get(appId);
                }
            }

            return null;
        }


        private void UpdateDelayElapsed(object sender, EventArgs e)
        {
            Load();
        }

        private void Update(object sender, ITrakHoundModule module)
        {
            _delayLoadEvent.Set();
        }

        private void Load()
        {
            LoadInformation();
            LoadPages();
            LoadStylesheets();
            LoadScripts();
            LoadStaticAssets();
        }


        private void LoadInformation()
        {
            var foundConfigurationIds = new List<string>();

            var configurations = _configurationProfile.Get<ITrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);
            if (!configurations.IsNullOrEmpty())
            {
                foreach (var configuration in configurations)
                {
                    var module = _moduleManager.Get(configuration.PackageId, configuration.PackageVersion);
                    if (module != null)
                    {
                        // Get the Installed Hash (to check if the configuration has changed)
                        var installedConfigurationHash = _installedConfigurationHashes.GetValueOrDefault(configuration.Id);
                        var installedModuleHash = _installedModuleHashes.GetValueOrDefault(configuration.Id);

                        if (configuration.Hash != installedConfigurationHash || module.Package.Hash != installedModuleHash)
                        {
                            _installedConfigurationHashes.Remove(configuration.Id);
                            _installedModuleHashes.Remove(configuration.Id);

                            ConfigureRoute(configuration, module);

                            // Add to installed List
                            _installedConfigurationHashes.Add(configuration.Id, configuration.Hash);
                            _installedModuleHashes.Add(configuration.Id, module.Package.Hash);
                        }

                        foundConfigurationIds.Add(configuration.Id);
                    }
                }
            }

            // Remove unused
            lock (_lock)
            {
                if (!_appInformations.IsNullOrEmpty())
                {
                    var configurationIds = _appInformations.Keys.ToList();
                    var notFoundConfigurationIds = new List<string>();


                    foreach (var configurationId in configurationIds)
                    {
                        if (!foundConfigurationIds.Contains(configurationId))
                        {
                            notFoundConfigurationIds.Add(configurationId);
                        }
                    }

                    foreach (var configurationId in notFoundConfigurationIds)
                    {
                        _appInformations.Remove(configurationId);
                        _appPackages.Remove(configurationId);
                    }
                }
            }
        }

        private void ConfigureRoute(ITrakHoundAppConfiguration configuration, ITrakHoundModule module)
        {
            if (configuration != null && module != null && !module.ModuleTypes.IsNullOrEmpty())
            {
                // Read the Package ReadMe (if exists)
                var packageReadMe = _packageManager.GetReadMe(module.Package.Category, module.Package.Id, module.Package.Version);

                var pageInfos = new List<TrakHoundAppPageInformation>();
                foreach (var moduleType in module.ModuleTypes) // Page Types
                {
                    var pageParameterInfos = new List<TrakHoundAppPageParameterInformation>();

                    var pageProperties = moduleType.GetProperties();
                    if (!pageProperties.IsNullOrEmpty())
                    {
                        foreach (var pageProperty in pageProperties)
                        {
                            var parameterAttribute = pageProperty.GetCustomAttribute<ParameterAttribute>();
                            if (parameterAttribute != null)
                            {
                                var pageParameterInfo = new TrakHoundAppPageParameterInformation();
                                pageParameterInfo.Name = pageProperty.Name;
                                pageParameterInfo.DataType = pageProperty.PropertyType.ToString();
                                pageParameterInfos.Add(pageParameterInfo);
                            }
                        }
                    }

                    var routeAttributes = moduleType.GetCustomAttributes<RouteAttribute>();
                    if (!routeAttributes.IsNullOrEmpty())
                    {
                        foreach (var routeAttribute in routeAttributes)
                        {
                            if (!string.IsNullOrEmpty(routeAttribute.Template))
                            {
                                var pageInfo = new TrakHoundAppPageInformation();
                                pageInfo.Id = $"{configuration.Id}:{moduleType.FullName}".ToMD5Hash();
                                pageInfo.Route = routeAttribute.Template;
                                pageInfo.Parameters = pageParameterInfos;
                                pageInfos.Add(pageInfo);
                            }
                        }
                    }
                }


                // Create Information
                var appInformation = new TrakHoundAppInformation();
                appInformation.Id = configuration.Id;
                appInformation.Name = configuration.Name;
                appInformation.Route = configuration.Route;
                appInformation.PackageId = configuration.PackageId;

                appInformation.PackageVersion = module.Package.Version;
                appInformation.PackageBuildDate = module.Package.BuildDate;
                appInformation.PackageHash = module.Package.Hash;
                appInformation.PackageIcon = module.Package.GetMetadata(TrakHoundPackage.ImageName);
                appInformation.PackageReadMe = packageReadMe;

                appInformation.TrakHoundVersion = module.Package.GetMetadata(TrakHoundPackage.TrakHoundVersionName);

                appInformation.Repository = module.Package.GetMetadata(TrakHoundPackage.RepositoryName);
                appInformation.RepositoryBranch = module.Package.GetMetadata(TrakHoundPackage.RepositoryBranchName);
                appInformation.RepositoryDirectory = module.Package.GetMetadata(TrakHoundPackage.RepositoryDirectoryName);
                appInformation.RepositoryCommit = module.Package.GetMetadata(TrakHoundPackage.RepositoryCommitName);

                appInformation.Pages = pageInfos;

                lock (_lock)
                {
                    _appInformations.Remove(appInformation.Id);
                    _appInformations.Add(appInformation.Id, appInformation);

                    _appPackages.Remove(appInformation.Id);
                    _appPackages.Add(appInformation.Id, module.Package);
                }

                if (_instance != null)
                {
                    // Add Security Resources
                    foreach (var pageInformation in appInformation.Pages)
                    {
                        var resourceId = $"app:{appInformation.Id}:{pageInformation.Id}";

                        _instance.SecurityManager.AddResource(Security.TrakHoundIdentityResourceType.App, resourceId);
                    }
                }
            }
        }


        private void LoadPages()
        {
            var appConfigurations = _configurationProfile.Get<ITrakHoundAppConfiguration>(TrakHoundAppConfiguration.ConfigurationCategory);
            if (!appConfigurations.IsNullOrEmpty())
            {
                foreach (var appConfiguration in appConfigurations)
                {
                    _routeManager.AddApp(appConfiguration);
                }
            }
        }


        private void LoadStylesheets()
        {
            var stylesheets = new List<StylesheetInfo>();

            foreach (var appPackage in _appPackages)
            {
                var appId = appPackage.Key;
                var package = appPackage.Value;

                if (!string.IsNullOrEmpty(appId))
                {
                    var stylesheet = new StylesheetInfo();
                    stylesheet.AppId = appId;
                    stylesheet.PackageDirectory = package.Location;
                    stylesheet.PackageId = package.Id;
                    stylesheet.PackageVersion = package.Version;
                    stylesheets.Add(stylesheet);
                }
            }

            UpdateStylesheets(stylesheets);
        }

        private void LoadScripts()
        {
            var stylesheets = new List<StylesheetInfo>();

            foreach (var appPackage in _appPackages)
            {
                var appId = appPackage.Key;
                var package = appPackage.Value;

                if (!string.IsNullOrEmpty(appId))
                {
                    var stylesheet = new StylesheetInfo();
                    stylesheet.AppId = appId;
                    stylesheet.PackageDirectory = package.Location;
                    stylesheet.PackageId = package.Id;
                    stylesheet.PackageVersion = package.Version;
                    stylesheets.Add(stylesheet);
                }
            }

            UpdateScripts(stylesheets);
        }

        public void UpdateStylesheets(IEnumerable<StylesheetInfo> stylesheets)
        {
            if (!stylesheets.IsNullOrEmpty())
            {
                foreach (var stylesheet in stylesheets)
                {
                    lock (_lock)
                    {
                        _packageStylesheets.Remove(stylesheet.AppId);
                    }

                    if (!string.IsNullOrEmpty(stylesheet.PackageDirectory))
                    {
                        try
                        {
                            var stylesheetFiles = Directory.GetFiles(stylesheet.PackageDirectory, "*.css", SearchOption.AllDirectories);
                            if (!stylesheetFiles.IsNullOrEmpty())
                            {
                                var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp", "wwwroot");
                                var appRoot = Path.Combine(wwwRoot, _packagesDirectory);
                                var packageDistPath = Path.Combine(stylesheet.PackageDirectory, "dist");
                                var packageWwwRootPath = Path.Combine(packageDistPath, "wwwroot");

                                foreach (var stylesheetFile in stylesheetFiles)
                                {
                                    if (!stylesheetFile.Contains("TrakHound.Blazor"))
                                    {
                                        var destinationDirectory = Path.Combine(appRoot, stylesheet.PackageId, stylesheet.PackageVersion);
                                        if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);

                                        var relativeSourcePath = Path.GetRelativePath(packageWwwRootPath, stylesheetFile);
                                        var destinationFile = Path.Combine(destinationDirectory, relativeSourcePath);
                                        var stylesheetPath = Path.GetRelativePath(appRoot, destinationFile);

                                        var newLines = new List<string>();
                                        var contentLines = File.ReadAllLines(stylesheetFile);
                                        if (contentLines != null)
                                        {
                                            var regex = new Regex("^@import '(.*)';$");

                                            foreach (var contentLine in contentLines)
                                            {
                                                var match = regex.Match(contentLine);
                                                if (match.Success)
                                                {
                                                    var originalPath = match.Groups[1].Value;

                                                    if (!originalPath.Contains("TrakHound.Blazor"))
                                                    {
                                                        var newPath = Path.Combine(_packagesDirectory, stylesheet.PackageId, stylesheet.PackageVersion, originalPath);
                                                        newPath = newPath.Replace('\\', '/');
                                                        var newLine = $"@import '/{newPath}';";
                                                        newLines.Add(newLine);
                                                    }
                                                }
                                                else
                                                {
                                                    newLines.Add(contentLine);
                                                }
                                            }
                                        }

                                        var destinationFileDirectory = Path.GetDirectoryName(destinationFile);
                                        if (!Directory.Exists(destinationFileDirectory)) Directory.CreateDirectory(destinationFileDirectory);

                                        if (File.Exists(destinationFile)) File.Delete(destinationFile);
                                        File.WriteAllLines(destinationFile, newLines);

                                        _packageStylesheets.Add(stylesheet.AppId, stylesheetPath);
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public void UpdateScripts(IEnumerable<StylesheetInfo> stylesheets)
        {
            if (!stylesheets.IsNullOrEmpty())
            {
                foreach (var stylesheet in stylesheets)
                {
                    lock (_lock)
                    {
                        _packageScripts.Remove(stylesheet.AppId);
                    }

                    if (!string.IsNullOrEmpty(stylesheet.PackageDirectory))
                    {
                        try
                        {
                            var scriptFiles = Directory.GetFiles(stylesheet.PackageDirectory, "*.js", SearchOption.AllDirectories);
                            if (!scriptFiles.IsNullOrEmpty())
                            {
                                var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp", "wwwroot");
                                var appRoot = Path.Combine(wwwRoot, _packagesDirectory);
                                var packageDistPath = Path.Combine(stylesheet.PackageDirectory, "dist");
                                var packageWwwRootPath = Path.Combine(packageDistPath, "wwwroot");

                                foreach (var scriptFile in scriptFiles)
                                {
                                    if (!scriptFile.Contains("TrakHound.Blazor"))
                                    {
                                        var destinationDirectory = Path.Combine(appRoot, stylesheet.PackageId, stylesheet.PackageVersion);
                                        if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);

                                        var relativeSourcePath = Path.GetRelativePath(packageWwwRootPath, scriptFile);
                                        var destinationFile = Path.Combine(destinationDirectory, relativeSourcePath);
                                        var scriptPath = Path.GetRelativePath(appRoot, destinationFile);

                                        var destinationFileDirectory = Path.GetDirectoryName(destinationFile);
                                        if (!Directory.Exists(destinationFileDirectory)) Directory.CreateDirectory(destinationFileDirectory);

                                        File.Copy(scriptFile, destinationFile, true);

                                        _packageScripts.Add(stylesheet.AppId, scriptPath);
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void LoadStaticAssets()
        {
            if (_packageManager != null)
            {
                var appPackages = _packageManager.GetByCategory(_packageCategory);
                if (!appPackages.IsNullOrEmpty())
                {
                    var assets = new List<StaticAssetInfo>();

                    foreach (var package in appPackages)
                    {
                        if (!string.IsNullOrEmpty(package.Location))
                        {
                            if (!assets.Any(o => o.PackageDirectory == package.Location))
                            {
                                var asset = new StaticAssetInfo();
                                asset.PackageDirectory = package.Location;
                                asset.PackageId = package.Id;
                                asset.PackageVersion = package.Version;
                                assets.Add(asset);
                            }
                        }
                    }

                    UpdateStaticAssets(assets);
                }
            }
        }

        public void UpdateStaticAssets(IEnumerable<StaticAssetInfo> assets)
        {
            if (!assets.IsNullOrEmpty())
            {
                foreach (var asset in assets)
                {
                    if (!string.IsNullOrEmpty(asset.PackageDirectory))
                    {
                        var sourceRoot = Path.Combine(asset.PackageDirectory, "dist", "wwwroot");

                        try
                        {
                            var assetFiles = Directory.GetFiles(sourceRoot, "*.*", SearchOption.AllDirectories);
                            if (!assetFiles.IsNullOrEmpty())
                            {
                                var wwwRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_temp", "wwwroot");
                                var appRoot = Path.Combine(wwwRoot, _packagesDirectory);
                                var packageDistPath = Path.Combine(asset.PackageDirectory, "dist");
                                var packageWwwRootPath = Path.Combine(packageDistPath, "wwwroot");

                                foreach (var assetFile in assetFiles)
                                {
                                    if (Path.GetExtension(assetFile) != ".css" && Path.GetExtension(assetFile) != ".js")
                                    {
                                        if (!assetFile.Contains("TrakHound.Blazor"))
                                        {
                                            var destinationDirectory = Path.Combine(appRoot, asset.PackageId, asset.PackageVersion);
                                            if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);

                                            var destinationFile = Path.Combine(destinationDirectory, Path.GetRelativePath(sourceRoot, assetFile));
                                            var destinationSubDirectory = Path.GetDirectoryName(destinationFile);
                                            if (!Directory.Exists(destinationSubDirectory)) Directory.CreateDirectory(destinationSubDirectory);

                                            File.Copy(assetFile, destinationFile, true);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
