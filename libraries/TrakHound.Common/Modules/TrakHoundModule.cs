// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using TrakHound.Packages;

namespace TrakHound.Modules
{
    public sealed class TrakHoundModule : ITrakHoundModule
    {
        public string PackageId { get; set; }

        public string PackageVersion { get; set; }

        public string PackageDirectory { get; set; }

        public TrakHoundPackage Package { get; set; }

        public string PackageReadMe { get; set; }

        public IEnumerable<Type> ModuleTypes { get; set; }


        public TrakHoundModule(TrakHoundPackage package, string packageDirectory, IEnumerable<Type> moduleTypes, string packageReadme = null)
        {
            if (package != null)
            {
                PackageId = package.Id;
                PackageVersion = package.Version;
                PackageDirectory = packageDirectory;
                Package = package;
            }

            PackageReadMe = packageReadme;

            ModuleTypes = moduleTypes;
        }
    }
}
