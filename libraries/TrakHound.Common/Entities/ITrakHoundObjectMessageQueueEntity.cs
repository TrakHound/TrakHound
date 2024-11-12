// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Represents an Entity that is added to an Immutable FIFO Queue of another Object
    /// </summary>
    public interface ITrakHoundObjectMessageQueueEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; set; }

        string QueueId { get; set; }

        string ContentType { get; set; }

        string SourceUuid { get; set; }
    }
}
