// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Entities.Filters
{
    public class TrakHoundEntityPeriodFilter
    {
        private readonly string _id;
        private readonly long _period;
        private readonly PersistentDictionary<string, long> _values;
        private readonly object _lock = new object();


        public TrakHoundEntityPeriodFilter(string id, TimeSpan period)
        {
            _id = id;
            _period = period.Ticks;
            _values = new PersistentDictionary<string, long>(id);

            _values.Recover();
        }


        public bool Filter(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var entityCategory = entity.Category.ConvertEnum<TrakHoundEntityCategory>();
                switch (entityCategory)
                {
                    case TrakHoundEntityCategory.Sources:

                        break;

                    case TrakHoundEntityCategory.Definitions:

                        break;

                    case TrakHoundEntityCategory.Objects:

                        var objectsClass = entity.Class.ConvertEnum<TrakHoundObjectsEntityClass>();
                        switch (objectsClass)
                        {
                            case TrakHoundObjectsEntityClass.Assignment: return Filter((ITrakHoundObjectAssignmentEntity)entity);
                            case TrakHoundObjectsEntityClass.Boolean: return Filter((ITrakHoundObjectBooleanEntity)entity);
                            case TrakHoundObjectsEntityClass.Duration: return Filter((ITrakHoundObjectDurationEntity)entity);
                            case TrakHoundObjectsEntityClass.Event: return Filter((ITrakHoundObjectEventEntity)entity);
                            case TrakHoundObjectsEntityClass.Hash: return Filter((ITrakHoundObjectHashEntity)entity);
                            case TrakHoundObjectsEntityClass.Number: return Filter((ITrakHoundObjectNumberEntity)entity);
                            case TrakHoundObjectsEntityClass.Object: return Filter((ITrakHoundObjectEntity)entity);
                            case TrakHoundObjectsEntityClass.Observation: return Filter((ITrakHoundObjectObservationEntity)entity);                        
                            case TrakHoundObjectsEntityClass.Set: return Filter((ITrakHoundObjectSetEntity)entity);                        
                            case TrakHoundObjectsEntityClass.String: return Filter((ITrakHoundObjectStringEntity)entity);                        
                            case TrakHoundObjectsEntityClass.Timestamp: return Filter((ITrakHoundObjectTimestampEntity)entity);                        
                            case TrakHoundObjectsEntityClass.Vocabulary: return Filter((ITrakHoundObjectVocabularyEntity)entity);                        
                            case TrakHoundObjectsEntityClass.VocabularySet: return Filter((ITrakHoundObjectVocabularySetEntity)entity);                        
                        }

                        break;
                }
            }

            return false;
        }

        private bool Filter(string uuid, long timestamp)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                lock (_lock)
                {
                    var update = false;

                    var existing = _values.Get(uuid);
                    if (existing > 0)
                    {
                        if (timestamp - existing > _period)
                        {
                            update = existing != timestamp;
                        }
                    }
                    else
                    {
                        update = true;
                    }

                    if (update)
                    {
                        _values.Add(uuid, timestamp);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.Uuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectAssignmentEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.AssigneeUuid, entity.RemoveTimestamp > entity.AddTimestamp ? entity.RemoveTimestamp : entity.AddTimestamp);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectBooleanEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectEventEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Timestamp);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectHashEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectNumberEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Timestamp);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectSetEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectStringEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectTimestampEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectVocabularyEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectVocabularySetEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Created);
            }

            return false;
        }


        public void Clear()
        {
            //lock (_lock) _values.Clear();
        }
    }
}
