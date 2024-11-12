// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Buffers;
using TrakHound.Instances;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemInstancesClient : ITrakHoundSystemInstancesClient
    {
        private readonly ITrakHoundInstance _instance;


        public TrakHoundInstanceSystemInstancesClient(ITrakHoundInstance instance)
        {
            _instance = instance;
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
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _instance.LogUpdated += (id, item) =>
                {
                    consumer.Push(new TrakHoundLogItem[] { item });
                };

                return consumer;
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
