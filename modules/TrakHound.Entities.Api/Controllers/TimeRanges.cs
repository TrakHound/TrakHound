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
    [TrakHoundApiController("objects/time-range")]
    public class TimeRanges : EntitiesApiControllerBase
    {
        public TimeRanges() { }

        public TimeRanges(ITrakHoundApiConfiguration configuration, ITrakHoundClient client)
        {
            SetConfiguration(configuration);
            SetClient(client);
        }


        [TrakHoundApiQuery]
        public async Task<TrakHoundApiResponse> GetTimeRanges(
            [FromQuery] string objectPath,
            [FromQuery] string start,
            [FromQuery] string stop,
            [FromQuery] long skip = 0,
            [FromQuery] long take = 1000,
            [FromQuery] string routerId = null
            )
        {
            if (!string.IsNullOrEmpty(objectPath))
            {
                var startTimestamp = start.ToDateTime().ToUnixTime();
                var stopTimestamp = stop.ToDateTime().ToUnixTime();

                IEnumerable<ITrakHoundObjectTimeRangeEntity> entities = null;
                if (startTimestamp > 0 && stopTimestamp > 0)
                {
                    entities = await Client.System.Entities.Objects.TimeRange.QueryByObject(objectPath, routerId);
                }

                if (!entities.IsNullOrEmpty())
                {
                    var objectUuids = entities.Select(o => o.ObjectUuid).Distinct();
                    var objects = await Client.System.Entities.Objects.ReadByUuid(objectUuids, routerId);
                    var dObjects = objects?.ToDictionary(o => o.Uuid);

                    var results = new List<TrakHoundTimeRange>();
                    foreach (var entity in entities)
                    {
                        var obj = dObjects?.GetValueOrDefault(entity.ObjectUuid);
                        if (obj != null)
                        {
                            var result = new TrakHoundTimeRange();
                            result.Uuid = entity.Uuid;
                            result.Object = obj.GetAbsolutePath();
                            result.ObjectUuid = entity.ObjectUuid;
                            result.Start = entity.Start.ToLocalDateTime();
                            result.End = entity.End.ToLocalDateTime();
                            result.SourceUuid = entity.SourceUuid;
                            result.Created = entity.Created.ToLocalDateTime();
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

        [TrakHoundApiPublish]
        public async Task<TrakHoundApiResponse> PublishTimeRange(
            [FromQuery] string objectPath,
            [FromQuery] string start,
            [FromQuery] string end,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!string.IsNullOrEmpty(objectPath) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                var entry = new TrakHoundTimeRangeEntry();
                entry.ObjectPath = objectPath;
                entry.Start = start.ToDateTime();
                entry.End = end.ToDateTime();

                var entries = new TrakHoundTimeRangeEntry[] { entry };

                return await PublishTimeRanges(entries, async, routerId);
            }
            else
            {
                response.StatusCode = 400;
            }

            return response;
        }

        [TrakHoundApiPublish("batch")]
        public async Task<TrakHoundApiResponse> PublishTimeRanges(
            [FromBody("application/json")] IEnumerable<TrakHoundTimeRangeEntry> entries,
            [FromQuery] bool async = false,
            [FromQuery] string routerId = null
            )
        {
            var response = new TrakHoundApiResponse();

            if (!entries.IsNullOrEmpty())
            {
                var now = UnixDateTime.Now;
                var publishCollection = new TrakHoundEntityCollection();

                CreateEntities(publishCollection, entries, now);

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

        public void CreateEntities(TrakHoundEntityCollection collection, IEnumerable<TrakHoundTimeRangeEntry> entries, long created = 0)
        {
            if (!entries.IsNullOrEmpty())
            {
                var ts = created > 0 ? created : UnixDateTime.Now;

                // Create Objects that are referenced by Path in entries
                CreateContentObjects(collection, entries, ts);

                foreach (var entry in entries)
                {
                    string objectUuid = TrakHoundPath.GetUuid(entry.ObjectPath);
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        collection.Add(new TrakHoundObjectTimeRangeEntity(objectUuid, entry.Start, entry.End, created: ts));
                    }
                }
            }
        }
    }
}
