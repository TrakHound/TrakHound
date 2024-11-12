// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Http.Entities
{
    public class ObjectMetadataDriver : 
        HttpEntityDriver<ITrakHoundObjectMetadataEntity>,
        IObjectMetadataQueryDriver
    {
        public ObjectMetadataDriver() { }

        public ObjectMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectMetadataEntity>>();

            if (!entityUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Metadata.QueryByEntityUuid(entityUuids);
                var dEntities = entities?.ToListDictionary(o => o.EntityUuid);

                foreach (var entityUuid in entityUuids)
                {
                    var targetEntities = dEntities?.Get(entityUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids, string name)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectMetadataEntity>>();

            if (!entityUuids.IsNullOrEmpty())
            {
                var entities = await Client.System.Entities.Objects.Metadata.QueryByEntityUuid(entityUuids, name);
                var dEntities = entities?.ToListDictionary(o => o.EntityUuid);

                foreach (var entityUuid in entityUuids)
                {
                    var targetEntities = dEntities?.Get(entityUuid);
                    if (!targetEntities.IsNullOrEmpty())
                    {
                        foreach (var targetEntity in targetEntities)
                        {
                            results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.Ok, targetEntity));
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.NotFound));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, null, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids, string name, TrakHoundMetadataQueryType queryType, string value)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectMetadataEntity>>();

            var entities = await Client.System.Entities.Objects.Metadata.QueryByEntityUuid(entityUuids, name, queryType, name);
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, name, TrakHoundResultType.Ok, entity));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, name, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(results);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(string name, TrakHoundMetadataQueryType queryType, string value)
        {
            var results = new List<TrakHoundResult<ITrakHoundObjectMetadataEntity>>();

            var entities = await Client.System.Entities.Objects.Metadata.QueryByName(name, queryType, name);
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, name, TrakHoundResultType.Ok, entity));
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, name, TrakHoundResultType.NotFound));
            }

            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(results);
        }
    }
}
