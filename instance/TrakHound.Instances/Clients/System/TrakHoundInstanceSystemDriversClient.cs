// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Drivers;

namespace TrakHound.Clients
{
    public class TrakHoundInstanceSystemDriversClient : ITrakHoundSystemDriversClient
    {
        private readonly TrakHoundDriverProvider _driverProvider;


        public TrakHoundInstanceSystemDriversClient(TrakHoundDriverProvider driverProvider)
        {
            _driverProvider = driverProvider;
        }


		public async Task<IEnumerable<TrakHoundDriverInformation>> GetDrivers()
        {
            var informations = _driverProvider.GetInformation();
            if (!informations.IsNullOrEmpty())
            {
                var results = new List<TrakHoundDriverInformation>();
                foreach (var information in informations)
                {
                    results.Add(information);
                }
                return results;
            }

            return null;
        }

        public async Task<TrakHoundDriverInformation> GetDriver(string configurationId)
        {
            return _driverProvider.GetInformation(configurationId);
        }
    }
}
