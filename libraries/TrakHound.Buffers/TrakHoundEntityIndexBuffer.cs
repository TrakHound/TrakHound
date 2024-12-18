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
    public class TrakHoundEntityIndexBuffer<TEntity> : TrakHoundOperationBuffer<EntityIndexPublishRequest> where TEntity : ITrakHoundEntity
    {
        private const string _operationType = "Index";
        private readonly IEntityIndexUpdateDriver<TEntity> _driver;


        public TrakHoundEntityIndexBuffer(IEntityIndexUpdateDriver<TEntity> driver, ITrakHoundLogProvider logProvider) 
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

            return $"{driver.Configuration.Id}.index-update.{entityCategory}.{entityClass}";
        }

        public static string GetBufferId(string driverConfigurationId)
        {
            var entityCategoryId = TrakHoundEntity.GetEntityCategoryId<TEntity>();
            var entityClassId = TrakHoundEntity.GetEntityClassId<TEntity>();

            var entityCategory = TrakHoundEntityCategoryName.Get(entityCategoryId).ToKebabCase();
            var entityClass = TrakHoundEntity.GetEntityClassName(entityCategoryId, entityClassId).ToKebabCase();

            return $"{driverConfigurationId}.index-update.{entityCategory}.{entityClass}";
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

        private async Task<bool> Update(ReadOnlyMemory<EntityIndexPublishRequest> items)
        {
            var requests = new List<EntityIndexPublishRequest>(items.Length);
            for (var i = 0; i < items.Length; i++)
            {
                requests.Add(items.Span[i]);
            }

            var response = await _driver.UpdateIndex(requests);
            return response.IsSuccess;
        }

        private static EntityIndexPublishRequest Deserialize(ReadOnlyMemory<byte> bytes)
        {
            var request = EntityIndexPublishRequest.FromJson(bytes.Span);
            if (request.IsValid)
            {
                return request;
            }

            return default;
        }

        private static int Serialize(Stream stream, EntityIndexPublishRequest request)
        {
            if (request.IsValid)
            {
                return EntityIndexPublishRequest.WriteJson(stream, request);
            }

            return 0;
        }
    }
}
