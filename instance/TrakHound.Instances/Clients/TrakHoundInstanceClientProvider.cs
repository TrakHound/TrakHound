// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Apps;
using TrakHound.Drivers;
using TrakHound.Functions;
using TrakHound.Instances;
using TrakHound.Routing;
using TrakHound.Services;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceClientProvider : TrakHoundClientProviderBase, ITrakHoundClientProvider
    {
        private readonly ITrakHoundInstance _hostInstance;
        private readonly TrakHoundDriverProvider _driverProvider;
        private readonly TrakHoundRouterProvider _routerProvider;


        public ITrakHoundApiProvider ApiProvider { get; set; }

        public ITrakHoundAppProvider AppProvider { get; set; }

        public TrakHoundFunctionManager FunctionManager { get; set; }

        public TrakHoundServiceManager ServiceManager { get; set; }


        public TrakHoundInstanceClientProvider(ITrakHoundInstance hostInstance, TrakHoundDriverProvider driverProvider, TrakHoundRouterProvider routerProvider)
        {
            _hostInstance = hostInstance;
            _driverProvider = driverProvider; 
            _routerProvider = routerProvider; 
        }


        public ITrakHoundClient GetClient()
        {
            var client = new TrakHoundInstanceClient(_hostInstance, _driverProvider, _routerProvider, ApiProvider, AppProvider, FunctionManager, ServiceManager);
            foreach (var middleware in Middleware)
            {
                
            }
            return client;
        }


        public async Task<IEnumerable<ITrakHoundRouterInformation>> GetRouters()
        {
            return _routerProvider.GetInformation();
        }
    }
}
