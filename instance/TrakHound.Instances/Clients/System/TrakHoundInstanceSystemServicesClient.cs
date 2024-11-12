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


        public TrakHoundInstanceSystemServicesClient(TrakHoundServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
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
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _serviceManager.ServiceLogUpdated += (id, item) =>
                {
                    if (id == serviceId)
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
    }
}
