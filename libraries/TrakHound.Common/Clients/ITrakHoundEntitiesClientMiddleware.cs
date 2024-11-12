// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Entities;

namespace TrakHound.Clients
{
    public interface ITrakHoundEntitiesClientMiddleware : ITrakHoundClientMiddleware
    {
        ITrakHoundEntity Process(ITrakHoundEntity entity, TrakHoundOperationMode operationMode);

        IEnumerable<ITrakHoundEntity> Process(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode);


        Task OnBeforePublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode);

        Task OnAfterPublish(IEnumerable<ITrakHoundEntity> entities, TrakHoundOperationMode operationMode);
    }
}
