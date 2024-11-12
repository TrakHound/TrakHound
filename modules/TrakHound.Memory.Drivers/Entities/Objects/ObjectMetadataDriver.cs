// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class ObjectMetadataDriver : 
        MemoryEntityDriver<ITrakHoundObjectMetadataEntity>,
        IObjectMetadataQueryDriver,
        IObjectMetadataDeleteDriver,
        IEntityEmptyDriver<ITrakHoundObjectMetadataEntity>
    {
        private readonly ListDictionary<string, ITrakHoundObjectMetadataEntity> _metadata = new ListDictionary<string, ITrakHoundObjectMetadataEntity>();
        //private readonly Dictionary<string, IEnumerable<ITrakHoundObjectMetadataEntity>> _metadata = new Dictionary<string, IEnumerable<ITrakHoundObjectMetadataEntity>>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public ObjectMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundObjectMetadataEntity>>();

            if (!entityUuids.IsNullOrEmpty())
            {
                foreach (var entityUuid in entityUuids)
                {
                    if (!string.IsNullOrEmpty(entityUuid))
                    {
                        lock (_lock)
                        {
                            var empty = _empty.Contains(entityUuid);
                            if (empty)
                            {
                                // Update Last Accessed
                                _accessed.Remove(entityUuid);
                                _accessed.Add(entityUuid, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.Empty));
                            }
                            else
                            {
                                var objs = _metadata.Get(entityUuid);
                                if (!objs.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(entityUuid);
                                    _accessed.Add(entityUuid, UnixDateTime.Now);

                                    foreach (var obj in objs)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.Ok, obj));
                                    }
                                }
                                else
                                {
                                    results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, entityUuid, TrakHoundResultType.NotFound));
                                }
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundObjectMetadataEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundObjectMetadataEntity>(results, stpw.ElapsedTicks);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(IEnumerable<string> entityIds, string name, TrakHoundMetadataQueryType queryType, string query)
        {
            return TrakHoundResponse<ITrakHoundObjectMetadataEntity>.RouteNotConfigured(Id, name);
        }

        public async Task<TrakHoundResponse<ITrakHoundObjectMetadataEntity>> Query(string name, TrakHoundMetadataQueryType queryType, string query)
        {
            return TrakHoundResponse<ITrakHoundObjectMetadataEntity>.RouteNotConfigured(Id, name);
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


        protected override bool PublishCompare(ITrakHoundObjectMetadataEntity newEntity, ITrakHoundObjectMetadataEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }


        protected override TrakHoundPublishResult<ITrakHoundObjectMetadataEntity> OnPublish(ITrakHoundObjectMetadataEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.EntityUuid))
            {
                lock (_lock)
                {
                    //var metadata = new List<ITrakHoundObjectMetadataEntity>();

                    //// Add Existing
                    //_metadata.TryGetValue(entity.EntityUuid, out var existing);
                    //if (!existing.IsNullOrEmpty())
                    //{
                    //    foreach (var existingMetadata in existing)
                    //    {
                    //        if (existingMetadata.Uuid != entity.Uuid) metadata.Add(existingMetadata);
                    //    }
                    //}

                    //metadata.Add(entity);

                    _empty.Remove(entity.EntityUuid);
                    //_metadata.Remove(entity.EntityUuid);
                    _metadata.Add(entity.EntityUuid, entity);

                    _updated.Remove(entity.EntityUuid);
                    _updated.Add(entity.EntityUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundObjectMetadataEntity>();
        }


        protected override void OnDeleteBefore(IEnumerable<EntityDeleteRequest> requests)
        {
            lock (_lock)
            {
                foreach (var request in requests)
                {
                    if (request.Target != null)
                    {
                        var existing = _entities.GetValueOrDefault(request.Target);
                        if (existing != null)
                        {
                            var entityMetadata = _metadata.Get(existing.EntityUuid);
                            if (!entityMetadata.IsNullOrEmpty())
                            {
                                // Clear all Metadata for Object
                                _metadata.Remove(existing.EntityUuid);

                                // Remove Metadata matching Name
                                var x = entityMetadata.ToList();
                                x.RemoveAll(o => o.Uuid == existing.Uuid);

                                // Add back new Metadata list for Object
                                _metadata.Add(existing.EntityUuid, x);
                            }
                        }
                    }
                }
            }
        }

        public async Task<TrakHoundResponse<bool>> DeleteByObjectUuid(IEnumerable<string> objectUuids)
        {
            if (!objectUuids.IsNullOrEmpty())
            {
                var stpw = System.Diagnostics.Stopwatch.StartNew();
                var results = new List<TrakHoundResult<bool>>();

                foreach (var objectUuid in objectUuids)
                {
                    if (!string.IsNullOrEmpty(objectUuid))
                    {
                        lock (_lock)
                        {
                            var entities = _metadata.Get(objectUuid);
                            if (!entities.IsNullOrEmpty())
                            {
                                foreach (var entity in entities)
                                {
                                    _entities.Remove(entity.Uuid);
                                }
                            }

                            _metadata.Remove(objectUuid);
                        }

                        results.Add(new TrakHoundResult<bool>(Id, objectUuid, TrakHoundResultType.Ok, true));
                    }
                }

                stpw.Stop();
                return new TrakHoundResponse<bool>(results, stpw.ElapsedTicks);
            }

            return TrakHoundResponse<bool>.InternalError(Id, null);
        }
    }
}
