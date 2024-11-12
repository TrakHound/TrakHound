// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundDefinitionCollection
    {
        private readonly Dictionary<string, ITrakHoundDefinitionDescriptionEntity> _descriptions = new Dictionary<string, ITrakHoundDefinitionDescriptionEntity>();

        private readonly ListDictionary<string, string> _descriptionsByDefinitionUuid = new ListDictionary<string, string>(); // DefinitionUuid => Uuid



        public IEnumerable<ITrakHoundDefinitionDescriptionEntity> Descriptions => _descriptions.Values;


        public void AddDescription(ITrakHoundDefinitionDescriptionEntity entity)
        {
            if (entity != null)
            {
                var x = _descriptions.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _descriptions.Remove(entity.Uuid);
                    _descriptions.Add(entity.Uuid, new TrakHoundDefinitionDescriptionEntity(entity));


                    _descriptionsByDefinitionUuid.Add(entity.DefinitionUuid, entity.Uuid);

                }
            }
        }

        public void AddDescriptions(IEnumerable<ITrakHoundDefinitionDescriptionEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _descriptions.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _descriptions.Remove(entity.Uuid);
                            _descriptions.Add(entity.Uuid, new TrakHoundDefinitionDescriptionEntity(entity));


                            _descriptionsByDefinitionUuid.Add(entity.DefinitionUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundDefinitionDescriptionEntity GetDescription(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _descriptions.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundDefinitionDescriptionEntity> GetDescriptions(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundDefinitionDescriptionEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _descriptions.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundDefinitionDescriptionEntity> QueryDescriptionsByDefinitionUuid(string definitionUuid)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                var uuids = _descriptionsByDefinitionUuid.Get(definitionUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundDefinitionDescriptionEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _descriptions.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetDescriptionArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _descriptions.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
