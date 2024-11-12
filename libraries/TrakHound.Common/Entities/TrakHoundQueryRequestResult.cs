// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Entities
{
    public class TrakHoundQueryRequestResult : ITrakHoundQueryRequestResult
    {
        public string Uuid { get; set; }

        public string Query { get; set; }

        public IEnumerable<string> TargetIds { get; set; }

        public IEnumerable<string> RootIds { get; set; }
    }
}
