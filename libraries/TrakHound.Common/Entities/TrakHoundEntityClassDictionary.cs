// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public class TrakHoundEntityClassDictionary
    {
        private readonly ListDictionary<byte, ITrakHoundEntity> _objects = new ListDictionary<byte, ITrakHoundEntity>();
        private readonly ListDictionary<byte, ITrakHoundEntity> _definitions = new ListDictionary<byte, ITrakHoundEntity>();
        private readonly ListDictionary<byte, ITrakHoundEntity> _sources = new ListDictionary<byte, ITrakHoundEntity>();


        public IEnumerable<byte> ObjectClassIds => _objects.Keys;
        public IEnumerable<byte> DefinitionClassIds => _definitions.Keys;
        public IEnumerable<byte> SourceClassIds => _sources.Keys;


        public void Add(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                switch (entity.Category)
                {
                    case TrakHoundEntityCategoryId.Objects: _objects.Add(entity.Class, entity); break;

                    case TrakHoundEntityCategoryId.Definitions: _definitions.Add(entity.Class, entity); break;

                    case TrakHoundEntityCategoryId.Sources: _sources.Add(entity.Class, entity); break;
                }
            }
        }

        public void Add(IEnumerable<ITrakHoundEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity != null)
                    {
                        switch (entity.Category)
                        {
                            case TrakHoundEntityCategoryId.Objects: _objects.Add(entity.Class, entity); break;

                            case TrakHoundEntityCategoryId.Definitions: _definitions.Add(entity.Class, entity); break;

                            case TrakHoundEntityCategoryId.Sources: _sources.Add(entity.Class, entity); break;
                        }
                    }
                }
            }
        }

        public IEnumerable<ITrakHoundEntity> Get(byte entityCategory, byte entityClass)
        {
            switch (entityCategory)
            {
                case TrakHoundEntityCategoryId.Objects: return _objects.Get(entityClass);

                case TrakHoundEntityCategoryId.Definitions: return _definitions.Get(entityClass);

                case TrakHoundEntityCategoryId.Sources: return _sources.Get(entityClass);
            }

            return null;
        }

        public IEnumerable<ITrakHoundEntity> Get(string entityCategory, string entityClass)
        {
            var entityCategoryId = TrakHoundEntityCategoryId.Get(entityCategory);
            var entityClassId = TrakHoundEntity.GetEntityClassId(entityCategory, entityClass);

            switch (entityCategoryId)
            {
                case TrakHoundEntityCategoryId.Objects: return _objects.Get(entityClassId);

                case TrakHoundEntityCategoryId.Definitions: return _definitions.Get(entityClassId);

                case TrakHoundEntityCategoryId.Sources: return _sources.Get(entityClassId);
            }

            return null;
        }
    }
}
