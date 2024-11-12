// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace TrakHound.Api
{
    internal class ApiRouteComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x != null && y != null)
            {
                var xParts = x.Split('/');
                var yParts = y.Split('/');

                for (var i = 0; i < xParts.Length; i++)
                {
                    if (i < yParts.Length)
                    {
                        if (string.Compare(xParts[i], yParts[i]) != 0)
                        {
                            if (IsParameter(xParts[i]) && IsParameter(yParts[i]))
                            {
                                return 0;
                            }
                            else if (IsParameter(xParts[i]))
                            {
                                return 1;
                            }
                            else if (IsParameter(yParts[i]))
                            {
                                return -1;
                            }
                            else
                            {
                                return string.Compare(xParts[i], yParts[i]);
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            else if (x != null)
            {
                return 1;
            }

            return 0;
        }

        //public int Compare(string x, string y)
        //{
        //    if (x != null && y != null)
        //    {
        //        var xParts = x.Split('/');
        //        var yParts = y.Split('/');

        //        if (xParts.Length == yParts.Length)
        //        {
        //            for (var i = 0; i < xParts.Length; i++)
        //            {
        //                if (IsParameter(xParts[i]) && IsParameter(yParts[i]))
        //                {
        //                    return 0;
        //                }
        //                else if (IsParameter(xParts[i]))
        //                {
        //                    return 1;
        //                }
        //                else if (IsParameter(yParts[i]))
        //                {
        //                    return -1;
        //                }
        //                else if (string.Compare(xParts[i], yParts[i]) != 0)
        //                {
        //                    return string.Compare(xParts[i], yParts[i]);
        //                }
        //            }

        //            return string.Compare(x, y);
        //        }
        //        else if (xParts.Length > yParts.Length)
        //        {
        //            return -1;
        //        }
        //        else
        //        {
        //            return 1;
        //        }
        //    }
        //    else if (x != null)
        //    {
        //        return 1;
        //    }

        //    return 0;
        //}

        private static bool IsParameter(string s)
        {
            if (s != null)
            {
                return s.StartsWith('{') && s.EndsWith('}');
            }

            return false;
        }
    }
}
