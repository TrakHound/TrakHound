// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        private readonly Dictionary<string, ITrakHoundObjectVocabularySetEntity> _vocabularySets = new Dictionary<string, ITrakHoundObjectVocabularySetEntity>();

        private readonly ListDictionary<string, string> _vocabularySetsByObjectUuid = new ListDictionary<string, string>(); // ObjectUuid => Uuid



        public IEnumerable<ITrakHoundObjectVocabularySetEntity> VocabularySets => _vocabularySets.Values;
        public void AddVocabularySet(ITrakHoundObjectVocabularySetEntity entity)
        {
            if (entity != null)
            {
                var x = _vocabularySets.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _vocabularySets.Remove(entity.Uuid);
                    _vocabularySets.Add(entity.Uuid, new TrakHoundObjectVocabularySetEntity(entity));


                    _vocabularySetsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);

                }
            }
        }

        public void AddVocabularySets(IEnumerable<ITrakHoundObjectVocabularySetEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _vocabularySets.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _vocabularySets.Remove(entity.Uuid);
                            _vocabularySets.Add(entity.Uuid, new TrakHoundObjectVocabularySetEntity(entity));


                            _vocabularySetsByObjectUuid.Add(entity.ObjectUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundObjectVocabularySetEntity GetVocabularySet(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _vocabularySets.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundObjectVocabularySetEntity> GetVocabularySets(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundObjectVocabularySetEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _vocabularySets.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundObjectVocabularySetEntity> QueryVocabularySetsByObjectUuid(string objectUuid)
        {
            if (!string.IsNullOrEmpty(objectUuid))
            {
                var uuids = _vocabularySetsByObjectUuid.Get(objectUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundObjectVocabularySetEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _vocabularySets.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetVocabularySetArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _vocabularySets.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
