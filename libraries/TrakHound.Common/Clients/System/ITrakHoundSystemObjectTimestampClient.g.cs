// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectTimestampClient : ITrakHoundEntityClient<ITrakHoundObjectTimestampEntity>
    {
        Task<IEnumerable<ITrakHoundObjectTimestampEntity>> QueryByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectTimestampEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<ITrakHoundObjectTimestampEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectTimestampEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectTimestampEntity>> QueryByObject(
            string path,
            long from,
            long to,
            long objectSkip,
            long objectTake,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectTimestampEntity>> QueryByObject(
            IEnumerable<string> paths,
            long from,
            long to,
            long objectSkip,
            long objectTake,
            string routerId = null);

        Task<ITrakHoundObjectTimestampEntity> QueryByObjectUuid(
            string objectUuid,
            long from,
            long to,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectTimestampEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long from,
            long to,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectTimestampEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}