// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectObservationLatestDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectObservationEntity>> Latest(IEnumerable<string> objectUuids);
    }
}
