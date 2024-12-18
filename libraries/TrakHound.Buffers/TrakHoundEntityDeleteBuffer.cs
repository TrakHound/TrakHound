// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TrakHound.Drivers;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Logging;

namespace TrakHound.Buffers
{
    public class TrakHoundEntityDeleteBuffer<TEntity> : TrakHoundOperationBuffer<EntityDeleteRequest> where TEntity : ITrakHoundEntity
    {
        private const string _operationType = "Delete";
        private readonly IEntityDeleteDriver<TEntity> _driver;


        public TrakHoundEntityDeleteBuffer(IEntityDeleteDriver<TEntity> driver, ITrakHoundLogProvider logProvider)
            : base(GetBufferId(driver), driver.Configuration.Id, logProvider, GetBufferName(), _operationType, driver.Configuration.Buffer)
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

            return $"{driver.Configuration.Id}.delete.{entityCategory}.{entityClass}";
        }

        public static string GetBufferId(string driverConfigurationId)
        {
            var entityCategoryId = TrakHoundEntity.GetEntityCategoryId<TEntity>();
            var entityClassId = TrakHoundEntity.GetEntityClassId<TEntity>();

            var entityCategory = TrakHoundEntityCategoryName.Get(entityCategoryId).ToKebabCase();
            var entityClass = TrakHoundEntity.GetEntityClassName(entityCategoryId, entityClassId).ToKebabCase();

            return $"{driverConfigurationId}.delete.{entityCategory}.{entityClass}";
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

        private async Task<bool> Update(ReadOnlyMemory<EntityDeleteRequest> items)
        {
            var requests = new List<EntityDeleteRequest>(items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                requests.Add(items.Span[i]);
            }

            var response = await _driver.Delete(requests);
            return response.IsSuccess;
        }

        private static EntityDeleteRequest Deserialize(ReadOnlyMemory<byte> bytes)
        {
            return EntityDeleteRequest.FromJson(bytes.Span);
        }

        private static int Serialize(Stream stream, EntityDeleteRequest request)
        {
            return EntityDeleteRequest.WriteJson(stream, request);
        }
    }
}
