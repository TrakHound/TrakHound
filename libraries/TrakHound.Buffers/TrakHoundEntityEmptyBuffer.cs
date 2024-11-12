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
    public class TrakHoundEntityEmptyBuffer<TEntity> : TrakHoundOperationBuffer<EntityEmptyRequest> where TEntity : ITrakHoundEntity
    {
        private const string _operationType = "Empty";
        private readonly IEntityEmptyDriver<TEntity> _driver;


        public TrakHoundEntityEmptyBuffer(IEntityEmptyDriver<TEntity> driver) : base(GetBufferId(driver), driver.Configuration.Id, GetBufferName(), _operationType, driver.Configuration.Buffer)
        {
            _driver = driver;

            AvailabilityFunction = CheckAvailability;
            ProcessFunction = Update;
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

            return $"{driver.Configuration.Id}.empty.{entityCategory}.{entityClass}";
        }

        public static string GetBufferId(string driverConfigurationId)
        {
            var entityCategoryId = TrakHoundEntity.GetEntityCategoryId<TEntity>();
            var entityClassId = TrakHoundEntity.GetEntityClassId<TEntity>();

            var entityCategory = TrakHoundEntityCategoryName.Get(entityCategoryId).ToKebabCase();
            var entityClass = TrakHoundEntity.GetEntityClassName(entityCategoryId, entityClassId).ToKebabCase();

            return $"{driverConfigurationId}.empty.{entityCategory}.{entityClass}";
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

        private async Task<bool> Update(ReadOnlyMemory<EntityEmptyRequest> items)
        {
            var requests = new List<EntityEmptyRequest>(items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                requests.Add(items.Span[i]);
            }

            var response = await _driver.Empty(requests);
            return response.IsSuccess;
        }

        private static EntityEmptyRequest Deserialize(ReadOnlyMemory<byte> bytes)
        {
            return EntityEmptyRequest.FromJson(bytes.Span);
        }

        private static int Serialize(Stream stream, EntityEmptyRequest request)
        {
            return EntityEmptyRequest.WriteJson(stream, request);
        }
    }
}
