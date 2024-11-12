// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    internal static class TrakHoundBlobRoutes
    {
        public const string Read = "Blobs.Read";
        public const string Publish = "Blobs.Write.Publish";
        public const string Delete = "Blobs.Write.Delete";


        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            { typeof(IBlobReadDriver), Read },
            { typeof(IBlobPublishDriver), Publish },
            { typeof(IBlobDeleteDriver), Delete }
        };
    }
}
