// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemDefinitionWikiClient : TrakHoundInstanceEntityClient<ITrakHoundDefinitionWikiEntity>, ITrakHoundSystemDefinitionWikiClient
    {


        public TrakHoundInstanceSystemDefinitionWikiClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundDefinitionWikiEntity>> QueryByDefinitionUuid(
            string definitionUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Definitions.Wikis.QueryByDefinitionUuid(new string[] { definitionUuid });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundDefinitionWikiEntity>> QueryByDefinitionUuid(
            IEnumerable<string> definitionUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Definitions.Wikis.QueryByDefinitionUuid(definitionUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

    }
}
