// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace TrakHound.Entities
{
    /// <summary>
    /// The State of an Object at a specific time
    /// </summary>
    public interface ITrakHoundObjectStateEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; set; }

        string DefinitionUuid { get; set; }

        long TTL { get; set; }

        string SourceUuid { get; set; }

        long Timestamp { get; set; }
    }
}

