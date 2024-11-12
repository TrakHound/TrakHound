// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundSystemSourceMetadataClient : ITrakHoundEntityClient<ITrakHoundSourceMetadataEntity>
    {
        Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            string sourceUuid,
            string routerId = null);

        Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            IEnumerable<string> sourceUuids,
            string routerId = null);

    }
}