// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Configurations;
using TrakHound.Volumes;

namespace TrakHound.Drivers
{
    /// <summary>
    /// Used to access external Infrastructure
    /// </summary>
    public interface ITrakHoundDriver : IDisposable
    {
        ITrakHoundDriverConfiguration Configuration { get; }

        string Id { get; }

        string Name { get; }

        bool IsAvailable { get; }

        string AvailabilityMessage { get; }

        ITrakHoundVolume Volume { get; }
    }
}
