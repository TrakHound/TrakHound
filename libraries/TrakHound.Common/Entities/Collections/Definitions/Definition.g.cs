// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundDefinitionCollection
    {
        private readonly Dictionary<string, ITrakHoundDefinitionEntity> _definitions = new Dictionary<string, ITrakHoundDefinitionEntity>();


        public IEnumerable<ITrakHoundDefinitionEntity> Definitions => _definitions.Values;


        public void AddDefinition(ITrakHoundDefinitionEntity entity)
        {
            if (entity != null)
            {
                var x = _definitions.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _definitions.Remove(entity.Uuid);
                    _definitions.Add(entity.Uuid, new TrakHoundDefinitionEntity(entity));


                    OnAddDefinition(entity);
                }
            }
        }

        public void AddDefinitions(IEnumerable<ITrakHoundDefinitionEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _definitions.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _definitions.Remove(entity.Uuid);
                            _definitions.Add(entity.Uuid, new TrakHoundDefinitionEntity(entity));


                            OnAddDefinition(entity);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundDefinitionEntity GetDefinition(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _definitions.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundDefinitionEntity> GetDefinitions(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundDefinitionEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _definitions.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }



        public IEnumerable<object[]> GetDefinitionArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _definitions.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
