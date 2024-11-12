// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundAcknowledgeResult<T>
    {
        public bool IsValid => !string.IsNullOrEmpty(DeliveryId) && Content != null;

        public string DeliveryId { get; set; }

        public long Sequence { get; set; }

        public T Content { get; set; }
    }
}
