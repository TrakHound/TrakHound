// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Drivers;

namespace TrakHound.Routing.Routers
{
    internal static class TrakHoundCommandRoutes
    {
        public const string Run = "Commands.Run";


        public static readonly Dictionary<Type, string> _routes = new Dictionary<Type, string>
        {
            { typeof(ICommandDriver), Run }
        };
    }
}
