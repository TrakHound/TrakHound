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
                var loggerName = $"service/{serviceId}/main";
                var loggerId = _logProvider.GetLoggerId(loggerName);
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _serviceManager.ServiceLogUpdated += (id, item) =>
                {
                    var itemLoggerName = $"service/{id}/{item.Sender}";
                    var itemLoggerId = _logProvider.GetLoggerId(itemLoggerName);

                    if (itemLoggerId == loggerId && item.LogLevel <= minimumLevel)
                    {
                        consumer.Push(new TrakHoundLogItem[] { item });
                    }
                };

                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string serviceId, string logName, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(serviceId) && !string.IsNullOrEmpty(logName))
            {
                var loggerName = $"service/{serviceId}/{logName}";
                var loggerId = _logProvider.GetLoggerId(loggerName);
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _serviceManager.ServiceLogUpdated += (id, item) =>
                {
                    var itemLoggerName = $"service/{id}/{item.Sender}";
                    var itemLoggerId = _logProvider.GetLoggerId(itemLoggerName);

                    if (itemLoggerId == loggerId && item.LogLevel <= minimumLevel)
                    {
                        consumer.Push(new TrakHoundLogItem[] { item });
                    }
                };

                return consumer;
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
            var pattern = $"service/{serviceId}/*";
            var loggerIds = _logProvider.QueryLoggerIds(pattern);
            if (!loggerIds.IsNullOrEmpty())
            {
                return loggerIds.Select(o => GetLogName(serviceId, o));
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerName = $"service/{serviceId}";
            return _logProvider.QueryLogs(loggerName, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerName = $"service/{serviceId}";
            return _logProvider.QueryLogsByMinimumLevel(loggerName, minimumLevel, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerName = $"service/{serviceId}";
            return _logProvider.QueryLogsByLevel(loggerName, level, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string serviceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerName = $"service/{serviceId}/{logName}";
            return _logProvider.QueryLogs(loggerName, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string serviceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerName = $"service/{serviceId}/{logName}";
            return _logProvider.QueryLogsByMinimumLevel(loggerName, minimumLevel, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string serviceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            var loggerName = $"service/{serviceId}/{logName}";
            return _logProvider.QueryLogsByLevel(loggerName, level, from, to, skip, take, sortOrder);
        }


        private string GetLogName(string serviceId, string loggerId)
        {
            if (!string.IsNullOrEmpty(loggerId))
            {
                var prefix = _logProvider.GetLoggerId($"service/{serviceId}");
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
