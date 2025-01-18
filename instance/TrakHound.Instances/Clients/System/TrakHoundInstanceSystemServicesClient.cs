// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Logging;
using TrakHound.Services;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemServicesClient : ITrakHoundSystemServicesClient
    {
        private readonly TrakHoundServiceManager _serviceManager;
        private readonly ITrakHoundLogProvider _logProvider;


        public TrakHoundInstanceSystemServicesClient(TrakHoundServiceManager serviceManager, ITrakHoundLogProvider logProvider)
        {
            _serviceManager = serviceManager;
            _logProvider = logProvider;
        }


        public async Task<IEnumerable<TrakHoundServiceInformation>> GetInformation()
        {
            return _serviceManager.GetInformation();
        }

        public async Task<TrakHoundServiceInformation> GetInformation(string serviceId)
        {
            return _serviceManager.GetInformation(serviceId);
        }

        public async Task<IEnumerable<TrakHoundServiceInformation>> GetInformation(IEnumerable<string> serviceIds)
        {
            return _serviceManager.GetInformation(serviceIds);
        }

        public async Task<ITrakHoundConsumer<TrakHoundServiceInformation>> Subscribe()
        {
            var consumer = new TrakHoundInstanceConsumer<TrakHoundServiceInformation>();

            _serviceManager.ServiceStatusChanged += async (id, status) =>
            {
                var information = await GetInformation(id);
                consumer.Push(information);
            };

            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundServiceInformation>> Subscribe(string serviceId)
        {
            if (!string.IsNullOrEmpty(serviceId))
            {
                var consumer = new TrakHoundInstanceConsumer<TrakHoundServiceInformation>();

                _serviceManager.ServiceStatusChanged += async (id, status) =>
                {
                    if (id == serviceId)
                    {
                        var information = await GetInformation(serviceId);
                        consumer.Push(information);
                    }
                };

                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(serviceId))
            {
                var service = _serviceManager.GetEngine(serviceId);
                if (service != null)
                {
                    var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                    var logFilter = $"services/{service.PackageId}/{serviceId}/*";
                    _logProvider.LogEntryReceived += (id, item) =>
                    {
                        if (IsMatch(logFilter, item.Sender) && item.LogLevel <= minimumLevel)
                        {
                            consumer.Push(new TrakHoundLogItem[] { item });
                        }
                    };

                    return consumer;
                }
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, string filter, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(serviceId) && !string.IsNullOrEmpty(filter))
            {
                var service = _serviceManager.GetEngine(serviceId);
                if (service != null)
                {
                    var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                    var logFilter = $"services/{service.PackageId}/{serviceId}/{filter}";
                    _logProvider.LogEntryReceived += (id, item) =>
                    {
                        if (IsMatch(logFilter, item.Sender) && item.LogLevel <= minimumLevel)
                        {
                            consumer.Push(new TrakHoundLogItem[] { item });
                        }
                    };

                    return consumer;
                }
            }

            return null;
        }


        public async Task<IEnumerable<string>> Retrieve(int skip = 0, int take = 100, SortOrder sortOrder = SortOrder.Ascending)
		{
			return _serviceManager.Engines?.Select(o => o.EngineId);
		}

		public async Task<IEnumerable<TrakHoundServiceStatus>> GetStatus(IEnumerable<string> serviceIds)
        {
            if (!serviceIds.IsNullOrEmpty())
            {
                var engines = _serviceManager.Engines?.Where(o => serviceIds.Contains(o.EngineId));
				if (!engines.IsNullOrEmpty())
				{
					var statuses = new List<TrakHoundServiceStatus>();
					foreach (var engine in engines)
					{
						statuses.Add(new TrakHoundServiceStatus(engine.EngineId, engine.Status, null));
					}
					return statuses;
				}
			}

			return null;
        }

        public async Task<TrakHoundServiceStatus> StartService(string serviceId)
        {
            if (!string.IsNullOrEmpty(serviceId))
            {
				var engine = _serviceManager.GetEngine(serviceId);
				if (engine != null)
				{
					engine.Start();
					return new TrakHoundServiceStatus(engine.EngineId, engine.Status, null);
				}
            }

            return default;
        }

        public async Task<TrakHoundServiceStatus> StopService(string serviceId)
        {
            if (!string.IsNullOrEmpty(serviceId))
            {
                var engine = _serviceManager.GetEngine(serviceId);
                if (engine != null)
                {
                    await engine.Stop();
                    return new TrakHoundServiceStatus(engine.EngineId, engine.Status, null);
                }
            }

            return default;
        }


        public async Task<IEnumerable<string>> QueryLogNames(string serviceId)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var pattern = $"services/{service.PackageId}/{serviceId}/*";
                var loggerIds = _logProvider.QueryLoggerIds(pattern);
                if (!loggerIds.IsNullOrEmpty())
                {
                    return loggerIds.Select(o => GetLogName(service.PackageId, serviceId, o));
                }

                //var results = new List<string>();
                //results.Add($"services/{service.PackageId}/{serviceId}");

                //var pattern = $"services/{service.PackageId}/{serviceId}/*";
                //var loggerIds = _logProvider.QueryLoggerIds(pattern);
                //if (!loggerIds.IsNullOrEmpty())
                //{
                //    results.AddRange(loggerIds.Select(o => GetLogName(service.PackageId, serviceId, o)));
                //}

                //return results;
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var loggerName = $"services/{service.PackageId}/{serviceId}/*";
                return _logProvider.QueryLogs(loggerName, from, to, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var loggerName = $"services/{service.PackageId}/{serviceId}/*";
                return _logProvider.QueryLogsByMinimumLevel(loggerName, minimumLevel, from, to, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var loggerName = $"services/{service.PackageId}/{serviceId}/*";
                return _logProvider.QueryLogsByLevel(loggerName, level, from, to, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var loggerName = $"services/{service.PackageId}/{serviceId}/{logName}/*";
                return _logProvider.QueryLogs(loggerName, from, to, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var loggerName = $"services/{service.PackageId}/{serviceId}/{logName}/*";
                return _logProvider.QueryLogsByMinimumLevel(loggerName, minimumLevel, from, to, skip, take, sortOrder);
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var service = _serviceManager.GetEngine(serviceId);
            if (service != null)
            {
                var loggerName = $"services/{service.PackageId}/{serviceId}/{logName}/*";
                return _logProvider.QueryLogsByLevel(loggerName, level, from, to, skip, take, sortOrder);
            }

            return null;
        }


        private bool IsMatch(string pattern, string loggerId)
        {
            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(loggerId))
            {
                var loggerPattern = _logProvider.GetLoggerId(pattern);

                if (loggerPattern.EndsWith('*'))
                {
                    return loggerId.StartsWith(loggerPattern.TrimEnd('*').TrimEnd('/'));
                }
                else
                {
                    return loggerId == pattern;
                }
            }

            return false;
        }

        private string GetLogName(string packageId, string serviceId, string loggerId)
        {
            if (!string.IsNullOrEmpty(packageId) && !string.IsNullOrEmpty(loggerId))
            {
                var prefix = _logProvider.GetLoggerId($"services/{packageId}/{serviceId}");
                if (loggerId != prefix)
                {
                    return loggerId.Substring(prefix.Length + 1);
                }
                else
                {
                    return "main";
                }
            }

            return null;
        }
    }
}
