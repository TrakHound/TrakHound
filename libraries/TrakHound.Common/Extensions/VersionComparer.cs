// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Extensions
{
    public class VersionComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x != null && y != null)
            {
                var xV = x.ToVersion();
                var yV = y.ToVersion();

                if (xV != null && yV != null)
                {
                    if (xV < yV) return -1;
                    if (xV > yV) return 1;
                    if (xV == yV) return 0;
                }
                else
                {
                    return string.Compare(x, y);
                }
            }
            else if (x != null)
            {
                return 1;
            }

            return 0;
        }
    }
}
