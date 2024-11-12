// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectNumberClient : ITrakHoundEntityClient<ITrakHoundObjectNumberEntity>
    {
        Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<ITrakHoundObjectNumberEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            string path,
            double minimum,
            double maximum,
            long objectSkip,
            long objectTake,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObject(
            IEnumerable<string> paths,
            double minimum,
            double maximum,
            long objectSkip,
            long objectTake,
            string routerId = null);

        Task<ITrakHoundObjectNumberEntity> QueryByObjectUuid(
            string objectUuid,
            double minimum,
            double maximum,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectNumberEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            double minimum,
            double maximum,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}