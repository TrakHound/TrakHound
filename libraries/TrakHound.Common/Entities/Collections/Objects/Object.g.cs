// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectEntity> _objects = new Dictionary<string, ITrakHoundObjectEntity>();


        public IEnumerable<ITrakHoundObjectEntity> Objects => _objects.Values;


        public ITrakHoundObjectEntity GetObject(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _objects.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectEntity> GetObjects(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _objects.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }



        public IEnumerable<object[]> GetObjectArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _objects.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
