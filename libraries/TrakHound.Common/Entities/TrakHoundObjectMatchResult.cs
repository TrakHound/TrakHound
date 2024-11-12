// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public struct TrakHoundObjectMatchResult
    {
        public string Pattern { get; set; }

        public string TargetUuid { get; set; }


        public TrakHoundObjectMatchResult(string pattern, string targetUuid)
        {
            Pattern = pattern;
            TargetUuid = targetUuid;
        }
    }
}
