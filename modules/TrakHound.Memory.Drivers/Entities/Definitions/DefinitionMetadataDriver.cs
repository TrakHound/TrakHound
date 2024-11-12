// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class DefinitionMetadataDriver : MemoryEntityDriver<ITrakHoundDefinitionMetadataEntity>,
        IDefinitionMetadataQueryDriver,
        IEntityEmptyDriver<ITrakHoundDefinitionMetadataEntity>
    {
        private readonly Dictionary<string, IEnumerable<ITrakHoundDefinitionMetadataEntity>> _metadata = new Dictionary<string, IEnumerable<ITrakHoundDefinitionMetadataEntity>>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public DefinitionMetadataDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundDefinitionMetadataEntity>> Query(IEnumerable<string> definitionUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundDefinitionMetadataEntity>>();

            if (!definitionUuids.IsNullOrEmpty())
            {
                foreach (var definitionUuid in definitionUuids)
                {
                    if (!string.IsNullOrEmpty(definitionUuid))
                    {
                        lock (_lock)
                        {
                            var empty = _empty.Contains(definitionUuid);
                            if (empty)
                            {
                                // Update Last Accessed
                                _accessed.Remove(definitionUuid);
                                _accessed.Add(definitionUuid, UnixDateTime.Now);

                                results.Add(new TrakHoundResult<ITrakHoundDefinitionMetadataEntity>(Id, definitionUuid, TrakHoundResultType.Empty));
                            }
                            else if (_metadata.TryGetValue(definitionUuid, out var objs))
                            {
                                if (!objs.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(definitionUuid);
                                    _accessed.Add(definitionUuid, UnixDateTime.Now);

                                    foreach (var obj in objs)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundDefinitionMetadataEntity>(Id, definitionUuid, TrakHoundResultType.Ok, obj));
                                    }
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionMetadataEntity>(Id, definitionUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionMetadataEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundDefinitionMetadataEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundDefinitionMetadataEntity>(results, stpw.ElapsedTicks);
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


        protected override bool PublishCompare(ITrakHoundDefinitionMetadataEntity newEntity, ITrakHoundDefinitionMetadataEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }

        protected override TrakHoundPublishResult<ITrakHoundDefinitionMetadataEntity> OnPublish(ITrakHoundDefinitionMetadataEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.DefinitionUuid))
            {
                lock (_lock)
                {
                    var metadata = new List<ITrakHoundDefinitionMetadataEntity>();

                    // Add Existing
                    _metadata.TryGetValue(entity.DefinitionUuid, out var existing);
                    if (!existing.IsNullOrEmpty())
                    {
                        foreach (var existingEntity in existing)
                        {
                            if (existingEntity.Uuid != entity.Uuid) metadata.Add(existingEntity);
                        }
                    }

                    metadata.Add(entity);

                    _empty.Remove(entity.DefinitionUuid);
                    _metadata.Remove(entity.DefinitionUuid);
                    _metadata.Add(entity.DefinitionUuid, metadata);

                    _updated.Remove(entity.DefinitionUuid);
                    _updated.Add(entity.DefinitionUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundDefinitionMetadataEntity>();
        }
    }
}
