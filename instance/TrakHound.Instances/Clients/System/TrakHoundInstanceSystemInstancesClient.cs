// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Buffers;
using TrakHound.Instances;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemInstancesClient : ITrakHoundSystemInstancesClient
    {
        private readonly ITrakHoundInstance _instance;
        private readonly ITrakHoundLogProvider _logProvider;


        public TrakHoundInstanceSystemInstancesClient(ITrakHoundInstance instance, ITrakHoundLogProvider logProvider)
        {
            _instance = instance;
            _logProvider = logProvider;
        }


		public async Task<TrakHoundInstanceInformation> GetHostInformation()
        {
            return null;
        }


        public async Task Start(string instanceId)
        {
            if (instanceId == _instance.Id)
            {
                await _instance.Start();
            }
        }

        public async Task Stop(string instanceId)
        {
            if (instanceId == _instance.Id)
            {
                await _instance.Stop();
            }
        }


        public async Task<ITrakHoundConsumer<TrakHoundInstanceInformation>> Subscribe()
        {
            var consumer = new TrakHoundInstanceConsumer<TrakHoundInstanceInformation>();

            _instance.StatusUpdated += (id, status) =>
            {
                consumer.Push(_instance.Information);
            };

            return consumer;
        }

        public async Task<ITrakHoundConsumer<TrakHoundInstanceInformation>> Subscribe(string instanceId)
        {
            if (!string.IsNullOrEmpty(instanceId) && instanceId == _instance.Id)
            {
                var consumer = new TrakHoundInstanceConsumer<TrakHoundInstanceInformation>();

                _instance.StatusUpdated += (id, status) =>
                {
                    consumer.Push(_instance.Information);
                };

                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string instanceId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(instanceId) && instanceId == _instance.Id)
            {
                var loggerId = _logProvider.GetLoggerId("instance");
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _instance.LogUpdated += (id, item) =>
                {
                    if (item.Sender == loggerId && item.LogLevel <= minimumLevel)
                    {
                        consumer.Push(new TrakHoundLogItem[] { item });
                    }
                };

                return consumer;
            }

            return null;
        }

        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string instanceId, string logName, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(instanceId) && instanceId == _instance.Id && !string.IsNullOrEmpty(logName))
            {
                var loggerId = _logProvider.GetLoggerId(logName);
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _instance.LogUpdated += (id, item) =>
                {
                    if (item.Sender == loggerId && item.LogLevel <= minimumLevel)
                    {
                        consumer.Push(new TrakHoundLogItem[] { item });
                    }
                };

                return consumer;
            }

            return null;
        }


        public async Task<IEnumerable<string>> QueryLogNames(string instanceId)
        {
            var loggerIds = _logProvider.QueryLoggerIds("*");
            if (!loggerIds.IsNullOrEmpty())
            {
                return loggerIds.Select(o => GetLogName(instanceId, o));
            }

            return null;
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string instanceId, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _logProvider.QueryLogs("instance", from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string instanceId, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _logProvider.QueryLogsByMinimumLevel("instance", minimumLevel, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string instanceId, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _logProvider.QueryLogsByLevel("instance", level, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogs(string instanceId, string logName, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _logProvider.QueryLogs(logName, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByMinimumLevel(string instanceId, string logName, TrakHoundLogLevel minimumLevel, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _logProvider.QueryLogsByMinimumLevel(logName, minimumLevel, from, to, skip, take, sortOrder);
        }

        public async Task<IEnumerable<TrakHoundLogItem>> QueryLogsByLevel(string instanceId, string logName, TrakHoundLogLevel level, DateTime from, DateTime to, int skip = 0, int take = 1000, SortOrder sortOrder = SortOrder.Ascending)
        {
            return _logProvider.QueryLogsByLevel(logName, level, from, to, skip, take, sortOrder);
        }


        private string GetLogName(string instanceId, string loggerId)
        {
            if (!string.IsNullOrEmpty(loggerId))
            {
                var prefix = _logProvider.GetLoggerId();
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

        private static string GetLoggerId(string instanceId, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var prefix = $"trakhound.instance.{instanceId}";
                if (name != "main")
                {
                    return $"{prefix}.{name}";
                }
                else
                {
                    return prefix;
                }
            }

            return null;
        }



        public async Task<IEnumerable<TrakHoundBufferMetrics>> GetBufferMetrics()
        {
            return _instance.BufferProvider.GetBufferMetrics();        
        }

        public async Task<TrakHoundBufferMetrics> GetBufferMetrics(string bufferId)
        {
            return _instance.BufferProvider.GetBufferMetrics(bufferId);
        }
    }
}
