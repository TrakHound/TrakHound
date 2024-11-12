// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using TrakHound.Entities;
using TrakHound.Memory;

namespace TrakHound.Drivers.Memory
{
    public class DefinitionWikiDriver : MemoryEntityDriver<ITrakHoundDefinitionWikiEntity>
    {
        public DefinitionWikiDriver(ITrakHoundDriverConfiguration configuration) : base(configuration) { }

        protected override bool PublishCompare(ITrakHoundDefinitionWikiEntity newEntity, ITrakHoundDefinitionWikiEntity existingEntity)
        {
            return existingEntity != null ? newEntity.Created > existingEntity.Created : true;
        }

    }
}
