// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Text.Json.Serialization;

namespace TrakHound.Requests
{
    public abstract class TrakHoundEntityEntryBase : ITrakHoundEntityEntryOperation
    {
        [JsonIgnore]
        public virtual byte[] EntryId { get; }

        [JsonIgnore]
        public virtual string AssemblyId { get; set; }

        [JsonIgnore]
        public byte Category { get; protected set; }

        [JsonIgnore]
        public byte Class { get; protected set; }

        [JsonIgnore]
        public TrakHoundEntityOperationType OperationType => TrakHoundEntityOperationType.Add;
    }
}
