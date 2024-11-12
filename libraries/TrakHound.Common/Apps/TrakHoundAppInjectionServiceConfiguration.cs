// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Clients;
using TrakHound.Volumes;

namespace TrakHound.Apps
{
    public class TrakHoundAppInjectionServiceConfiguration
    {
        public string Id { get; set; }

        public ITrakHoundAppConfiguration Configuration { get; set; }

        public ITrakHoundClient Client { get; set; }

        public ITrakHoundVolume Volume { get; set; }

        public Type ServiceType { get; set; }


        public TrakHoundAppInjectionServiceConfiguration(
            ITrakHoundAppConfiguration configuration,
            ITrakHoundClient client,
            ITrakHoundVolume volume, 
            Type serviceType
            )
        {
            Id = configuration?.Id;
            Configuration = configuration;
            Client = client;
            Volume = volume;
            ServiceType = serviceType;
        }


        public static string CreateId(ITrakHoundAppConfiguration configuration, Type serviceType)
        {
            if (configuration != null && serviceType != null && serviceType.Assembly != null)
            {
                return $"{serviceType.Assembly.Location}:{serviceType.FullName}".ToMD5Hash();
            }

            return null;
        }
    }
}
