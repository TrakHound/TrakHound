// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    /// <summary>
    /// Sources are used to record where data originated from in the TrakHound Framework
    /// </summary>
    public interface ITrakHoundSourceEntity : ITrakHoundEntity
    {
        string ParentUuid { get; set; }

        string Type { get; set; }

        string Sender { get; set; }
    }
}

