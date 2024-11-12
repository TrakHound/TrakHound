// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using TrakHound.Configurations;
using YamlDotNet.Serialization;

namespace TrakHound.Routing
{
    /// <summary>
    /// A Configuration used to configure an ApiRouter
    /// </summary>
    public class TrakHoundRouterConfiguration : ITrakHoundConfiguration
    {
        public const string ConfigurationCategory = "router";


        /// <summary>
        /// A unique Identifier for the Router
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A fiendly identifier for the Router
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the Router
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Configurations used to configure Routes to Drivers
        /// </summary>
        public IEnumerable<TrakHoundRouteConfiguration> Routes { get; set; }


        [YamlIgnore]
        public string Category => ConfigurationCategory;

        [YamlIgnore]
        public string Path { get; set; }

        [YamlIgnore]
        public string Hash => GenerateHash();


        private string GenerateHash()
        {
            return $"{Id}:{Name}:{Description}:{Routes?.ToJson()}".ToSHA256Hash();
        }


        public void AddRoute(TrakHoundRouteConfiguration configuration)
        {
            if (configuration != null)
            {
                var routers = Routes?.ToList();
                if (routers == null) routers = new List<TrakHoundRouteConfiguration>();
                routers.Add(configuration);
                Routes = routers;
            }
        }

        public void RemoveRoute(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Routes.IsNullOrEmpty())
            {
                var routers = Routes.ToList();
                routers.RemoveAll(o => o.Id == id);
                if (routers.Count > 0)
                {
                    Routes = routers;
                }
                else Routes = null;
            }
        }

        public void RemoveTarget(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Routes.IsNullOrEmpty())
            {
                foreach (var route in Routes)
                {
                    route.RemoveTarget(id);
                }
            }
        }

        public void RemoveRedirect(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Routes.IsNullOrEmpty())
            {
                foreach (var route in Routes)
                {
                    route.RemoveRedirect(id);
                }
            }
        }
    }
}
