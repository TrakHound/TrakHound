// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectStringClient : ITrakHoundEntityClient<ITrakHoundObjectStringEntity>
    {
        Task<IEnumerable<ITrakHoundObjectStringEntity>> QueryByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStringEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<ITrakHoundObjectStringEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectStringEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStringEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectStringEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}