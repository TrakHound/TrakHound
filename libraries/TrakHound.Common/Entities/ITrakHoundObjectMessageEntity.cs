// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public interface ITrakHoundObjectMessageEntity : ITrakHoundEntity, ITrakHoundSourcedEntity
    {
        string ObjectUuid { get; set; }

        string BrokerId { get; set; }

        string Topic { get; set; }

        string ContentType { get; set; }

        bool Retain { get; set; }

        int Qos { get; set; }

        string SourceUuid { get; set; }
    }
}
