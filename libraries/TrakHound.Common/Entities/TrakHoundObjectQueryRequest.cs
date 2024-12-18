// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public struct TrakHoundObjectQueryRequest
    {
        public TrakHoundObjectQueryRequestType Type { get; set; }

        public string Namespace { get; set; }

        public IEnumerable<string> Queries { get; set; }

        public int ParentLevel { get; set; }

        public IEnumerable<string> ParentUuids { get; set; }
    }
}
