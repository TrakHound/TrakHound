// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public struct TrakHoundTargetResult
    {
        public string Target { get; }

        public string Uuid { get; }


        public TrakHoundTargetResult(string query, string uuid)
        {
            Target = query;
            Uuid = uuid;
        }
    }
}
