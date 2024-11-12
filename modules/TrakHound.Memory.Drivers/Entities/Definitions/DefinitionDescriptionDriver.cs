// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class DefinitionDescriptionDriver : MemoryEntityDriver<ITrakHoundDefinitionDescriptionEntity>,
        IDefinitionDescriptionQueryDriver,
        IEntityEmptyDriver<ITrakHoundDefinitionDescriptionEntity>
    {
        private readonly Dictionary<string, IEnumerable<ITrakHoundDefinitionDescriptionEntity>> _descriptions = new Dictionary<string, IEnumerable<ITrakHoundDefinitionDescriptionEntity>>();
        private readonly HashSet<string> _empty = new HashSet<string>();


        public DefinitionDescriptionDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }


        public async Task<TrakHoundResponse<ITrakHoundDefinitionDescriptionEntity>> Query(IEnumerable<string> definitionUuids)
        {
            var stpw = System.Diagnostics.Stopwatch.StartNew();
            var results = new List<TrakHoundResult<ITrakHoundDefinitionDescriptionEntity>>();

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

                                results.Add(new TrakHoundResult<ITrakHoundDefinitionDescriptionEntity>(Id, definitionUuid, TrakHoundResultType.Empty));
                            }
                            else if (_descriptions.TryGetValue(definitionUuid, out var objs))
                            {
                                if (!objs.IsNullOrEmpty())
                                {
                                    // Update Last Accessed
                                    _accessed.Remove(definitionUuid);
                                    _accessed.Add(definitionUuid, UnixDateTime.Now);

                                    foreach (var obj in objs)
                                    {
                                        results.Add(new TrakHoundResult<ITrakHoundDefinitionDescriptionEntity>(Id, definitionUuid, TrakHoundResultType.Ok, obj));
                                    }
                                }
                            }
                            else
                            {
                                results.Add(new TrakHoundResult<ITrakHoundDefinitionDescriptionEntity>(Id, definitionUuid, TrakHoundResultType.NotFound));
                            }
                        }
                    }
                    else
                    {
                        results.Add(new TrakHoundResult<ITrakHoundDefinitionDescriptionEntity>(Id, null, TrakHoundResultType.BadRequest));
                    }
                }
            }
            else
            {
                results.Add(new TrakHoundResult<ITrakHoundDefinitionDescriptionEntity>(Id, null, TrakHoundResultType.BadRequest));
            }

            stpw.Stop();
            return new TrakHoundResponse<ITrakHoundDefinitionDescriptionEntity>(results, stpw.ElapsedTicks);
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


        protected override bool PublishCompare(ITrakHoundDefinitionDescriptionEntity newEntity, ITrakHoundDefinitionDescriptionEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }

        protected override TrakHoundPublishResult<ITrakHoundDefinitionDescriptionEntity> OnPublish(ITrakHoundDefinitionDescriptionEntity entity)
        {
            if (entity != null && !string.IsNullOrEmpty(entity.DefinitionUuid))
            {
                lock (_lock)
                {
                    var description = new List<ITrakHoundDefinitionDescriptionEntity>();

                    // Add Existing
                    _descriptions.TryGetValue(entity.DefinitionUuid, out var existing);
                    if (!existing.IsNullOrEmpty())
                    {
                        foreach (var existingEntity in existing)
                        {
                            if (existingEntity.Uuid != entity.Uuid) description.Add(existingEntity);
                        }
                    }

                    description.Add(entity);

                    _empty.Remove(entity.DefinitionUuid);
                    _descriptions.Remove(entity.DefinitionUuid);
                    _descriptions.Add(entity.DefinitionUuid, description);

                    _updated.Remove(entity.DefinitionUuid);
                    _updated.Add(entity.DefinitionUuid, UnixDateTime.Now);
                }

                return base.OnPublish(entity);
            }

            return new TrakHoundPublishResult<ITrakHoundDefinitionDescriptionEntity>();
        }
    }
}
