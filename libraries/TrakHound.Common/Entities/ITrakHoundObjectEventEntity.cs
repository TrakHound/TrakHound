// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Events are used to record a Reference to a Target Object at a specified Timestamp. 
    /// </summary>
    public interface ITrakHoundObjectEventEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; set; }

        string TargetUuid { get; set; }

        long Timestamp { get; set; }

        string SourceUuid { get; set; }
    }
}
