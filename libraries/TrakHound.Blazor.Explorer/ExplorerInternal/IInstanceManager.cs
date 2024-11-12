// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Clients;
using TrakHound.Instances;

namespace TrakHound.Blazor.ExplorerInternal
{
    public interface IInstanceManager
    {
        IEnumerable<TrakHoundInstanceInformation> GetInstances();

        TrakHoundInstanceInformation GetInstance(string instanceId);

        ITrakHoundClient GetClient(string instanceId);
    }
}
