// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using TrakHound.Volumes;

namespace TrakHound.Drivers
{
    /// <summary>
    /// Used to access external Infrastructure
    /// </summary>
    public interface ITrakHoundDriver : IDisposable
    {
        string Id { get; }

        string Name { get; }

        bool IsAvailable { get; }

        string AvailabilityMessage { get; }

        ITrakHoundDriverConfiguration Configuration { get; }

        ITrakHoundVolume Volume { get; }
    }
}
