// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Entities.Filters
{
    public class TrakHoundEntityDeltaFilter
    {
        private readonly string _id;
        private readonly double _minimumDelta;
        private readonly PersistentDictionary<string, double?> _values;
        private readonly object _lock = new object();


        public TrakHoundEntityDeltaFilter(string id, double minimumDelta)
        {
            _id = id;
            _minimumDelta = minimumDelta;
            _values = new PersistentDictionary<string, double?>(id);

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
                            case TrakHoundObjectsEntityClass.Duration: return Filter((ITrakHoundObjectDurationEntity)entity);
                            case TrakHoundObjectsEntityClass.Number: return Filter((ITrakHoundObjectNumberEntity)entity);
                            case TrakHoundObjectsEntityClass.Observation: return Filter((ITrakHoundObjectObservationEntity)entity);                                              
                            case TrakHoundObjectsEntityClass.Timestamp: return Filter((ITrakHoundObjectTimestampEntity)entity);                                               
                        }

                        break;
                }
            }

            return false;
        }

        private bool Filter(string uuid, double value)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                lock (_lock)
                {
                    var update = false;

                    var existing = _values.Get(uuid);
                    if (existing != null)
                    {
                        update = Math.Abs(value - existing.Value) >= _minimumDelta;
                    }
                    else
                    {
                        update = true;
                    }

                    if (update)
                    {
                        _values.Add(uuid, value);
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

        public bool Filter(ITrakHoundObjectDurationEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Value.ToDouble());
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectNumberEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Value.ToDouble());
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectObservationEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Value.ToDouble());
            }

            return false;
        }

        public bool Filter(ITrakHoundObjectTimestampEntity entity)
        {
            if (entity != null)
            {
                return Filter(entity.ObjectUuid, entity.Value.ToDouble());
            }

            return false;
        }


        public void Clear()
        {
            //lock (_lock) _values.Clear();
        }
    }
}
