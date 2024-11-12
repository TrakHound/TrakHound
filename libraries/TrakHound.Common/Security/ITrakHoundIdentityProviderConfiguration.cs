// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Configurations;

namespace TrakHound.Security
{
    public interface ITrakHoundIdentityProviderConfiguration : ITrakHoundConfiguration
    {
        string Description { get; }

        string VolumeId { get; }

        string PackageId { get; }

        string PackageVersion { get; }

        Dictionary<string, object> Parameters { get; }


        bool ParameterExists(string name);

        string GetParameter(string name);

        T GetParameter<T>(string name);

        void SetParameter(string name, object value);
    }
}
