// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemObjectMetadataClient : ITrakHoundEntityClient<ITrakHoundObjectMetadataEntity>
    {
        Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            string entityUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            IEnumerable<string> entityUuids,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            string entityUuid,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByEntityUuid(
            IEnumerable<string> entityUuids,
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null);

        Task<IEnumerable<ITrakHoundObjectMetadataEntity>> QueryByName(
            string name,
            TrakHound.Entities.TrakHoundMetadataQueryType queryType,
            string query,
            string routerId = null);

        Task<bool> DeleteByObjectUuid(
            string objectUuid,
            string routerId = null);

        Task<bool> DeleteByObjectUuid(
            IEnumerable<string> objectUuids,
            string routerId = null);

    }
}