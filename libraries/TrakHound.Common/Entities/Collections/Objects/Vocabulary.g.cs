// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectVocabularyEntity> _vocabularies = new Dictionary<string, ITrakHoundObjectVocabularyEntity>();

        private readonly Dictionary<string, string> _vocabulariesByObjectUuid = new Dictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectVocabularyEntity> Vocabularies => _vocabularies.Values;


        public void AddVocabulary(ITrakHoundObjectVocabularyEntity entity)
        {
            if (entity != null)
            {
                var x = _vocabularies.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _vocabularies.Remove(entity.Uuid);
                    _vocabularies.Add(entity.Uuid, new TrakHoundObjectVocabularyEntity(entity));


                    _vocabulariesByObjectUuid.Remove(entity.ObjectUuid);
                    _vocabulariesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                }
            }
        }

        public void AddVocabularies(IEnumerable<ITrakHoundObjectVocabularyEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _vocabularies.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _vocabularies.Remove(entity.Uuid);
                            _vocabularies.Add(entity.Uuid, new TrakHoundObjectVocabularyEntity(entity));


                            _vocabulariesByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectVocabularyEntity GetVocabulary(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _vocabularies.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectVocabularyEntity> GetVocabularies(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectVocabularyEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _vocabularies.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public ITrakHoundObjectVocabularyEntity QueryVocabularyByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuid = _vocabulariesByObjectUuid.GetValueOrDefault(objectUuid);
                if (!string.IsNullOrEmpty(uuid))
                {
                    return _vocabularies.GetValueOrDefault(uuid);
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetVocabularyArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _vocabularies.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
