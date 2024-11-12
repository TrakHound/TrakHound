// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectSubscribeDriver : IEntitySubscribeDriver<ITrakHoundObjectEntity>
    {
        Task<TrakHoundResponse<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEntity>>>> Subscribe(IEnumerable<string> uuids);
    }
}
