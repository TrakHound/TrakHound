// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Buffers;
using TrakHound.Configurations;

namespace TrakHound.Drivers
{
    public interface ITrakHoundDriverConfiguration : ITrakHoundConfiguration
    {
        string Name { get; }

        string Description { get; }

        string PackageId { get; }

        string PackageVersion { get; }

        string VolumeId { get; }

        Dictionary<string, object> Parameters { get; }

        TrakHoundBufferConfiguration Buffer { get; }


        bool ParameterExists(string name);

        string GetParameter(string name);

        T GetParameter<T>(string name);

        void SetParameter(string name, object value);
    }
}
