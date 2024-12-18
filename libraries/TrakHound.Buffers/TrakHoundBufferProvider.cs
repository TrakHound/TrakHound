// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers.Entities;
using TrakHound.Entities;
using TrakHound.Logging;

namespace TrakHound.Buffers
{
    public class TrakHoundBufferProvider : ITrakHoundBufferProvider, IDisposable
    {
        private const int _metricsInterval = 1000;

        private readonly ITrakHoundLogProvider _logProvider;
        private readonly Dictionary<string, ITrakHoundOperationBuffer> _buffers = new Dictionary<string, ITrakHoundOperationBuffer>();
        private readonly Dictionary<string, TrakHoundBufferMetrics> _metrics = new Dictionary<string, TrakHoundBufferMetrics>();
        private readonly System.Timers.Timer _metricsTimer;
        private readonly object _lock = new object();


        public TrakHoundBufferProvider(ITrakHoundLogProvider logProvider)
        {
            _logProvider = logProvider;

            _metricsTimer = new System.Timers.Timer();
            _metricsTimer.Interval = _metricsInterval;
            _metricsTimer.Elapsed += MetricsTimerElapsed;
            _metricsTimer.Start();
        }

        public void Dispose()
        {
            _metricsTimer.Stop();
            _metricsTimer.Dispose();

            IEnumerable<ITrakHoundOperationBuffer> buffers;
            lock (_lock) buffers = _buffers.Values;

            if (!buffers.IsNullOrEmpty())
            {
                foreach (var buffer in buffers)
                {
                    buffer.Dispose();
                }
            }
        }


        private void MetricsTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lock)
            {
                var buffers = _buffers.Values;
                if (!buffers.IsNullOrEmpty())
                {
                    foreach (var buffer in buffers)
                    {
                        if (!string.IsNullOrEmpty(buffer.Id))
                        {
                            _metrics.Remove(buffer.Id);
                            _metrics.Add(buffer.Id, buffer.Metrics);
                        }
                    }
                }
            }
        }


        public TrakHoundEntityPublishBuffer<TEntity> GetPublishBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                var bufferId = TrakHoundEntityPublishBuffer<TEntity>.GetBufferId(configurationId);
                return (TrakHoundEntityPublishBuffer<TEntity>)GetBuffer<TEntity>(bufferId);
            }

            return null;
        }

        public TrakHoundEntityPublishBuffer<TEntity> AddPublishBuffer<TEntity>(IEntityPublishDriver<TEntity> driver)
            where TEntity : ITrakHoundEntity
        {
            if (driver != null && driver.Configuration != null)
            {
                var bufferId = TrakHoundEntityPublishBuffer<TEntity>.GetBufferId(driver);
                if (!BufferExists(bufferId))
                {
                    var buffer = new TrakHoundEntityPublishBuffer<TEntity>(driver, _logProvider);
                    AddBuffer(buffer);

                    return (TrakHoundEntityPublishBuffer<TEntity>)GetBuffer<TEntity>(bufferId);
                }
            }

            return null;
        }


        public TrakHoundEntityEmptyBuffer<TEntity> GetEmptyBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                var bufferId = TrakHoundEntityEmptyBuffer<TEntity>.GetBufferId(configurationId);
                return (TrakHoundEntityEmptyBuffer<TEntity>)GetBuffer<EntityEmptyRequest>(bufferId);
            }

            return null;
        }

        public TrakHoundEntityEmptyBuffer<TEntity> AddEmptyBuffer<TEntity>(IEntityEmptyDriver<TEntity> driver) where TEntity : ITrakHoundEntity
        {
            if (driver != null && driver.Configuration != null)
            {
                var bufferId = TrakHoundEntityEmptyBuffer<TEntity>.GetBufferId(driver);
                if (!BufferExists(bufferId))
                {
                    var buffer = new TrakHoundEntityEmptyBuffer<TEntity>(driver, _logProvider);
                    AddBuffer(buffer);

                    return (TrakHoundEntityEmptyBuffer<TEntity>)GetBuffer<EntityEmptyRequest>(bufferId);
                }
            }

            return null;
        }


        public TrakHoundEntityDeleteBuffer<TEntity> GetDeleteBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                var bufferId = TrakHoundEntityDeleteBuffer<TEntity>.GetBufferId(configurationId);
                return (TrakHoundEntityDeleteBuffer<TEntity>)GetBuffer<EntityDeleteRequest>(bufferId);
            }

            return null;
        }

        public TrakHoundEntityDeleteBuffer<TEntity> AddDeleteBuffer<TEntity>(IEntityDeleteDriver<TEntity> driver) where TEntity : ITrakHoundEntity
        {
            if (driver != null && driver.Configuration != null)
            {
                var bufferId = TrakHoundEntityDeleteBuffer<TEntity>.GetBufferId(driver);
                if (!BufferExists(bufferId))
                {
                    var buffer = new TrakHoundEntityDeleteBuffer<TEntity>(driver, _logProvider);
                    AddBuffer(buffer);

                    return (TrakHoundEntityDeleteBuffer<TEntity>)GetBuffer<EntityDeleteRequest>(bufferId);
                }
            }

            return null;
        }


        public TrakHoundEntityIndexBuffer<TEntity> GetIndexBuffer<TEntity>(string configurationId) where TEntity : ITrakHoundEntity
        {
            if (!string.IsNullOrEmpty(configurationId))
            {
                var bufferId = TrakHoundEntityIndexBuffer<TEntity>.GetBufferId(configurationId);
                return (TrakHoundEntityIndexBuffer<TEntity>)GetBuffer<EntityIndexPublishRequest>(bufferId);
            }

            return null;
        }

        public TrakHoundEntityIndexBuffer<TEntity> AddIndexBuffer<TEntity>(IEntityIndexUpdateDriver<TEntity> driver) where TEntity : ITrakHoundEntity
        {
            if (driver != null && driver.Configuration != null)
            {
                var bufferId = TrakHoundEntityIndexBuffer<TEntity>.GetBufferId(driver);
                if (!BufferExists(bufferId))
                {
                    var buffer = new TrakHoundEntityIndexBuffer<TEntity>(driver, _logProvider);
                    AddBuffer(buffer);

                    return (TrakHoundEntityIndexBuffer<TEntity>)GetBuffer<EntityIndexPublishRequest>(bufferId);
                }
            }

            return null;
        }


        public bool BufferExists(string bufferId)
        {
            if (!string.IsNullOrEmpty(bufferId))
            {
                lock (_lock)
                {
                    return _buffers.ContainsKey(bufferId);
                }
            }

            return false;
        }

        public void AddBuffer(ITrakHoundOperationBuffer buffer)
        {
            if (buffer != null && !string.IsNullOrEmpty(buffer.Id))
            {
                lock (_lock)
                {
                    if (!_buffers.ContainsKey(buffer.Id))
                    {
                        _buffers.Add(buffer.Id, buffer);
                    }
                }
            }
        }

        public ITrakHoundOperationBuffer GetBuffer(string bufferId)
        {
            if (!string.IsNullOrEmpty(bufferId))
            {
                lock (_lock)
                {
                    return _buffers.GetValueOrDefault(bufferId);
                }
            }

            return null;
        }

        public ITrakHoundOperationBuffer<TItem> GetBuffer<TItem>(string bufferId)
        {
            if (!string.IsNullOrEmpty(bufferId))
            {
                lock (_lock)
                {
                    var buffer = _buffers.GetValueOrDefault(bufferId);
                    if (buffer != null && typeof(ITrakHoundOperationBuffer<TItem>).IsAssignableFrom(buffer.GetType()))
                    {
                        return (ITrakHoundOperationBuffer<TItem>)buffer;
                    }
                }
            }

            return null;
        }

        public void RemoveBuffer(string bufferId)
        {
            if (!string.IsNullOrEmpty(bufferId))
            {
                lock (_lock)
                {
                    _buffers.Remove(bufferId);
                }
            }
        }


        public IEnumerable<TrakHoundBufferMetrics> GetBufferMetrics()
        {
            lock (_lock)
            {
                if (!_buffers.IsNullOrEmpty())
                {
                    var i = 0;
                    var metrics = new TrakHoundBufferMetrics[_buffers.Count];
                    foreach (var buffer in _buffers)
                    {
                        metrics[i] = buffer.Value.Metrics;
                        i++;
                    }
                    return metrics;
                }
            }

            return null;
        }

        public TrakHoundBufferMetrics GetBufferMetrics(string bufferId)
        {
            if (!string.IsNullOrEmpty(bufferId))
            {
                lock (_lock)
                {
                    var buffer = _buffers.GetValueOrDefault(bufferId);
                    if (buffer != null)
                    {
                        return buffer.Metrics;
                    }
                }
            }

            return null;
        }
    }
}
