// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectReferenceClient : ITrakHoundEntityClient<ITrakHoundObjectReferenceEntity>
    {
        Task<IEnumerable<ITrakHoundObjectReferenceEntity>> QueryByObject(
            string path,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectReferenceEntity>> QueryByObject(
            IEnumerable<string> paths,
            string routerId = null);

        Task<ITrakHoundObjectReferenceEntity> QueryByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectReferenceEntity>> QueryByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>> SubscribeByObject(
            IEnumerable<string> paths,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

        Task<ITrakHoundConsumer<IEnumerable<ITrakHoundObjectReferenceEntity>>> SubscribeByObjectUuid(
            IEnumerable<string> objectUuids,
            int interval = 0,
            int take = 1000,
            string consumerId = null,
            string routerId = null);

    }
}