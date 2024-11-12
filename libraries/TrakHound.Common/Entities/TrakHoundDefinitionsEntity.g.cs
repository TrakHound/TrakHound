// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Definitions Entities in the TrakHound Framework
    /// </summary>
    public static class TrakHoundDefinitionsEntity
    {

        public const string Definition = "Definition";
        public const string Metadata = "Metadata";
        public const string Description = "Description";
        public const string Wiki = "Wiki";


        public static bool IsDefinitionsEntity<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity))) return true;


            return false;
        }

        public static bool IsDefinitionsEntity(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(type)) return true;

            }

            return false;
        }


        public static string GetEntityClass<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity)))
                return Definition;

            else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return Metadata;

            else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity)))
                return Description;

            else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity)))
                return Wiki;


            return default;
        }

        public static byte GetEntityClassId<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundDefinitionsEntityClassId.Definition;

            else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundDefinitionsEntityClassId.Metadata;

            else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundDefinitionsEntityClassId.Description;

            else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundDefinitionsEntityClassId.Wiki;


            return 0;
        }

        public static IEnumerable<string> GetEntityClasses()
        {
            var classes = new List<string>();

            foreach (var entityClass in Enum.GetValues(typeof(TrakHoundDefinitionsEntityClass)))
            {
                classes.Add(entityClass.ToString());
            }

            return classes;
        }

        public static IEnumerable<byte> GetEntityClassIds()
        {
            var classes = new List<byte>();

            foreach (TrakHoundDefinitionsEntityClass entityClass in Enum.GetValues(typeof(TrakHoundDefinitionsEntityClass)))
            {
                classes.Add(TrakHoundDefinitionsEntityClassId.Get(entityClass));
            }

            return classes;
        }

        public static TEntity FromJson<TEntity>(string json) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(json))
            {

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionEntity.FromJson(json);

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionMetadataEntity.FromJson(json);

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionDescriptionEntity.FromJson(json);

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionWikiEntity.FromJson(json);

            }

            return default;
        }

        public static ITrakHoundEntity FromJson(byte entityClass, string json)
        {
            if (entityClass > 0 && !string.IsNullOrEmpty(json))
            {
                switch (entityClass)
                {

                    case TrakHoundDefinitionsEntityClassId.Definition: return TrakHoundDefinitionEntity.FromJson(json);
                    case TrakHoundDefinitionsEntityClassId.Metadata: return TrakHoundDefinitionMetadataEntity.FromJson(json);
                    case TrakHoundDefinitionsEntityClassId.Description: return TrakHoundDefinitionDescriptionEntity.FromJson(json);
                    case TrakHoundDefinitionsEntityClassId.Wiki: return TrakHoundDefinitionWikiEntity.FromJson(json);
                }
            }

            return default;
        }


        public static string ToJson(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionEntity((ITrakHoundDefinitionEntity)entity).ToJson();

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionMetadataEntity((ITrakHoundDefinitionMetadataEntity)entity).ToJson();

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionDescriptionEntity((ITrakHoundDefinitionDescriptionEntity)entity).ToJson();

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionWikiEntity((ITrakHoundDefinitionWikiEntity)entity).ToJson();

            }

            return null;
        }


        public static TEntity FromArray<TEntity>(object[] a) where TEntity : ITrakHoundEntity
        {
            if (a != null)
            {

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionEntity.FromArray(a);

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionMetadataEntity.FromArray(a);

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionDescriptionEntity.FromArray(a);

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundDefinitionWikiEntity.FromArray(a);

            }

            return default;
        }

        public static ITrakHoundEntity FromArray(string entityClass, object[] a)
        {
            if (!string.IsNullOrEmpty(entityClass) && !a.IsNullOrEmpty())
            {
                var x = entityClass.ToPascalCase().ConvertEnum<TrakHoundDefinitionsEntityClass>();
                switch (x)
                {

                    case TrakHoundDefinitionsEntityClass.Definition: return TrakHoundDefinitionEntity.FromArray(a);
                    case TrakHoundDefinitionsEntityClass.Metadata: return TrakHoundDefinitionMetadataEntity.FromArray(a);
                    case TrakHoundDefinitionsEntityClass.Description: return TrakHoundDefinitionDescriptionEntity.FromArray(a);
                    case TrakHoundDefinitionsEntityClass.Wiki: return TrakHoundDefinitionWikiEntity.FromArray(a);
                }
            }

            return default;
        }


        public static object[] ToArray(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundDefinitionEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionEntity((ITrakHoundDefinitionEntity)entity).ToArray();

                else if (typeof(ITrakHoundDefinitionMetadataEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionMetadataEntity((ITrakHoundDefinitionMetadataEntity)entity).ToArray();

                else if (typeof(ITrakHoundDefinitionDescriptionEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionDescriptionEntity((ITrakHoundDefinitionDescriptionEntity)entity).ToArray();

                else if (typeof(ITrakHoundDefinitionWikiEntity).IsAssignableFrom(type))
                    return new TrakHoundDefinitionWikiEntity((ITrakHoundDefinitionWikiEntity)entity).ToArray();

            }

            return null;
        }
    }
}
