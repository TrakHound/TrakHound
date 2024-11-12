// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Modules;
using TrakHound.Requests;

namespace TrakHound.Clients
{
    public class TrakHoundSourceMiddleware : ITrakHoundClientMiddleware, ITrakHoundEntitiesClientMiddleware
    {
        private const string _defaultId = "source";

        private readonly string _id;
        private readonly TrakHoundSourceChain _baseChain;
        private readonly string _sourceUuid;
        private bool _sourcePublished;


        public string Id => _id;

        public ITrakHoundClient Client { get; set; }


        public TrakHoundSourceMiddleware()
        {
            _id = _defaultId;

            _baseChain = new TrakHoundSourceChain();
            _baseChain.Add(TrakHoundSourceEntry.CreateApplicationSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateUserSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateDeviceSource());

            _sourceUuid = _baseChain.GetUuid();
        }

        public TrakHoundSourceMiddleware(string sourceUuid)
        {
            _id = _defaultId;
            _sourceUuid = sourceUuid;
        }

        public TrakHoundSourceMiddleware(string sourceType, string sourceSender)
        {
            _id = _defaultId;

            _baseChain = new TrakHoundSourceChain();
            _baseChain.Add(sourceType, sourceSender);
            _baseChain.Add(TrakHoundSourceEntry.CreateApplicationSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateUserSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateDeviceSource());

            _sourceUuid = _baseChain.GetUuid();
        }

        public TrakHoundSourceMiddleware(TrakHoundSourceEntry source)
        {
            _id = _defaultId;

            _baseChain = new TrakHoundSourceChain();
            _baseChain.Add(source);
            _baseChain.Add(TrakHoundSourceEntry.CreateApplicationSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateUserSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateDeviceSource());

            _sourceUuid = _baseChain.GetUuid();
        }

        public TrakHoundSourceMiddleware(TrakHoundSourceChain sourceChain)
        {
            _id = _defaultId;
            _baseChain = sourceChain;
            _sourceUuid = sourceChain?.GetUuid();
        }

        public TrakHoundSourceMiddleware(ITrakHoundModule module)
        {
            _id = _defaultId;

            _baseChain = new TrakHoundSourceChain();
            _baseChain.Add(TrakHoundSourceEntry.CreateModuleSource(module));
            _baseChain.Add(TrakHoundSourceEntry.CreateApplicationSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateUserSource());
            _baseChain.Add(TrakHoundSourceEntry.CreateDeviceSource());

            _sourceUuid = _baseChain.GetUuid();
        }


        public ITrakHoundEntity Process(ITrakHoundEntity entity, TrakHoundOperationMode operationMode)
        {
            if (entity != null && typeof(ITrakHoundSourcedEntity).IsAssignableFrom(entity.GetType()))
            {
                ((ITrakHoundSourcedEntity)entity).SetSource(_sourceUuid);
            }

            return entity;
        }

        public IEnumerable<ITrakHoundEntity> Process(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode)
        {
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity != null && typeof(ITrakHoundSourcedEntity).IsAssignableFrom(entity.GetType()))
                    {
                        ((ITrakHoundSourcedEntity)entity).SetSource(_sourceUuid);
                    }
                }
            }

            return entities;
        }


        public Task OnBeforePublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) => Task.CompletedTask;

        public Task OnAfterPublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) => Task.CompletedTask;


        public Task OnBeforePublish() => Task.CompletedTask;

        public async Task OnAfterPublish()
        {
            if (!_sourcePublished && Client != null && _baseChain != null)
            {
                _sourcePublished = true;

                var entry = _baseChain.GetEntry();
                if (entry != null)
                {
                    var entities = TrakHoundSourceEntry.GetEntities(entry);

                    if (await Client.System.Entities.Sources.Publish(entities, TrakHoundOperationMode.Async))
                    {

                    }
                }
            }
        }
    }
}
