// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    public static class TrakHoundEntityExtensions
    {
        public static IEnumerable<TEntity> ToDistinct<TEntity>(this IEnumerable<TEntity> entities) where TEntity : ITrakHoundEntity
        {
            if (!entities.IsNullOrEmpty())
            {
                var x = new Dictionary<string, TEntity>();
                foreach (var entity in entities)
                {
                    if (entity != null && entity.Uuid != null && x.GetValueOrDefault(entity.Uuid) == null)
                    {
                        x.TryAdd(entity.Uuid, entity);
                    }
                }
                return x.Select(o => o.Value);
            }

            return Enumerable.Empty<TEntity>();
        }


        //public static object Convert<TEntity>(this TEntity entity, Type type) where TEntity : ITrakHoundEntity
        //{
        //    if (entity != null)
        //    {
        //        try
        //        {
        //            var rObj = Activator.CreateInstance(type);

        //            // Get object's properties
        //            var sourceProperties = entity.GetType().GetProperties().ToList();
        //            var targetProperties = type.GetProperties().ToList();

        //            for (int i = 0; i < sourceProperties.Count; i++)
        //            {
        //                var sourceProperty = sourceProperties[i];
        //                var targetProperty = targetProperties.FirstOrDefault(o => o.Name == sourceProperty.Name);
        //                if (targetProperty != null && targetProperty.SetMethod != null)
        //                {
        //                    if (targetProperty.PropertyType == sourceProperty.PropertyType)
        //                    {
        //                        var sourceValue = sourceProperty.GetValue(entity);
        //                        if (sourceValue != null)
        //                        {
        //                            targetProperty.SetValue(rObj, sourceValue, null);
        //                        }
        //                    }
        //                }
        //            }

        //            return rObj;
        //        }
        //        catch (Exception) { }
        //    }

        //    return default;
        //}

        //public static IEnumerable<object> Convert<TEntity>(this IEnumerable<TEntity> entities, Type type) where TEntity : ITrakHoundEntity
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var x = new List<object>();
        //        foreach (var entity in entities)
        //        {
        //            var y = entity.Convert<TEntity>(type);
        //            if (y != null) x.Add(y);
        //        }
        //        return x;
        //    }

        //    return Enumerable.Empty<object>();
        //}


        //public static TOutput Convert<TEntity, TOutput>(this TEntity entity) where TEntity : ITrakHoundEntity
        //{
        //    if (entity != null)
        //    {
        //        try
        //        {
        //            var rObj = (TOutput)Activator.CreateInstance(typeof(TOutput));

        //            // Get object's properties
        //            var sourceProperties = entity.GetType().GetProperties().ToList();
        //            var targetProperties = typeof(TOutput).GetProperties().ToList();

        //            for (int i = 0; i < sourceProperties.Count; i++)
        //            {
        //                var sourceProperty = sourceProperties[i];
        //                var targetProperty = targetProperties.FirstOrDefault(o => o.Name == sourceProperty.Name);
        //                if (targetProperty != null && targetProperty.SetMethod != null)
        //                {
        //                    if (targetProperty.PropertyType == sourceProperty.PropertyType)
        //                    {
        //                        var sourceValue = sourceProperty.GetValue(entity);
        //                        if (sourceValue != null)
        //                        {
        //                            targetProperty.SetValue(rObj, sourceValue, null);
        //                        }
        //                    }
        //                }
        //            }

        //            return rObj;
        //        }
        //        catch (Exception) { }
        //    }

        //    return default;
        //}

        //public static IEnumerable<TOutput> Convert<TEntity, TOutput>(this IEnumerable<TEntity> entities) where TEntity : ITrakHoundEntity
        //{
        //    if (!entities.IsNullOrEmpty())
        //    {
        //        var x = new List<TOutput>();
        //        foreach (var entity in entities)
        //        {
        //            var y = entity.Convert<TEntity, TOutput>();
        //            if (y != null) x.Add(y);
        //        }
        //        return x;
        //    }

        //    return Enumerable.Empty<TOutput>();
        //}
    }
}
