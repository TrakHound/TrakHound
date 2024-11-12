// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectStatisticDeleteDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids);
    }
}
