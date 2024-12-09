// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Collections;
using TrakHound.Http;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public class TrakHoundHttpPublishStreamClient : IDisposable
    {
        private readonly object _lock = new object();
        private readonly TrakHoundHttpClientConfiguration _clientConfiguration;
        private readonly string _routerId;
        private readonly ITrakHoundEntity[] _entityBuffer;
        private readonly ITrakHoundEntityEntryOperation[] _entryBuffer;
        private readonly EntityIndexPublishRequest[] _indexBuffer;
        private readonly int _bufferSize;
        private int _entityBufferIndex;
        private int _entryBufferIndex;
        private int _indexBufferIndex;

        private TrakHoundWebSocketsClient _entitiesClient;
        private TrakHoundWebSocketsClient _entriesClient;


        public event EventHandler<int> Published;

        public event EventHandler<int> Error;


        public TrakHoundHttpPublishStreamClient(TrakHoundHttpClientConfiguration clientConfiguration, string routerId = null)
        {
            _clientConfiguration = clientConfiguration;
            _routerId = routerId;
        }


        public void Connect()
        {
            // Entities API (System)
            var entitiesUrl = Url.Combine(_clientConfiguration.GetWebSocketBaseUrl(), "_entities/publish");
            _entitiesClient = new TrakHoundWebSocketsClient(entitiesUrl);
            _entitiesClient.Start();

            // Entries API (TrakHound.Entities.Api)
            var entriesUrl = Url.Combine(_clientConfiguration.GetWebSocketBaseUrl(), "entities/entries/publish");
            _entriesClient = new TrakHoundWebSocketsClient(entriesUrl);
            _entriesClient.Start();
        }

        public void Disconnect()
        {
            if (_entitiesClient != null) _entitiesClient.Stop();
            if (_entriesClient != null) _entriesClient.Stop();
        }

        public void Dispose()
        {
            if (_entitiesClient != null) _entitiesClient.Dispose();
            if (_entriesClient != null) _entriesClient.Dispose();
        }


        public async Task<bool> Publish(ITrakHoundEntity entity)
        {
            if (entity != null)
            {
                var collection = new TrakHoundEntityCollection();
                collection.Add(entity);
                return await Publish(collection);
            }

            return false;
        }

        public async Task<bool> Publish(IEnumerable<ITrakHoundEntity> entities)
        {
            if (!entities.IsNullOrEmpty())
            {
                var collection = new TrakHoundEntityCollection();
                collection.Add(entities);
                return await Publish(collection);
            }

            return false;
        }

        public async Task<bool> Publish(TrakHoundEntityCollection collection)
        {
            if (collection != null)
            {
                var jsonCollection = new TrakHoundJsonEntityCollection(collection);
                var json = jsonCollection.ToJson(false);
                var jsonBytes = json.ToUtf8Bytes();
                if (jsonBytes != null)
                {
                    return await _entitiesClient.Send(jsonBytes);
                }
            }

            return false;
        }


        public async Task<bool> Publish(ITrakHoundEntityEntryOperation operation)
        {
            if (operation != null)
            {
                var transaction = new TrakHoundEntityTransaction();
                transaction.PublishOperations = new TrakHoundEntityPublishTransaction();
                transaction.PublishOperations.Add(operation);

                return await Publish(transaction);
            }

            return false;
        }

        public async Task<bool> Publish(IEnumerable<ITrakHoundEntityEntryOperation> operations)
        {
            if (!operations.IsNullOrEmpty())
            {
                var transaction = new TrakHoundEntityTransaction();
                transaction.PublishOperations = new TrakHoundEntityPublishTransaction();
                foreach (var operation in operations)
                {
                    transaction.PublishOperations.Add(operation);
                }

                return await Publish(transaction);
            }

            return false;
        }

        public async Task<bool> Publish(TrakHoundEntityTransaction transaction)
        {
            if (transaction != null && transaction.PublishOperations != null)
            {
                var json = transaction.ToJson(false);
                var jsonBytes = json.ToUtf8Bytes();
                if (jsonBytes != null)
                {
                    return await _entriesClient.Send(jsonBytes);
                }
            }

            return false;
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

                //if (write) await FlushIndexes();
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


        //public async Task<bool> Flush()
        //{
        //    bool success;
        //    success = await FlushEntities();
        //    if (success) success = await FlushEntries();
        //    if (success) success = await FlushIndexes();
        //    return success;
        //}

        //public async Task<bool> FlushEntities()
        //{
        //    ITrakHoundEntity[] publishEntities;
        //    int index;
        //    int count;

        //    lock (_lock)
        //    {
        //        index = _entityBufferIndex;
        //        count = _entityBufferIndex;
        //        publishEntities = new ITrakHoundEntity[count];
        //        Array.Copy(_entityBuffer, 0, publishEntities, 0, count);
        //        _entityBufferIndex = 0;
        //    }

        //    if (index > 0)
        //    {
        //        if (await _client.System.Entities.Publish(publishEntities, TrakHoundOperationMode.Async, _routerId))
        //        {
        //            if (Published != null) Published.Invoke(this, count);
        //            return true;
        //        }
        //        else
        //        {
        //            if (Error != null) Error.Invoke(this, count);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public async Task<bool> FlushEntries()
        //{
        //    ITrakHoundEntityEntryOperation[] publishEntries;
        //    int index;
        //    int count;

        //    lock (_lock)
        //    {
        //        index = _entryBufferIndex;
        //        count = _entryBufferIndex;
        //        publishEntries = new ITrakHoundEntityEntryOperation[count];
        //        Array.Copy(_entryBuffer, 0, publishEntries, 0, count);
        //        _entryBufferIndex = 0;
        //    }

        //    if (index > 0)
        //    {
        //        var transaction = new TrakHoundEntityTransaction();
        //        transaction.PublishOperations = new TrakHoundEntityPublishTransaction();
        //        foreach (var entry in publishEntries) transaction.PublishOperations.Add(entry);

        //        if (await _client.Entities.Publish(transaction, true, _routerId))
        //        {
        //            if (Published != null) Published.Invoke(this, count);
        //            return true;
        //        }
        //        else
        //        {
        //            if (Error != null) Error.Invoke(this, count);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public async Task<bool> FlushIndexes()
        //{
        //    EntityIndexPublishRequest[] publishRequests;
        //    int index;
        //    int count;

        //    lock (_lock)
        //    {
        //        index = _indexBufferIndex;
        //        count = _indexBufferIndex;
        //        publishRequests = new EntityIndexPublishRequest[count];
        //        Array.Copy(_indexBuffer, 0, publishRequests, 0, count);
        //        _indexBufferIndex = 0;
        //    }

        //    if (index > 0)
        //    {
        //        if (await _client.System.Entities.Objects.UpdateIndex(publishRequests, TrakHoundOperationMode.Async, _routerId))
        //        {
        //            if (Published != null) Published.Invoke(this, count);
        //            return true;
        //        }
        //        else
        //        {
        //            if (Error != null) Error.Invoke(this, count);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
    }
}
