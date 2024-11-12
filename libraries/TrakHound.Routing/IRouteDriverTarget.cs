// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using TrakHound.Drivers;

namespace TrakHound.Routing
{
    interface IRouteDriverTarget : IRouteTarget
    {
        IEnumerable<ITrakHoundDriver> Drivers { get; set; }

        IEnumerable<TDriver> GetDrivers<TDriver>() where TDriver : ITrakHoundDriver;
    }
}
