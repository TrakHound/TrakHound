// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Configurations;

namespace TrakHound.Services
{
    public interface ITrakHoundServiceConfiguration : ITrakHoundConfiguration
    {
        string Description { get; }

        string PackageId { get; }

        string PackageVersion { get; }


        string VolumeId { get; }

        string RouterId { get; }

        Dictionary<string, object> Parameters { get; }


        string GetParameter(string name);

        T GetParameter<T>(string name);

        void SetParameter(string name, object value);
    }
}
