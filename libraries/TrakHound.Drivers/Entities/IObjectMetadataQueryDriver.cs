// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Drivers.Entities
{
    /// <summary>
    /// Driver used to read TrakHound Object Metadata Entities
    /// </summary>
    public interface IObjectMetadataQueryDriver : ITrakHoundDriver
    {
        Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids);

        Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids, string name, TrakHoundMetadataQueryType queryType, string query);

        Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(string name, TrakHoundMetadataQueryType queryType, string query);
    }
}
