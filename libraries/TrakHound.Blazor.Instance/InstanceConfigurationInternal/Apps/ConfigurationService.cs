using TrakHound.Apps;
using TrakHound.Packages;
using TrakHound.Routing;

namespace TrakHound.Blazor.Instance.InstanceConfigurationInternal.Apps
{
    public class ConfigurationService
    {
        private const string _configurationCategory = TrakHoundAppConfiguration.ConfigurationCategory;
        private const string _packageCategory = "app";

        private readonly InstanceConfiguration _instanceConfiguration;
        private IEnumerable<TableItem> _tableItems;
        private IEnumerable<TrakHoundRouterConfiguration> _routerConfigurations;


        public struct TableItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Route { get; set; }
            public string VolumeId { get; set; }
            public string RouterId { get; set; }
            public string PackageId { get; set; }
            public string PackageVersion { get; set; }
            public bool AutoUpdate { get; set; }
            public ITrakHoundAppConfiguration Configuration { get; set; }
        }

        public struct Configuration
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Route { get; set; }
            public string VolumeId { get; set; }
            public string RouterId { get; set; }
            public string PackageId { get; set; }
            public string PackageVersion { get; set; }
            public bool AutoUpdate { get; set; }


            private Dictionary<string, object> _parameters;
            public Dictionary<string, object> Parameters
            {
                get
                {
                    if (_parameters == null) _parameters = new Dictionary<string, object>();
                    return _parameters;
                }
                set => _parameters = value;
            }

            public Configuration()
            {
                RouterId = TrakHoundRouter.Default;
                AutoUpdate = true;
            }
        }

        public struct ConfigurationAddResult
        {
            public bool IsNew { get; set; }
            public string ConfigurationId { get; set; }
        }


        public IEnumerable<TableItem> TableItems => _tableItems;

        public IEnumerable<TrakHoundRouterConfiguration> RouterConfigurations => _routerConfigurations;

        public Configuration AddConfiguration;

        public string AddPackageQuery;


        public ConfigurationService(InstanceConfiguration instanceConfiguration)
        {
            _instanceConfiguration = instanceConfiguration;
        }


        public void Load()
        {
            var configurationProfile = _instanceConfiguration.ConfigurationGroup.ConfigurationProfile;
            if (configurationProfile != null)
            {
                _routerConfigurations = configurationProfile.Get<TrakHoundRouterConfiguration>(TrakHoundRouterConfiguration.ConfigurationCategory);

                var items = new List<TableItem>();

                var configurations = configurationProfile.Get<TrakHoundAppConfiguration>(_configurationCategory);
                if (!configurations.IsNullOrEmpty())
                {
                    foreach (var configuration in configurations)
                    {
                        var item = new TableItem();
                        item.Id = configuration.Id;
                        item.Name = configuration.Name;
                        item.Route = configuration.Route;
                        item.VolumeId = configuration.VolumeId;
                        item.RouterId = configuration.RouterId;
                        item.PackageId = configuration.PackageId;
                        item.PackageVersion = configuration.PackageVersion;
                        item.AutoUpdate = configuration.PackageVersion != null && configuration.PackageVersion.Contains("*");
                        item.Configuration = configuration;

                        items.Add(item);
                    }
                }

                _tableItems = items.OrderBy(o => o.Route);
            }
        }

        public async Task<ConfigurationAddResult> CreateConfiguration()
        {
            var result = new ConfigurationAddResult();
            result.IsNew = string.IsNullOrEmpty(AddConfiguration.Id);

            // Create Configuration
            var createConfiguration = new TrakHoundAppConfiguration();
            createConfiguration.Id = !string.IsNullOrEmpty(AddConfiguration.Id) ? AddConfiguration.Id : Guid.NewGuid().ToString();
            createConfiguration.Name = AddConfiguration.Name;
            createConfiguration.Route = AddConfiguration.Route;
            createConfiguration.VolumeId = AddConfiguration.VolumeId;
            createConfiguration.RouterId = AddConfiguration.RouterId;
            createConfiguration.PackageId = AddConfiguration.PackageId;
            createConfiguration.PackageVersion = GetPackageVersion(AddConfiguration.PackageVersion, AddConfiguration.AutoUpdate);

            // Set Parameters
            var parameters = new Dictionary<string, object>();
            if (AddConfiguration.Parameters != null && !AddConfiguration.Parameters.IsNullOrEmpty())
            {
                foreach (var property in AddConfiguration.Parameters)
                {
                    parameters.Add(property.Key, property.Value);
                }
            }
            createConfiguration.Parameters = parameters;

            // Add to Configuration Profile
            _instanceConfiguration.ConfigurationGroup.ConfigurationProfile.Add(createConfiguration, true);

            var packageId = AddConfiguration.PackageId;
            var packageVersion = GetPackageVersion(AddConfiguration.PackageVersion, AddConfiguration.AutoUpdate);

            // Install Package
            var existingPackage = _instanceConfiguration.PackageManager.Get(packageId, packageVersion);
            if (existingPackage == null)
            {
                var packageBytes = await _instanceConfiguration.ManagementClient.Packages.Download(packageId, packageVersion);
                await _instanceConfiguration.PackageManager.Install(packageBytes);
            }

            // Add Package to Configuration
            var packageItem = new TrakHoundPackagesFileItem();
            packageItem.Category = _packageCategory;
            packageItem.Id = AddConfiguration.PackageId;
            packageItem.Version = GetPackageVersion(AddConfiguration.PackageVersion, AddConfiguration.AutoUpdate);
            _instanceConfiguration.ConfigurationGroup.AddPackage(packageItem);

            AddConfiguration.Id = null;
            AddConfiguration.Name = null;
            AddConfiguration.PackageId = null;
            AddConfiguration.PackageVersion = null;
            AddConfiguration.Route = null;
            AddConfiguration.VolumeId = null;
            AddConfiguration.RouterId = TrakHoundRouter.Default;
            AddConfiguration.AutoUpdate = false;
            AddConfiguration.Parameters = null;
            AddPackageQuery = null;

            result.ConfigurationId = createConfiguration.Id;
            return result;
        }

        public void EditConfiguration(string configurationId)
        {
            var configurationProfile = _instanceConfiguration.ConfigurationGroup.ConfigurationProfile;
            if (configurationProfile != null)
            {
                var configuration = configurationProfile.Get<TrakHoundAppConfiguration>(_configurationCategory, configurationId);
                if (configuration != null)
                {
                    AddConfiguration.Id = configuration.Id;
                    AddConfiguration.Name = configuration.Name;
                    AddConfiguration.Route = configuration.Route;
                    AddConfiguration.RouterId = configuration.RouterId;
                    AddConfiguration.AutoUpdate = configuration.PackageVersion != null && configuration.PackageVersion.Contains(TrakHoundPackage.WilcardVersion);

                    AddPackageQuery = configuration.PackageId;
                    AddConfiguration.PackageId = configuration.PackageId;
                    AddConfiguration.PackageVersion = configuration.PackageVersion;

                    //if (!configuration.Parameters.IsNullOrEmpty())
                    //{
                    //    var parameters = new Dictionary<string, object>();
                    //    foreach (var parameter in configuration.Parameters)
                    //    {
                    //        parameters.Add(parameter.Key, parameter.Value);
                    //    }
                    //    AddConfiguration.Parameters = parameters;
                    //}
                }
            }
        }

        public void RemoveConfiguration(string configurationId)
        {
            var configurationProfile = _instanceConfiguration.ConfigurationGroup.ConfigurationProfile;
            if (configurationProfile != null)
            {
                var configuration = configurationProfile.Get<TrakHoundAppConfiguration>(_configurationCategory, configurationId);
                if (configuration != null)
                {
                    _instanceConfiguration.ConfigurationGroup.ConfigurationProfile.Remove(_configurationCategory, configuration.Id);
                    _instanceConfiguration.ConfigurationGroup.RemovePackage(configuration.PackageId, configuration.PackageVersion);
                }
            }
        }

        private static string GetPackageVersion(string packageVersion, bool autoUpdate)
        {
            if (packageVersion != null)
            {
                return autoUpdate ? TrakHoundPackage.WilcardVersion : packageVersion;
            }

            return null;
        }
    }
}
