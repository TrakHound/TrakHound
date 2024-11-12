// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public partial interface ITrakHoundEntitiesClient
    {
        Task<TrakHoundDefinition> GetDefinition(string definitionId, string routerId = null);

        Task<TrakHoundDefinition> GetDefinitionByUuid(string definitionUuid, string routerId = null);

        Task<IEnumerable<TrakHoundDefinition>> GetDefinitions(IEnumerable<string> definitionIds, string routerId = null);

        Task<IEnumerable<TrakHoundDefinition>> GetParentDefinitions(string definitionId, string routerId = null);

        Task<IEnumerable<TrakHoundDefinition>> GetParentDefinitionsByUuid(string definitionUuid, string routerId = null);


        Task<IEnumerable<TrakHoundDefinition>> QueryDefinitions(string pattern, string routerId = null);


        Task<bool> PublishDefinition(string definitionId, string description = null, string parentId = null, bool async = false, string routerId = null);

        Task<bool> PublishDefinitions(IEnumerable<TrakHoundDefinitionEntry> entries, bool async = false, string routerId = null);
    }
}
