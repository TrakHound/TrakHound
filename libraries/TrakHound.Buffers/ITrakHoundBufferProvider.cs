// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;

namespace TrakHound.Buffers
{
    public interface ITrakHoundBufferProvider
    {
        TrakHoundEntityPublishBuffer<TEntity> GetPublishBuffer<TEntity>(string driverConfigurationId) where TEntity : ITrakHoundEntity;

        TrakHoundEntityPublishBuffer<TEntity> AddPublishBuffer<TEntity>(IEntityPublishDriver<TEntity> driver) where TEntity : ITrakHoundEntity;


        TrakHoundEntityEmptyBuffer<TEntity> GetEmptyBuffer<TEntity>(string driverConfigurationId) where TEntity : ITrakHoundEntity;

        TrakHoundEntityEmptyBuffer<TEntity> AddEmptyBuffer<TEntity>(IEntityEmptyDriver<TEntity> driver) where TEntity : ITrakHoundEntity;


        TrakHoundEntityDeleteBuffer<TEntity> GetDeleteBuffer<TEntity>(string driverConfigurationId) where TEntity : ITrakHoundEntity;

        TrakHoundEntityDeleteBuffer<TEntity> AddDeleteBuffer<TEntity>(IEntityDeleteDriver<TEntity> driver) where TEntity : ITrakHoundEntity;


        TrakHoundEntityIndexBuffer<TEntity> GetIndexBuffer<TEntity>(string driverConfigurationId) where TEntity : ITrakHoundEntity;

        TrakHoundEntityIndexBuffer<TEntity> AddIndexBuffer<TEntity>(IEntityIndexUpdateDriver<TEntity> driver) where TEntity : ITrakHoundEntity;


        IEnumerable<TrakHoundBufferMetrics> GetBufferMetrics();

        TrakHoundBufferMetrics GetBufferMetrics(string bufferId);
    }
}
