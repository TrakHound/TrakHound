// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrakHound.Api;
using TrakHound.Clients;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Entities.Api
{
    [TrakHoundApiController("objects/number")]
    public class Numbers : EntitiesApiControllerBase
    {
        public Numbers() { }

        public Numbers(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetNumbers(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Number.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundNumber>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundNumber();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.DataType = (TrakHoundNumberDataType)entity.DataType;
                            result.Value = entity.Value;
                            result.SourceUuid = entity.SourceUuid;
                            result.Created = entity.Created.ToDateTime();
                            results.Add(result);
                        }
                    }
                    return Ok(results);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiQuery("values")]
        public async Task<TrakHoundApiResponse> GetNumberValues(
            [FromQuery] string objectPath,
            [FromBody(ContentType = "application/json")] IEnumerable<string> objectPaths,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath) || !objectPaths.IsNullOrEmpty())
            {
                var paths = !string.IsNullOrEmpty(objectPath) ? new string[] { objectPath } : objectPaths;

                var entities = await Client.System.Entities.Objects.Number.QueryByObject(paths, routerId);
                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var values = new Dictionary<string, string>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var key = obj.GetAbsolutePath();
                            if (!values.ContainsKey(key)) values[key] = entity.Value;
                        }
                    }
                    return Ok(values);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [TrakHoundApiSubscribe]
        public async Task<ITrakHoundConsumer<TrakHoundApiResponse>> SubscribeNumbers(
            [FromQuery] string objectPath,
            [FromQuery] int interval = _defaultSubscribeInterval,
            [FromQuery] int take = _defaultSubscribeTake,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var objectPaths = new string[] { objectPath };
                var consumer = await Client.System.Entities.Objects.Number.SubscribeByObject(objectPaths, interval, take, routerId: routerId);
                if (consumer != null)
                {
                    var resultConsumer = new TrakHoundConsumer<IEnumerable<ITrakHoundObjectNumberEntity>, TrakHoundApiResponse>(consumer);
                    resultConsumer.OnReceivedAsync = async (entities) =>
                    {
                        var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                        var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                        var dObjects = objects?.ToDictionary(o => o.Uuid);

                        var results = new List<TrakHoundNumber>();
                        foreach (var entity in entities)
                        {
                            var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                            if (obj != null)
                            {
                                var result = new TrakHoundNumber();
                                result.Uuid = entity.Uuid;
                                result.Object = obj.GetAbsolutePath();
                                result.ObjectUuid = entity.ObjectUuid;
                                result.DataType = (TrakHoundNumberDataType)entity.DataType;
                                result.Value = entity.Value;
                                result.SourceUuid = entity.SourceUuid;
                                result.Created = entity.Created.ToLocalDateTime();
                                results.Add(result);
                            }
                        }
                        return TrakHoundApiJsonResponse.Ok(results);
                    };

                    return resultConsumer;
                }
            }

            return null;
        }

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishNumber(
            [FromQuery] string objectPath,
            [FromBody] string value,
            [FromQuery] int dataType = (int)TrakHoundNumberDataType.Float,
            [FromQuery] string aggregateType = null,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(value))
            {
                var aggregate = aggregateType.ConvertEnum<TrakHoundAggregateType>();

                var entry = new TrakHoundNumberEntry();
                entry.ObjectPath = objectPath;
                entry.Value = value;
                entry.DataType = (TrakHoundNumberDataType)dataType;
                entry.AggregateType = aggregate;

                var entries = new TrakHoundNumberEntry[] { entry };

                return await PublishNumbers(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishNumbers(
            [FromBody("application/json")] IEnumerable<TrakHoundNumberEntry> entries,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!entries.IsNullOrEmpty())
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                await CreateEntities(publishCollection, entries, now);

                // Publish Entities
                if (await PublishEntities(publishCollection, now, async, routerId: routerId))
                {
                    return Created();
                }
                else
                {
                    return InternalError();
                }
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        public async Task CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundNumberEntry> entries, long created = 0, string routerId = null)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                var incrementEntries = entries.Where(o => o.AggregateType == TrakHoundAggregateType.Increment);
                if (!incrementEntries.IsNullOrEmpty())
                {
                    var objectUuids = incrementEntries.Select(o => TrakHoundPath.GetUuid(o.ObjectPath)).Distinct();

                    // Get Existing Entities
                    var existingEntities = await Client.System.Entities.Objects.Number.QueryByObjectUuid(objectUuids, routerId);
                    var dExistingEntities = existingEntities?.ToDictionary(o => o.ObjectUuid);


                    foreach (var objectUuid in objectUuids)
                    {
                        var objectIncrementEntries = incrementEntries.Where(o => TrakHoundPath.GetUuid(o.ObjectPath) == objectUuid);
                        double value = 0;
                        var dataType = TrakHoundNumberDataType.Byte;

                        // Set initial value based on existing entity
                        var existingEntity = dExistingEntities?.GetValueOrDefault(objectUuid);
                        if (existingEntity != null)
                        {
                            value = existingEntity.Value.ToDouble();
                        }

                        foreach (var entry in objectIncrementEntries)
                        {
                            value += entry.Value.ToDouble();
                            if (entry.DataType > dataType) dataType = entry.DataType;
                        }

                        collection.Add(new TrakHoundObjectNumberEntity(objectUuid, value.ToString(), dataType, created: ts));
                    }
                }

                var absoluteEntries = entries.Where(o => o.AggregateType == TrakHoundAggregateType.Absolute);
                if (!absoluteEntries.IsNullOrEmpty())
                {
                    foreach (var entry in absoluteEntries)
                    {
                        // Get the ObjectUuid for the given Path from the Transaction
                        string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                        if (!string.IsNullOrEmpty(objectUuid))
                        {
                            collection.Add(new TrakHoundObjectNumberEntity(objectUuid, entry.Value, entry.DataType, created: ts));
                        }
                    }
                }
            }
        }
    }
}
