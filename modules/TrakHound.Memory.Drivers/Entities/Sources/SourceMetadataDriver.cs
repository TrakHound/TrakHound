// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class SourceMetadataDriver : MemoryEntityDriver<ITrakHoundSourceMetadataEntity>,
        ISourceMetadataQueryDriver,
        IEntityEmptyDriver<ITrakHoundSourceMetadataEntity>
    {
        private readonly Dictionary<string, IEnumerable<ITrakHoundSourceMetadataEntity>> _metadata = new Dictionary<string, IEnumerable<ITrakHoundSourceMetadataEntity>>();
        private readonly List<string> _empty = new List<string>();


        public SourceMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }



        public async Task<TrakHoundResponse<ITrakHoundSourceMetadataEntity>> Query(IEnumerable<string> entityIds)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundSourceMetadataEntity>>();

            if (!entityIds.IsNullOrEmpty())
            {
                foreach (var entityId in entityIds)
                {
                    if (!string.IsNullOrEmpty(entityId))
                    {
                        lock (_lock)
                        {
                            var empty = _empty.Contains(entityId);
                            if (empty)
                            {
                                // Update Last Accessed
                                _accessed.Remove(entityId);
                                _accessed.Add(entityId, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, entityId, TrakHoundResultType.Empty));
                            }
                            else if (_metadata.TryGetValue(entityId, out var objs))
                            {
                                if (!objs.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(entityId);
                                    _accessed.Add(entityId, UnixDateTime.Now);

                                    foreach (var obj in objs)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, entityId, TrakHoundResultType.Ok, obj));
                                    }
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, entityId, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundSourceMetadataEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundSourceMetadataEntity>(results, stpw.ElapsedTicks);
        }


        public async Task<TrakHoundResponse<bool>> Empty(IEnumerable<EntityEmptyRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<bool>>();

                foreach (var request in requests)
                {
                    if (!string.IsNullOrEmpty(request.EntityUuid))
                    {
                        lock (_lock)
                        {
                            if (!_empty.Contains(request.EntityUuid))
                            {
                                _empty.Add(request.EntityUuid);

                                _updated.Remove(request.EntityUuid);
                                _updated.Add(request.EntityUuid, UnixDateTime.Now);
                            }
                        }

                        results.Add(new TrakHoundResult<bool>(Id, request.EntityUuid, TrakHoundResultType.Ok, true));
                    }
                }

                stpw.Stop();
                return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<bool>.InternalError(Id, null);
        }


        protected override bool PublishCompare(ITrakHoundSourceMetadataEntity newEntity, ITrakHoundSourceMetadataEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }


        protected override TrakHoundPublishResult<ITrakHoundSourceMetadataEntity> OnPublish(ITrakHoundSourceMetadataEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.SourceUuid))
            {
                lock (_lock)
                {
                    var metadata = new List<ITrakHoundSourceMetadataEntity>();

                    // Add Existing
                    _metadata.TryGetValue(entity.SourceUuid, out var existing);
                    if (!existing.IsNullOrEmpty()) metadata.AddRange(existing);

                    metadata.Add(entity);

                    _empty.RemoveAll(o => o == entity.SourceUuid);
                    _metadata.Remove(entity.SourceUuid);
                    _metadata.Add(entity.SourceUuid, metadata);

                    _updated.Remove(entity.SourceUuid);
                    _updated.Add(entity.SourceUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundSourceMetadataEntity>();
        }
    }
}
