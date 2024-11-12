// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities.Collections
{
    public partial class TrakHoundObjectCollection
    {
        public ITrakHoundObjectMetadataEntity QueryMetadataByEntityUuid(string entityUuid, string name)
        {
            if (!string.IsNullOrEmpty(entityUuid))
            {
                var uuid = TrakHoundObjectMetadataEntity.GenerateUuid(entityUuid, name);
                if (uuid != null)
                {
                    return _metadata.GetValueOrDefault(uuid);
                }
            }

            return null;
        }
    }
}
