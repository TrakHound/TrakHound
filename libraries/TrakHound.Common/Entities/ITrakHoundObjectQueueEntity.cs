// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Represents an Object that is added to a FIFO Queue of another Object
    /// </summary>
    public interface ITrakHoundObjectQueueEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string QueueUuid { get; set; }

        string MemberUuid { get; set; }

        int Index { get; set; }

        string SourceUuid { get; set; }

        long Timestamp { get; set; }
    }
}
