// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Routing;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemSourceMetadataClient : TrakHoundInstanceEntityClient<ITrakHoundSourceMetadataEntity>, ITrakHoundSystemSourceMetadataClient
    {


        public TrakHoundInstanceSystemSourceMetadataClient(TrakHoundInstanceClient baseClient) : base(baseClient) 
        {
        }


        public async Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            string sourceUuid,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Sources.Metadata.QueryBySourceUuid(new string[] { sourceUuid });
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

        public async Task<IEnumerable<ITrakHoundSourceMetadataEntity>> QueryBySourceUuid(
            IEnumerable<string> sourceUuids,
            string routerId = null)
        {
            var router = BaseClient.GetRouter(routerId);
            if (router != null)
            {
                var response = await router.Entities.Sources.Metadata.QueryBySourceUuid(sourceUuids);
                if (response.IsSuccess)
                {
                    return response.Content.ToDistinct();
                }
            }

            return default;
        }

    }
}
