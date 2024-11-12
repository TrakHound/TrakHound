// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundObjectLogEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        /// <summary>
        /// The UUID of the Object that this Entity is associated with
        /// </summary>
        string ObjectUuid { get; set; }

        int LogLevel { get; set; }

        string Message { get; set; }

        string Code { get; set; }

        string SourceUuid { get; set; }

        long Timestamp { get; set; }
    }
}
