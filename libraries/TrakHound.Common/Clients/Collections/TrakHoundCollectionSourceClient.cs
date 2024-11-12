// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients.Collections
{
    internal partial class TrakHoundCollectionSourceClient
    {
        public async Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var entities = new List<ITrakHoundSourceEntity>();

                ITrakHoundSourceEntity entity;
                string nextUuid = uuid;

                do
                {
                    entity = _collection.Sources.GetSource(nextUuid);
                    if (entity != null)
                    {
                        entities.Add(entity);
                        nextUuid = entity.ParentUuid;
                    }
                }
                while (entity != null);

                return entities;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundSourceEntity>> QueryChain(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundSourceEntity>();

                foreach (var uuid in uuids)
                {
                    var targetEntities = await QueryChain(uuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        entities.AddRange(targetEntities);
                    }
                }

                return entities;
            }

            return null;
        }


        public async Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(string uuid, string routerId = null)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                var results = new List<ITrakHoundSourceQueryResult>();

                ITrakHoundSourceEntity entity;
                string nextUuid = uuid;

                do
                {
                    entity = _collection.Sources.GetSource(nextUuid);
                    if (entity != null)
                    {
                        results.Add(new TrakHoundSourceQueryResult(TrakHoundSourceQueryRequestType.Uuid, uuid, entity.Uuid));
                        nextUuid = entity.ParentUuid;
                    }
                }
                while (entity != null);

                return results;
            }

            return null;
        }

        public async Task<IEnumerable<ITrakHoundSourceQueryResult>> QueryUuidChain(IEnumerable<string> uuids, string routerId = null)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var results = new List<ITrakHoundSourceQueryResult>();

                foreach (var uuid in uuids)
                {
                    var targetResults = await QueryUuidChain(uuid);
                    if (!targetResults.IsNullOrEmpty())
                    {
                        results.AddRange(targetResults);
                    }
                }

                return results;
            }

            return null;
        }
    }
}
