// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    internal static class TrakHoundSequenceRoutes
    {
        public const string Read = "Sequences.Read";
        public const string Publish = "Sequences.Write.Publish";
        public const string Delete = "Sequences.Write.Delete";


        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            { typeof(ISequenceReadDriver), Read },
            { typeof(ISequencePublishDriver), Publish },
            { typeof(ISequenceDeleteDriver), Delete }
        };
    }
}
