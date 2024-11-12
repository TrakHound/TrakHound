// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Blazor.ExplorerInternal;
using TrakHound.Clients;
using TrakHound.Instances;

namespace TrakHound.Instance.Pages.Explorer
{
    public class InstanceManager : IInstanceManager
    {
        private readonly ITrakHoundInstance _instance;


        public InstanceManager(ITrakHoundInstance instance)
        {
            _instance = instance;
        }


        public IEnumerable<TrakHoundInstanceInformation> GetInstances()
        {
            return new TrakHoundInstanceInformation[] { _instance.Information };
        }

        public TrakHoundInstanceInformation GetInstance(string instanceId)
        {
            if (instanceId == _instance.Information.Id) return _instance.Information;
            return null;
        }

        public ITrakHoundClient GetClient(string instanceId)
        {
            var instance = GetInstance(instanceId);
            if (instance != null)
            {
                var client = _instance.ClientProvider.GetClient();
                client.AddMiddleware(new TrakHoundSourceMiddleware());
                return client;
            }

            return null;
        }
    }
}
