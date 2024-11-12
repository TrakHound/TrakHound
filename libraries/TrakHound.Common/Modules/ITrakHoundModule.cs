// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Packages;

namespace TrakHound.Modules
{
    public interface ITrakHoundModule
    {
        string PackageId { get; }

        string PackageVersion { get; }

        string PackageDirectory { get; }

        public TrakHoundPackage Package { get; }

        public string PackageReadMe { get; }

        IEnumerable<Type> ModuleTypes { get; }
    }
}
