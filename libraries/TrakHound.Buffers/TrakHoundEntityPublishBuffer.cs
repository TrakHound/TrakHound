// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Buffers
{
    public class TrakHoundEntityPublishBuffer<TEntity> : TrakHoundOperationBuffer<TEntity> where TEntity : ITrakHoundEntity
    {
        private const string _operationType = "Publish";
        private readonly IEntityPublishDriver<TEntity> _driver;


        public TrakHoundEntityPublishBuffer(IEntityPublishDriver<TEntity> driver) : base(GetBufferId(driver), driver.Configuration.Id, GetBufferName(), _operationType, driver.Configuration.Buffer)
        {
            _driver = driver;

            AvailabilityFunction = CheckAvailability;
            ProcessFunction = Publish;
            DeserializeFunction = Deserialize;
            SerializeFunction = Serialize;

            Start();
        }

        public static string GetBufferId(ITrakHoundDriver driver)
        {
            var entityCategoryId = TrakHoundEntity.GetEntityCategoryId<TEntity>();
            var entityClassId = TrakHoundEntity.GetEntityClassId<TEntity>();

            var entityCategory = TrakHoundEntityCategoryName.Get(entityCategoryId).ToKebabCase();
            var entityClass = TrakHoundEntity.GetEntityClassName(entityCategoryId, entityClassId).ToKebabCase();

            return $"{driver.Configuration.Id}.publish.{entityCategory}.{entityClass}";
        }

        public static string GetBufferId(string driverConfigurationId)
        {
            var entityCategoryId = TrakHoundEntity.GetEntityCategoryId<TEntity>();
            var entityClassId = TrakHoundEntity.GetEntityClassId<TEntity>();

            var entityCategory = TrakHoundEntityCategoryName.Get(entityCategoryId).ToKebabCase();
            var entityClass = TrakHoundEntity.GetEntityClassName(entityCategoryId, entityClassId).ToKebabCase();

            return $"{driverConfigurationId}.publish.{entityCategory}.{entityClass}";
        }

        public static string GetBufferName()
        {
            var entityCategoryId = TrakHoundEntity.GetEntityCategoryId<TEntity>();
            var entityClassId = TrakHoundEntity.GetEntityClassId<TEntity>();

            var entityCategory = TrakHoundEntityCategoryName.Get(entityCategoryId).ToPascalCase();
            var entityClass = TrakHoundEntity.GetEntityClassName(entityCategoryId, entityClassId).ToPascalCase();

            return $"{entityCategory}.{entityClass}";
        }

        private bool CheckAvailability()
        {
            return _driver.IsAvailable;
        }

        private async Task<bool> Publish(ReadOnlyMemory<TEntity> items)
        {
            var entities = new List<TEntity>(items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                entities.Add(items.Span[i]);
            }

            var response = await _driver.Publish(entities.ToDistinct());
            return response.IsSuccess;
        }

        private static TEntity Deserialize(ReadOnlyMemory<byte> bytes)
        {
            if (bytes.Length > 0)
            {
                var json = System.Text.Encoding.UTF8.GetString(bytes.Span);
                var entity = TrakHoundEntity.FromJson<TEntity>(json);
                if (entity != null && entity.IsValid)
                {
                    return entity;
                }
            }

            return default;
        }

        private static int Serialize(Stream stream, TEntity entity)
        {
            if (entity.IsValid)
            {
                var json = entity.ToJson();
                if (!string.IsNullOrEmpty(json))
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    stream.Write(bytes);

                    return bytes.Length;
                }
            }

            return 0;
        }
    }
}
