// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Drivers;

namespace TrakHound.Clients
{
    public interface ITrakHoundSystemDriversClient
    {
        Task<IEnumerable<TrakHoundDriverInformation>> GetDrivers();

        Task<TrakHoundDriverInformation> GetDriver(string configurationId);
    }
}
