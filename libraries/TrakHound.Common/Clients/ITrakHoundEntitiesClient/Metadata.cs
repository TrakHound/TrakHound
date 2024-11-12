// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<IEnumerable<TrakHoundMetadata>> GetMetadata(string objectPath, string routerId = null);

        Task<IEnumerable<TrakHoundMetadata>> GetMetadata(IEnumerable<string> objectPaths, string routerId = null);


        Task<bool> PublishMetadata(
            string entityUuid,
            string name,
            string value,
            string definitionId = null,
            string valueDefinitionId = null,
            bool async = false,
            string routerId = null
            );

        Task<bool> PublishMetadata(IEnumerable<TrakHoundMetadataEntry> entries, bool async = false, string routerId = null);


        Task<bool> DeleteMetadata(string entityUuid, string routerId = null);

        Task<bool> DeleteMetadata(string entityUuid, string name, string routerId = null);
    }
}
