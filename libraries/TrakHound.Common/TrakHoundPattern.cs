// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace TrakHound
{
    public static class TrakHoundPattern
    {
        private const string Wildcard = "#";


        public static bool Match(string pattern, string path)
        {
            if (!string.IsNullOrEmpty(pattern) && !string.IsNullOrEmpty(path))
            {
                if (pattern == Wildcard)
                {
                    return true;
                }
                else
                {
                    var patternMatch = pattern.TrimEnd('#');
                    return path.StartsWith(patternMatch);
                }
            }

            return false;
        }
    }
}
