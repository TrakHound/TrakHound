// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Apps;
using TrakHound.Logging;

namespace TrakHound.Clients
{
    public partial class TrakHoundInstanceAppsClient : ITrakHoundAppsClient
    {
        private readonly ITrakHoundAppProvider _appProvider;


        public TrakHoundInstanceAppsClient(ITrakHoundAppProvider appProvider)
        {
            _appProvider = appProvider;
        }


        public async Task<IEnumerable<TrakHoundAppInformation>> GetInformation()
        {
            return _appProvider.GetInformation();
        }

        public async Task<TrakHoundAppInformation> GetInformation(string apiId)
        {
            return _appProvider.GetInformation(apiId);
        }


        public async Task<ITrakHoundConsumer<IEnumerable<TrakHoundLogItem>>> SubscribeToLog(string appId, TrakHoundLogLevel minimumLevel = TrakHoundLogLevel.Information)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                var consumer = new TrakHoundInstanceConsumer<IEnumerable<TrakHoundLogItem>>();

                _appProvider.AppLogUpdated += (id, item) =>
                {
                    if (id == appId)
                    {
                        consumer.Push(new TrakHoundLogItem[] { item });
                    }
                };

                return consumer;
            }

            return null;
        }
    }
}
