// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;
using TrakHound.Entities.Filters;

namespace TrakHound.Clients
{
    public class TrakHoundOnChangeMiddleware : ITrakHoundEntitiesClientMiddleware
    {
        private const string _defaultId = "entities";

        private readonly string _id;
        private readonly TrakHoundEntityOnChangeFilter _filter;


        public string Id => _id;

        public ITrakHoundClient Client { get; set; }


        public TrakHoundOnChangeMiddleware(string id = _defaultId)
        {
            _id = id;
            _filter = new TrakHoundEntityOnChangeFilter();
        }


        public ITrakHoundEntity Process(ITrakHoundEntity entity, TrakHoundOperationMode operationMode)
        {
            if (_filter.Filter(entity))
            {
                return entity;
            }

            return null;
        }

        public IEnumerable<ITrakHoundEntity> Process(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode)
        {
            if (!entities.IsNullOrEmpty())
            {
                var outputEntities = new List<ITrakHoundEntity>();
                foreach (var entity in entities)
                {
                    if (_filter.Filter(entity))
                    {
                        outputEntities.Add(entity);
                    }
                }
                return outputEntities;
            }

            return null;
        }

        public Task OnBeforePublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) => Task.CompletedTask;

        public Task OnAfterPublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode) => Task.CompletedTask;


        public Task OnBeforePublish() => Task.CompletedTask;

        public Task OnAfterPublish() => Task.CompletedTask;
    }
}
