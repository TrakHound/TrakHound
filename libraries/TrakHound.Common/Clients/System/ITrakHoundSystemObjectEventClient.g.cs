// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectEventClient : ITrakHoundEntityClient<ITrakHoundObjectEventEntity>
    {
        Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> LatestByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObject(
            string path,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObject(
            IEnumerable<string> paths,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObjectUuid(
            string objectUuid,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectEventEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectEventEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

    }
}