// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectStringEntity> _strings = new Dictionary<string, ITrakHoundObjectStringEntity>();

        private readonly Dictionary<string, string> _stringsByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectStringEntity> Strings => _strings.Values;
        public void AddString(ITrakHoundObjectStringEntity entity)
        {
            if (entity != null)
            {
                var x = _strings.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _strings.Remove(entity.Uuid);
                    _strings.Add(entity.Uuid, new TrakHoundObjectStringEntity(entity));


                    _stringsByObjectUuid.Remove(entity.ObjectUuid);
                    _stringsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddStrings(IEnumerable<ITrakHoundObjectStringEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _strings.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _strings.Remove(entity.Uuid);
                            _strings.Add(entity.Uuid, new TrakHoundObjectStringEntity(entity));


                            _stringsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectStringEntity GetString(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _strings.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectStringEntity> GetStrings(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectStringEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _strings.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectStringEntity QueryStringByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _stringsByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _strings.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetStringArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _strings.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
