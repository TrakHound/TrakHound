// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    public interface IObjectQueueQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByQueue(IEnumerable<string> queueUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending);

        Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> QueryByMember(IEnumerable<string> memberUuids, long skip = 0, long take = 0, SortOrder sortOrder = SortOrder.Ascending);
    }

    public interface IObjectQueuePullDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectQueueEntity>> Pull(string queueUuid, int count = 1);
    }
}
