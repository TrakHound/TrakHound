// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectDurationClient : ITrakHoundEntityClient<ITrakHoundObjectDurationEntity>
    {
        Task<IEnumerable<ITrakHoundObjectDurationEntity>> QueryByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectDurationEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<ITrakHoundObjectDurationEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectDurationEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectDurationEntity>> QueryByObject(
            string path,
            long minimum,
            long maximum,
            long objectSkip,
            long objectTake,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectDurationEntity>> QueryByObject(
            IEnumerable<string> paths,
            long minimum,
            long maximum,
            long objectSkip,
            long objectTake,
            string routerId = null);

        Task<ITrakHoundObjectDurationEntity> QueryByObjectUuid(
            string objectUuid,
            long minimum,
            long maximum,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectDurationEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            long minimum,
            long maximum,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectDurationEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}