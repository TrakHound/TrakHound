// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrakHound.Clients;
using TrakHound.Entities;
using TrakHound.Instances;

namespace TrakHound.Http.Entities
{
    [ApiController]
    [Route($"{HttpConstants.EntitiesPrefix}/definitions")]
    public abstract class TrakHoundDefinitionControllerBase : TrakHoundHttpEntityControllerBase<ITrakHoundDefinitionEntity>
    {
        public TrakHoundDefinitionControllerBase(ITrakHoundInstance instance, TrakHoundHttpWebSocketManager webSocketManager) : base(instance, webSocketManager) { }

    }
}
