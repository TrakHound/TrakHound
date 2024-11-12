// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Apps;
using TrakHound.Services;

namespace TrakHound.Blazor.ExplorerInternal.Services
{
    public class ExplorerTreeViewService : ITrakHoundScopedAppInjectionService
    {
        private readonly ExplorerService _explorerService;
        private readonly HashSet<string> _expanded = new HashSet<string>();
        private readonly Dictionary<string, IEnumerable<TrakHoundServiceLogInformation>> _serviceLogs = new Dictionary<string, IEnumerable<TrakHoundServiceLogInformation>>();
        private readonly object _lock = new object();


        public ExplorerTreeViewService(ExplorerService explorerService)
        {
            _explorerService = explorerService;
        }


        public void CollapseAll()
        {
            lock (_lock) _expanded.Clear();
        }

        public void ExpandAll()
        {

        }

        public bool IsExpanded(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                lock (_lock)
                {
                    return _expanded.Contains(path);
                }
            }

            return false;
        }

        public void SetExpanded(string path, bool expanded)
        {
            if (!string.IsNullOrEmpty(path))
            {
                lock (_lock)
                {
                    if (expanded)
                    {
                        if (!_expanded.Contains(path)) _expanded.Add(path);
                    }
                    else
                    {
                        _expanded.Remove(path);
                    }
                }
            }
        }


        public IEnumerable<TrakHoundServiceLogInformation> GetServiceLogs(string serviceId)
        {
            if (!string.IsNullOrEmpty(serviceId))
            {
                IEnumerable<TrakHoundServiceLogInformation> serviceLogs;
                lock (_lock) serviceLogs = _serviceLogs.GetValueOrDefault(serviceId);
                return serviceLogs;
            }

            return null;
        }

        public async Task LoadServiceLogs(string instanceId, string serviceId)
        {
            var client = _explorerService.GetClient(instanceId, null);
            if (client != null)
            {
                var serviceLogs = await client.Services.GetLogs(serviceId);
                if (!serviceLogs.IsNullOrEmpty())
                {
                    lock (_lock)
                    {
                        _serviceLogs.Remove(serviceId);
                        _serviceLogs.Add(serviceId, serviceLogs);
                    }
                }
            }
        }
    }
}
