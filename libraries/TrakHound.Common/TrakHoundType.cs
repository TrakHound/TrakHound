// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Linq;

namespace TrakHound
{
    public static class TrakHoundType
    {
        public static string Combine(params string[] paths)
        {
            if (!paths.IsNullOrEmpty())
            {
                return string.Join('.', paths.Where(o => !string.IsNullOrEmpty(o)));
            }

            return null;
        }

        public static bool IsRoot(string definitionId)
        {
            if (!string.IsNullOrEmpty(definitionId))
            {
                return !definitionId.Contains('.');
            }

            return false;
        }

        public static bool IsMember(string ns, string definitionId)
        {
            if (!string.IsNullOrEmpty(ns) && !string.IsNullOrEmpty(definitionId))
            {
                return definitionId != ns && definitionId.StartsWith(ns);
            }

            return false;
        }

        public static string GetType(string definitionId)
        {
            if (!string.IsNullOrEmpty(definitionId))
            {
                if (definitionId.Contains('.'))
                {
                    var i = definitionId.LastIndexOf('.');
                    if (i > 0 && i < definitionId.Length - 1)
                    {
                        return definitionId.Substring(i + 1);
                    }
                }
            }

            return definitionId;
        }
    }
}
