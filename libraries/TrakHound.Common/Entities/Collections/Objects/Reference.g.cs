// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectReferenceEntity> _references = new Dictionary<string, ITrakHoundObjectReferenceEntity>();

        private readonly Dictionary<string, string> _referencesByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectReferenceEntity> References => _references.Values;
        public void AddReference(ITrakHoundObjectReferenceEntity entity)
        {
            if (entity != null)
            {
                var x = _references.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _references.Remove(entity.Uuid);
                    _references.Add(entity.Uuid, new TrakHoundObjectReferenceEntity(entity));


                    _referencesByObjectUuid.Remove(entity.ObjectUuid);
                    _referencesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddReferences(IEnumerable<ITrakHoundObjectReferenceEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _references.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _references.Remove(entity.Uuid);
                            _references.Add(entity.Uuid, new TrakHoundObjectReferenceEntity(entity));


                            _referencesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectReferenceEntity GetReference(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _references.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectReferenceEntity> GetReferences(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectReferenceEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _references.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectReferenceEntity QueryReferenceByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _referencesByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _references.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetReferenceArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _references.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
