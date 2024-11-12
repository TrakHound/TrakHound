// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;

namespace TrakHound.Serialization
{
    public abstract class TrakHoundEntityEntryAttribute : Attribute
    {
        public string Category { get; set; }

        public string Class { get; set; }
    }
}
