// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Entities
{
    /// <summary>
    /// Sources Entities in the TrakHound Framework
    /// </summary>
    public static class TrakHoundSourcesEntity
    {

        public const string Source = "Source";
        public const string Metadata = "Metadata";


        public static bool IsSourcesEntity<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity))) return true;

            else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity))) return true;


            return false;
        }

        public static bool IsSourcesEntity(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(type)) return true;

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(type)) return true;

            }

            return false;
        }


        public static string GetEntityClass<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity)))
                return Source;

            else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return Metadata;


            return default;
        }

        public static byte GetEntityClassId<TEntity>() where TEntity : ITrakHoundEntity
        {

            if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundSourcesEntityClassId.Source;

            else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                return TrakHoundSourcesEntityClassId.Metadata;


            return 0;
        }

        public static IEnumerable<string> GetEntityClasses()
        {
            var classes = new List<string>();

            foreach (var entityClass in Enum.GetValues(typeof(TrakHoundSourcesEntityClass)))
            {
                classes.Add(entityClass.ToString());
            }

            return classes;
        }

        public static IEnumerable<byte> GetEntityClassIds()
        {
            var classes = new List<byte>();

            foreach (TrakHoundSourcesEntityClass entityClass in Enum.GetValues(typeof(TrakHoundSourcesEntityClass)))
            {
                classes.Add(TrakHoundSourcesEntityClassId.Get(entityClass));
            }

            return classes;
        }

        public static TEntity FromJson<TEntity>(string json) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(json))
            {

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundSourceEntity.FromJson(json);

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundSourceMetadataEntity.FromJson(json);

            }

            return default;
        }

        public static ITrakHoundEntity FromJson(byte entityClass, string json)
        {
            if (entityClass > 0 && !string.IsNullOrEmpty(json))
            {
                switch (entityClass)
                {

                    case TrakHoundSourcesEntityClassId.Source: return TrakHoundSourceEntity.FromJson(json);
                    case TrakHoundSourcesEntityClassId.Metadata: return TrakHoundSourceMetadataEntity.FromJson(json);
                }
            }

            return default;
        }


        public static string ToJson(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(type))
                    return new TrakHoundSourceEntity((ITrakHoundSourceEntity)entity).ToJson();

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(type))
                    return new TrakHoundSourceMetadataEntity((ITrakHoundSourceMetadataEntity)entity).ToJson();

            }

            return null;
        }


        public static TEntity FromArray<TEntity>(object[] a) where TEntity : ITrakHoundEntity
        {
            if (a != null)
            {

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundSourceEntity.FromArray(a);

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(typeof(TEntity)))
                    return (TEntity)TrakHoundSourceMetadataEntity.FromArray(a);

            }

            return default;
        }

        public static ITrakHoundEntity FromArray(string entityClass, object[] a)
        {
            if (!string.IsNullOrEmpty(entityClass) && !a.IsNullOrEmpty())
            {
                var x = entityClass.ToPascalCase().ConvertEnum<TrakHoundSourcesEntityClass>();
                switch (x)
                {

                    case TrakHoundSourcesEntityClass.Source: return TrakHoundSourceEntity.FromArray(a);
                    case TrakHoundSourcesEntityClass.Metadata: return TrakHoundSourceMetadataEntity.FromArray(a);
                }
            }

            return default;
        }


        public static object[] ToArray(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var type = entity.GetType();

                if (typeof(ITrakHoundSourceEntity).IsAssignableFrom(type))
                    return new TrakHoundSourceEntity((ITrakHoundSourceEntity)entity).ToArray();

                else if (typeof(ITrakHoundSourceMetadataEntity).IsAssignableFrom(type))
                    return new TrakHoundSourceMetadataEntity((ITrakHoundSourceMetadataEntity)entity).ToArray();

            }

            return null;
        }
    }
}
