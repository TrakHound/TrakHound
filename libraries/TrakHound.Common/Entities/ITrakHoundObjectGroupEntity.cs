// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Used to represent a Horizontal Relationship of an Object to another Object
    /// </summary>
    public interface ITrakHoundObjectGroupEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string GroupUuid { get; set; }

        string MemberUuid { get; set; }

        string SourceUuid { get; set; }
    }
}
