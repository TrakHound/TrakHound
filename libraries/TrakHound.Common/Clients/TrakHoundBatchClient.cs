// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public class TrakHoundBatchClient
    {
        private const int _defaultBatchSize = 5000;


        private readonly object _lock = new object();
        private readonly ITrakHoundClient _client;
        private readonly string _routerId;
        private readonly ITrakHoundEntity[] _entityBuffer;
        private readonly ITrakHoundEntityEntryOperation[] _entryBuffer;
        private readonly EntityIndexPublishRequest[] _indexBuffer;
        private readonly int _bufferSize;
        private int _entityBufferIndex;
        private int _entryBufferIndex;
        private int _indexBufferIndex;


        public event EventHandler<int> Published;

        public event EventHandler<int> Error;


        public TrakHoundBatchClient(ITrakHoundClient client, int batchSize = _defaultBatchSize, string routerId = null)
        {
            _client = client;
            _routerId = routerId;
            _entityBuffer = new ITrakHoundEntity[batchSize];
            _entryBuffer = new ITrakHoundEntityEntryOperation[batchSize];
            _indexBuffer = new EntityIndexPublishRequest[batchSize];
            _bufferSize = batchSize;
        }

        public async Task Add(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var write = false;

                lock (_lock)
                {
                    _entityBuffer[_entityBufferIndex] = entity;
                    _entityBufferIndex++;

                    write = _entityBufferIndex == _bufferSize - 1;
                }

                if (write) await FlushEntities();
            }
        }

        public async Task Add(IEnumerable<ITrakHoundEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    await Add(entity);
                }
            }
        }

        public async Task Add(TrakHoundEntityCollection collection)
        {
            if (collection != null)
            {
                await Add(collection.GetEntities());
            }
        }

        public async Task Add(ITrakHoundEntityEntryOperation operation)
        {
            if (operation != null)
            {
                var write = false;

                lock (_lock)
                {
                    _entryBuffer[_entryBufferIndex] = operation;
                    _entryBufferIndex++;

                    write = _entryBufferIndex == _bufferSize - 1;
                }

                if (write) await FlushEntries();
            }
        }

        public async Task Add(IEnumerable<ITrakHoundEntityEntryOperation> operations)
        {
            if (!operations.IsNullOrEmpty())
            {
                foreach (var operation in operations)
                {
                    await Add(operation);
                }
            }
        }

        public async Task Add(TrakHoundEntityTransaction transaction)
        {
            if (transaction != null && transaction.PublishOperations != null)
            {
                await Add(transaction.PublishOperations.GetAllOperations());
            }
        }

        public async Task Add(EntityIndexPublishRequest request)
        {
            if (request.IsValid)
            {
                var write = false;

                lock (_lock)
                {
                    _indexBuffer[_indexBufferIndex] = request;
                    _indexBufferIndex++;

                    write = _indexBufferIndex == _bufferSize - 1;
                }

                if (write) await FlushIndexes();
            }
        }

        public async Task Add(IEnumerable<EntityIndexPublishRequest> requests)
        {
            if (!requests.IsNullOrEmpty())
            {
                foreach (var request in requests)
                {
                    await Add(request);
                }
            }
        }


        public async Task<bool> Flush()
        {
            bool success;
            success = await FlushEntities();
            if (success) success = await FlushEntries();
            if (success) success = await FlushIndexes();
            return success;
        }

        public async Task<bool> FlushEntities()
        {
            ITrakHoundEntity[] publishEntities;
            int index;
            int count;

            lock (_lock)
            {
                index = _entityBufferIndex;
                count = _entityBufferIndex;
                publishEntities = new ITrakHoundEntity[count];
                Array.Copy(_entityBuffer, 0, publishEntities, 0, count);
                _entityBufferIndex = 0;
            }

            if (index > 0)
            {
                if (await _client.System.Entities.Publish(publishEntities, TrakHoundOperationMode.Async, _routerId))
                {
                    if (Published != null) Published.Invoke(this, count);
                    return true;
                }
                else
                {
                    if (Error != null) Error.Invoke(this, count);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> FlushEntries()
        {
            ITrakHoundEntityEntryOperation[] publishEntries;
            int index;
            int count;

            lock (_lock)
            {
                index = _entryBufferIndex;
                count = _entryBufferIndex;
                publishEntries = new ITrakHoundEntityEntryOperation[count];
                Array.Copy(_entryBuffer, 0, publishEntries, 0, count);
                _entryBufferIndex = 0;
            }

            if (index > 0)
            {
                var transaction = new TrakHoundEntityTransaction();
                transaction.PublishOperations = new TrakHoundEntityPublishTransaction();
                foreach (var entry in publishEntries) transaction.PublishOperations.Add(entry);

                if (await _client.Entities.Publish(transaction, true, _routerId))
                {
                    if (Published != null) Published.Invoke(this, count);
                    return true;
                }
                else
                {
                    if (Error != null) Error.Invoke(this, count);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> FlushIndexes()
        {
            EntityIndexPublishRequest[] publishRequests;
            int index;
            int count;

            lock (_lock)
            {
                index = _indexBufferIndex;
                count = _indexBufferIndex;
                publishRequests = new EntityIndexPublishRequest[count];
                Array.Copy(_indexBuffer, 0, publishRequests, 0, count);
                _indexBufferIndex = 0;
            }

            if (index > 0)
            {
                if (await _client.System.Entities.Objects.UpdateIndex(publishRequests, TrakHoundOperationMode.Async, _routerId))
                {
                    if (Published != null) Published.Invoke(this, count);
                    return true;
                }
                else
                {
                    if (Error != null) Error.Invoke(this, count);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
