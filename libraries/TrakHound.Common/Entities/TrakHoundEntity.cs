// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// An Entity in the TrakHound Framework
    /// </summary>
    public static class TrakHoundEntity
    {
        public const string Definitions = "Definitions";
        public const string Objects = "Objects";
        public const string Sources = "Sources";


        public static string GetEntityCategory<TEntity>() where TEntity : ITrakHoundEntity
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return Sources;
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return Definitions;
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return Objects;

            return default;
        }

        public static byte GetEntityCategoryId<TEntity>() where TEntity : ITrakHoundEntity
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return TrakHoundEntityCategoryId.Sources;
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return TrakHoundEntityCategoryId.Definitions;
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return TrakHoundEntityCategoryId.Objects;

            return default;
        }


        public static string GetEntityClass<TEntity>() where TEntity : ITrakHoundEntity
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return TrakHoundSourcesEntity.GetEntityClass<TEntity>();
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return TrakHoundDefinitionsEntity.GetEntityClass<TEntity>();
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return TrakHoundObjectsEntity.GetEntityClass<TEntity>();

            return default;
        }

        public static byte GetEntityClassId<TEntity>() where TEntity : ITrakHoundEntity
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return TrakHoundSourcesEntity.GetEntityClassId<TEntity>();
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return TrakHoundDefinitionsEntity.GetEntityClassId<TEntity>();
            else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return TrakHoundObjectsEntity.GetEntityClassId<TEntity>();

            return default;
        }

        public static byte GetEntityClassId(string entityCategory, string entityClass)
        {
            switch (entityCategory)
            {
                case TrakHoundEntityCategoryName.Objects: return TrakHoundObjectsEntityClassId.Get(entityClass);
                case TrakHoundEntityCategoryName.Definitions: return TrakHoundDefinitionsEntityClassId.Get(entityClass);
                case TrakHoundEntityCategoryName.Sources: return TrakHoundSourcesEntityClassId.Get(entityClass);
            }

            return default;
        }

        public static string GetEntityClassName(byte entityCategory, byte entityClass)
        {
            switch (entityCategory)
            {
                case TrakHoundEntityCategoryId.Objects: return TrakHoundObjectsEntityClassName.Get(entityClass);
                case TrakHoundEntityCategoryId.Definitions: return TrakHoundDefinitionsEntityClassName.Get(entityClass);
                case TrakHoundEntityCategoryId.Sources: return TrakHoundSourcesEntityClassName.Get(entityClass);
            }

            return default;
        }


        public static IEnumerable<string> GetEntityCategories()
        {
            var categories = new List<string>();

            categories.Add(Definitions);
            categories.Add(Objects);
            categories.Add(Sources);

            return categories;
        }

        public static IEnumerable<string> GetEntityClasses()
        {
            var classes = new List<string>();

            classes.AddRange(TrakHoundDefinitionsEntity.GetEntityClasses());
            classes.AddRange(TrakHoundObjectsEntity.GetEntityClasses());
            classes.AddRange(TrakHoundSourcesEntity.GetEntityClasses());

            return classes;
        }

        public static IEnumerable<string> GetEntityClasses(string entityCategory)
        {
            var classes = new List<string>();

            if (entityCategory == Definitions) classes.AddRange(TrakHoundDefinitionsEntity.GetEntityClasses());
            if (entityCategory == Objects) classes.AddRange(TrakHoundObjectsEntity.GetEntityClasses());
            if (entityCategory == Sources) classes.AddRange(TrakHoundSourcesEntity.GetEntityClasses());

            return classes;
        }

        public static IEnumerable<string> GetEntityClasses(byte entityCategoryId)
        {
            var classes = new List<string>();

            if (entityCategoryId == TrakHoundEntityCategoryId.Definitions) classes.AddRange(TrakHoundDefinitionsEntity.GetEntityClasses());
            if (entityCategoryId == TrakHoundEntityCategoryId.Objects) classes.AddRange(TrakHoundObjectsEntity.GetEntityClasses());
            if (entityCategoryId == TrakHoundEntityCategoryId.Sources) classes.AddRange(TrakHoundSourcesEntity.GetEntityClasses());

            return classes;
        }

        public static IEnumerable<byte> GetEntityClassIds(string entityCategory)
        {
            var classes = new List<byte>();

            if (entityCategory == Definitions) classes.AddRange(TrakHoundDefinitionsEntity.GetEntityClassIds());
            if (entityCategory == Objects) classes.AddRange(TrakHoundObjectsEntity.GetEntityClassIds());
            if (entityCategory == Sources) classes.AddRange(TrakHoundSourcesEntity.GetEntityClassIds());

            return classes;
        }

        public static IEnumerable<byte> GetEntityClassIds(byte entityCategoryId)
        {
            var classes = new List<byte>();

            if (entityCategoryId == TrakHoundEntityCategoryId.Definitions) classes.AddRange(TrakHoundDefinitionsEntity.GetEntityClassIds());
            if (entityCategoryId == TrakHoundEntityCategoryId.Objects) classes.AddRange(TrakHoundObjectsEntity.GetEntityClassIds());
            if (entityCategoryId == TrakHoundEntityCategoryId.Sources) classes.AddRange(TrakHoundSourcesEntity.GetEntityClassIds());

            return classes;
        }


        public static TEntity FromJson<TEntity>(string json) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(json))
            {
                if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return TrakHoundSourcesEntity.FromJson<TEntity>(json);
                else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return TrakHoundDefinitionsEntity.FromJson<TEntity>(json);
                else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return TrakHoundObjectsEntity.FromJson<TEntity>(json);
            }

            return default;
        }

        public static ITrakHoundEntity FromJson(byte entityCategory, byte entityClass, string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                switch (entityCategory)
                {
                    case TrakHoundEntityCategoryId.Definitions: return TrakHoundDefinitionsEntity.FromJson(entityClass, json);
                    case TrakHoundEntityCategoryId.Objects: return TrakHoundObjectsEntity.FromJson(entityClass, json);
                    case TrakHoundEntityCategoryId.Sources: return TrakHoundSourcesEntity.FromJson(entityClass, json);
                }
            }

            return default;
        }

        public static string ToJson(ITrakHoundEntity entity)
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity(entity)) return TrakHoundSourcesEntity.ToJson(entity);
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity(entity)) return TrakHoundDefinitionsEntity.ToJson(entity);
            else if (TrakHoundObjectsEntity.IsObjectsEntity(entity)) return TrakHoundObjectsEntity.ToJson(entity);

            return null;
        }


        public static TEntity FromArray<TEntity>(object[] a) where TEntity : ITrakHoundEntity
        {
            if (a != null)
            {
                if (TrakHoundSourcesEntity.IsSourcesEntity<TEntity>()) return TrakHoundSourcesEntity.FromArray<TEntity>(a);
                else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity<TEntity>()) return TrakHoundDefinitionsEntity.FromArray<TEntity>(a);
                else if (TrakHoundObjectsEntity.IsObjectsEntity<TEntity>()) return TrakHoundObjectsEntity.FromArray<TEntity>(a);
            }

            return default;
        }

        public static ITrakHoundEntity FromArray(string entityCategory, string entityClass, object[] a)
        {
            if (!a.IsNullOrEmpty())
            {
                var x = entityCategory.ToPascalCase().ConvertEnum<TrakHoundEntityCategory>();
                switch (x)
                {
                    case TrakHoundEntityCategory.Definitions: return TrakHoundDefinitionsEntity.FromArray(entityClass, a);
                    case TrakHoundEntityCategory.Objects: return TrakHoundObjectsEntity.FromArray(entityClass, a);
                    case TrakHoundEntityCategory.Sources: return TrakHoundSourcesEntity.FromArray(entityClass, a);
                }
            }

            return default;
        }

        public static object[] ToArray(ITrakHoundEntity entity)
        {
            if (TrakHoundSourcesEntity.IsSourcesEntity(entity)) return TrakHoundSourcesEntity.ToArray(entity);
            else if (TrakHoundDefinitionsEntity.IsDefinitionsEntity(entity)) return TrakHoundDefinitionsEntity.ToArray(entity);
            else if (TrakHoundObjectsEntity.IsObjectsEntity(entity)) return TrakHoundObjectsEntity.ToArray(entity);

            return null;
        }
    }
}
