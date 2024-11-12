// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectLogClient : ITrakHoundEntityClient<ITrakHoundObjectLogEntity>
    {
        Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObject(
            string path,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            string objectUuid,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectLogEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundLogLevel minLevel,
            long start,
            long stop,
            long skip = 0,
            long take = 1000,
            SortOrder sortOrder = TrakHound.SortOrder.Ascending,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            TrakHound.TrakHoundLogLevel minLevel,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectLogEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            TrakHound.TrakHoundLogLevel minLevel,
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