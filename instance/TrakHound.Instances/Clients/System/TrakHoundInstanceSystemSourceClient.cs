// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    internal partial class TrakHoundInstanceSystemSourceClient
    {

        public async Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var uuids = new string[] { uuid };
                    var response = await router?.Entities.Sources.Sources.QueryChain(uuids);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundSourceEntity>();
        }

        public async Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Sources.Sources.QueryChain(uuids);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundSourceEntity>();
        }


        public async Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var uuids = new string[] { uuid };
                    var response = await router?.Entities.Sources.Sources.QueryUuidChain(uuids);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundSourceQueryResult>();
        }

        public async Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var router = BaseClient.GetRouter(routerId);
                if (router != null)
                {
                    var response = await router?.Entities.Sources.Sources.QueryUuidChain(uuids);
                    if (response.IsSuccess)
                    {
                        return response.Content;
                    }
                }
            }

            return Enumerable.Empty<ITrakHoundSourceQueryResult>();
        }
    }
}
