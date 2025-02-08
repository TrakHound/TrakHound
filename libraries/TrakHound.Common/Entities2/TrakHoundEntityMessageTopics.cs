// Copyright (c) 2025 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound.Entities
{
    public static class TrakHoundEntityMessageTopics
    {
        public static bool IsMatch(string pattern, string topic)
        {
            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(topic))
            {
                if (pattern.EndsWith('#'))
                {
                    var patternMatch = pattern.TrimEnd('#');
                    return topic.StartsWith(patternMatch);
                }
                else
                {
                    return topic == pattern;
                }
            }

            return false;
        }
    }
}
