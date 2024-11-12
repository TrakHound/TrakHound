// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to represent a Temporary Horizontal Relationship of one Object to another Object
    /// </summary>
    public interface ITrakHoundObjectAssignmentEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string AssigneeUuid { get; set; }

        string MemberUuid { get; set; }

        long AddTimestamp { get; set; }

        string AddSourceUuid { get; set; }

        long RemoveTimestamp { get; set; }

        string RemoveSourceUuid { get; set; }

        TimeSpan Duration { get; }
    }
}
