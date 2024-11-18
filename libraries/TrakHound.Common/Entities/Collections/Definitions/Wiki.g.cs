// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundDefinitionCollection
    {
        private readonly Dictionary<string, ITrakHoundDefinitionWikiEntity> _wikis = new Dictionary<string, ITrakHoundDefinitionWikiEntity>();

        private readonly ListDictionary<string, string> _wikisByDefinitionUuid = new ListDictionary<string, string>(); // DefinitionUuid => Uuid



        public IEnumerable<ITrakHoundDefinitionWikiEntity> Wikis => _wikis.Values;
        public void AddWiki(ITrakHoundDefinitionWikiEntity entity)
        {
            if (entity != null)
            {
                var x = _wikis.GetValueOrDefault(entity.Uuid);
                if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                {
                    _wikis.Remove(entity.Uuid);
                    _wikis.Add(entity.Uuid, new TrakHoundDefinitionWikiEntity(entity));


                    _wikisByDefinitionUuid.Add(entity.DefinitionUuid, entity.Uuid);

                }
            }
        }

        public void AddWikis(IEnumerable<ITrakHoundDefinitionWikiEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity.Uuid != null)
                    {                    
                        var x = _wikis.GetValueOrDefault(entity.Uuid);
                        if (x == null || !ObjectExtensions.ByteArraysEqual(x.Hash, entity.Hash))
                        {
                            _wikis.Remove(entity.Uuid);
                            _wikis.Add(entity.Uuid, new TrakHoundDefinitionWikiEntity(entity));


                            _wikisByDefinitionUuid.Add(entity.DefinitionUuid, entity.Uuid);
                        }                   
                    }
                }
            }
        }


        public ITrakHoundDefinitionWikiEntity GetWiki(string uuid)
        {
            if (!string.IsNullOrEmpty(uuid))
            {
                return _wikis.GetValueOrDefault(uuid);
            }

            return null;
        }

        public IEnumerable<ITrakHoundDefinitionWikiEntity> GetWikis(IEnumerable<string> uuids)
        {
            if (!uuids.IsNullOrEmpty())
            {
                var entities = new List<ITrakHoundDefinitionWikiEntity>();
                foreach (var uuid in uuids)
                {
                    var entity = _wikis.GetValueOrDefault(uuid);
                    if (entity != null) entities.Add(entity);
                }
                return entities;
            }

            return null;
        }




        public IEnumerable<ITrakHoundDefinitionWikiEntity> QueryWikisByDefinitionUuid(string definitionUuid)
        {
            if (!string.IsNullOrEmpty(definitionUuid))
            {
                var uuids = _wikisByDefinitionUuid.Get(definitionUuid);
                if (!uuids.IsNullOrEmpty())
                {
                    var entities = new List<ITrakHoundDefinitionWikiEntity>();
                    foreach (var uuid in uuids)
                    {
                        var entity = _wikis.GetValueOrDefault(uuid);
                        if (entity != null) entities.Add(entity);
                    }
                    return entities;
                }
            }

            return null;
        }




        public IEnumerable<object[]> GetWikiArrays()
        {
            var arrays = new List<object[]>();
            foreach (var entity in _wikis.Values) arrays.Add(TrakHoundEntity.ToArray(entity));
            return !arrays.IsNullOrEmpty() ? arrays : null;
        }
    }
}
