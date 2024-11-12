// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Modules
{
    public interface ITrakHoundModuleManager
    {
        IEnumerable<string> PackageCategories { get; }

        Func<string, string> FilterFunction { get; }

        IEnumerable<ITrakHoundModule> Modules { get; }

        IEnumerable<Type> ModuleTypes { get; }

        EventHandler<ITrakHoundModule> ModuleAdded { get; set; }

        EventHandler<ITrakHoundModule> ModuleRemoved { get; set; }


        ITrakHoundModule Get(string packageId, string packageVersion);

        IEnumerable<Type> GetTypes(string packageId, string packageVersion);
    }
}
